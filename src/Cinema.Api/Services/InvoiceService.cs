using Cinema.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema.Api.Services
{
    /// <summary>
    /// Servicio para generar facturas y PDFs.
    /// </summary>
    public class InvoiceService
    {
        private readonly FirestoreInvoiceService _firestoreInvoiceService;
        private readonly FirestoreUserService _userService;
        private readonly FirestoreFoodOrderService _foodOrderService;
        private readonly FirestoreFoodComboService _foodComboService;
        private readonly ILogger<InvoiceService> _logger;
        private readonly double _taxRate;

        public InvoiceService(
            FirestoreInvoiceService firestoreInvoiceService,
            FirestoreUserService userService,
            FirestoreFoodOrderService foodOrderService,
            FirestoreFoodComboService foodComboService,
            IConfiguration configuration,
            ILogger<InvoiceService> logger)
        {
            _firestoreInvoiceService = firestoreInvoiceService;
            _userService = userService;
            _foodOrderService = foodOrderService;
            _foodComboService = foodComboService;
            _logger = logger;
            _taxRate = configuration.GetValue<double>("Invoice:TaxRate", 0.13);

            // Configurar licencia de QuestPDF (Community License)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        /// <summary>
        /// Genera una factura para una reserva y pago.
        /// </summary>
        /// <param name="booking">Reserva</param>
        /// <param name="payment">Pago</param>
        /// <param name="movieTitle">Título de la película</param>
        /// <param name="confirmationEmail">Email de confirmación (opcional, usa el del usuario si no se proporciona)</param>
        public async Task<Invoice> GenerateInvoiceAsync(Booking booking, Payment payment, string movieTitle, string? confirmationEmail = null)
        {
            _logger.LogInformation($"Generating invoice for booking {booking.Id}");

            // Obtener información del usuario
            var user = await _userService.GetUserAsync(booking.UserId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            // Usar email de confirmación si se proporcionó, si no usar el del usuario
            var invoiceEmail = !string.IsNullOrWhiteSpace(confirmationEmail)
                ? confirmationEmail
                : user.Email;

            var invoice = new Invoice
            {
                Id = Guid.NewGuid().ToString(),
                BookingId = booking.Id,
                UserId = booking.UserId,
                IssuedDate = DateTime.UtcNow,
                UserName = user.DisplayName,
                UserEmail = invoiceEmail,
                PaymentMethod = GetPaymentMethodDisplay(payment.PaymentMethod, payment.CardBrand, payment.CardLastFourDigits),
                Items = new List<InvoiceItem>(),
                Subtotal = booking.SubtotalTickets + booking.SubtotalFood,
                Tax = booking.Tax,
                Total = booking.Total
            };

            // Agregar items de boletos
            invoice.Items.Add(new InvoiceItem
            {
                Description = $"Boleto - {movieTitle}",
                Quantity = booking.TicketQuantity,
                UnitPrice = booking.TicketPrice,
                Total = booking.SubtotalTickets
            });

            // Agregar items de comida (si existen)
            if (!string.IsNullOrEmpty(booking.FoodOrderId))
            {
                var foodOrder = await _foodOrderService.GetFoodOrderAsync(booking.FoodOrderId);
                if (foodOrder != null)
                {
                    foreach (var comboId in foodOrder.FoodComboIds)
                    {
                        var combo = await _foodComboService.GetFoodComboAsync(comboId);
                        if (combo != null)
                        {
                            invoice.Items.Add(new InvoiceItem
                            {
                                Description = $"Combo - {combo.Name}",
                                Quantity = 1,
                                UnitPrice = combo.Price,
                                Total = combo.Price
                            });
                        }
                    }
                }
            }

            // Guardar en Firestore (generará número de factura automáticamente)
            await _firestoreInvoiceService.AddInvoiceAsync(invoice);

            _logger.LogInformation($"Invoice {invoice.InvoiceNumber} generated successfully");
            return invoice;
        }

        /// <summary>
        /// Genera un PDF de la factura.
        /// </summary>
        public byte[] GenerateInvoicePdf(Invoice invoice)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(50);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(content => ComposeContent(content, invoice));
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }

        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("MAGIA CINEMA").FontSize(20).Bold().FontColor(Colors.Red.Darken2);
                    column.Item().Text("Sistema de Cine").FontSize(10).FontColor(Colors.Grey.Darken1);
                    column.Item().Text("www.magiacinema.com").FontSize(9).FontColor(Colors.Grey.Medium);
                });

                row.ConstantItem(150).Column(column =>
                {
                    column.Item().AlignRight().Text("FACTURA").FontSize(16).Bold();
                });
            });
        }

        private void ComposeContent(IContainer container, Invoice invoice)
        {
            container.PaddingVertical(20).Column(column =>
            {
                column.Spacing(10);

                // Invoice Info
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text($"Número: {invoice.InvoiceNumber}").Bold();
                        col.Item().Text($"Fecha: {invoice.IssuedDate:dd/MM/yyyy HH:mm}");
                    });

                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text($"Cliente: {invoice.UserName}").Bold();
                        col.Item().Text($"Email: {invoice.UserEmail}");
                        col.Item().Text($"Pago: {invoice.PaymentMethod}");
                    });
                });

                column.Item().PaddingTop(20).LineHorizontal(1).LineColor(Colors.Grey.Medium);

                // Items Table
                column.Item().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                    });

                    // Header
                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Red.Darken2).Padding(5).Text("Descripción").FontColor(Colors.White).Bold();
                        header.Cell().Background(Colors.Red.Darken2).Padding(5).Text("Cant.").FontColor(Colors.White).Bold();
                        header.Cell().Background(Colors.Red.Darken2).Padding(5).Text("Precio").FontColor(Colors.White).Bold();
                        header.Cell().Background(Colors.Red.Darken2).Padding(5).Text("Total").FontColor(Colors.White).Bold();
                    });

                    // Items
                    foreach (var item in invoice.Items)
                    {
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.Description);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignCenter().Text(item.Quantity.ToString());
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text($"${item.UnitPrice:F2}");
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text($"${item.Total:F2}");
                    }
                });

                // Totals
                column.Item().PaddingTop(20).AlignRight().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.ConstantItem(100).Text("Subtotal:");
                        row.ConstantItem(100).AlignRight().Text($"${invoice.Subtotal:F2}");
                    });

                    col.Item().Row(row =>
                    {
                        row.ConstantItem(100).Text("Impuestos:");
                        row.ConstantItem(100).AlignRight().Text($"${invoice.Tax:F2}");
                    });

                    col.Item().PaddingTop(5).BorderTop(2).BorderColor(Colors.Red.Darken2).Row(row =>
                    {
                        row.ConstantItem(100).Text("TOTAL:").Bold().FontSize(14);
                        row.ConstantItem(100).AlignRight().Text($"${invoice.Total:F2}").Bold().FontSize(14).FontColor(Colors.Red.Darken2);
                    });
                });

                // Footer
                column.Item().PaddingTop(40).AlignCenter().Text("¡Gracias por tu compra!").FontSize(10).Italic();
                column.Item().AlignCenter().Text("Conserve esta factura como comprobante de pago").FontSize(8).FontColor(Colors.Grey.Medium);
            });
        }

        private string GetPaymentMethodDisplay(string method, string brand, string lastFour)
        {
            if (method == "credit_card" || method == "debit_card")
            {
                return $"{brand} **** {lastFour}";
            }
            return method;
        }
    }
}
