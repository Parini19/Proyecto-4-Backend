using Cinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cinema.Api.Services
{
    /// <summary>
    /// Servicio para enviar correos electrónicos usando Resend.
    /// Incluye templates HTML para confirmaciones, boletos y facturas.
    /// </summary>
    public class EmailService
    {
        private readonly string? _resendApiKey;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly bool _isConfigured;
        private readonly ILogger<EmailService> _logger;
        private readonly HttpClient _httpClient;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger, HttpClient httpClient)
        {
            _resendApiKey = configuration["Resend:ApiKey"];
            _fromEmail = configuration["Resend:FromEmail"] ?? "onboarding@resend.dev";
            _fromName = configuration["Resend:FromName"] ?? "Magia Cinema";
            _isConfigured = !string.IsNullOrEmpty(_resendApiKey);
            _logger = logger;
            _httpClient = httpClient;

            if (_isConfigured)
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_resendApiKey}");
                _logger.LogInformation("✅ Resend configurado correctamente. Emails listos para enviar.");
            }
            else
            {
                _logger.LogWarning("⚠️ Resend API Key is not configured. Emails will be simulated with logs only.");
            }
        }

        /// <summary>
        /// Envía email de confirmación de reserva.
        /// </summary>
        public async Task SendBookingConfirmationAsync(string toEmail, string userName, Booking booking, string movieTitle, string theaterRoom, DateTime showTime)
        {
            var subject = "¡Reserva Confirmada! - Magia Cinema";
            var htmlContent = GenerateBookingConfirmationHtml(userName, booking, movieTitle, theaterRoom, showTime);

            await SendEmailAsync(toEmail, subject, htmlContent);
        }

        /// <summary>
        /// Envía boletos digitales por email con QR codes como attachments inline.
        /// </summary>
        public async Task SendTicketsAsync(string toEmail, string userName, List<Ticket> tickets, string movieTitle)
        {
            var subject = $"Tus Boletos Digitales - {movieTitle}";

            // Generar attachments para cada QR code
            var attachments = tickets.Select((t, index) => new
            {
                content = t.QrCode,  // Base64 del QR
                filename = $"qr-ticket-{index}.png",
                content_id = $"qr-ticket-{index}",
                disposition = "inline"
            }).ToList();

            var htmlContent = GenerateTicketsEmailHtmlWithCid(userName, tickets, movieTitle);

            await SendEmailWithAttachmentsAsync(toEmail, subject, htmlContent, attachments);
        }

        /// <summary>
        /// Envía factura por email.
        /// </summary>
        public async Task SendInvoiceAsync(string toEmail, string userName, Invoice invoice)
        {
            var subject = $"Factura de Compra - {invoice.InvoiceNumber}";
            var htmlContent = GenerateInvoiceEmailHtml(userName, invoice);

            await SendEmailAsync(toEmail, subject, htmlContent);
        }

        /// <summary>
        /// Método genérico para enviar emails usando Resend.
        /// </summary>
        private async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            if (!_isConfigured)
            {
                // Modo simulado - solo logs
                _logger.LogInformation("===== EMAIL SIMULADO =====");
                _logger.LogInformation($"Para: {toEmail}");
                _logger.LogInformation($"Asunto: {subject}");
                _logger.LogInformation($"Contenido HTML: {htmlContent.Substring(0, Math.Min(200, htmlContent.Length))}...");
                _logger.LogInformation("==========================");
                return;
            }

            try
            {
                var emailData = new
                {
                    from = $"{_fromName} <{_fromEmail}>",
                    to = new[] { toEmail },
                    subject = subject,
                    html = htmlContent
                };

                var jsonContent = JsonSerializer.Serialize(emailData);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://api.resend.com/emails", httpContent);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"✅ Email enviado exitosamente a {toEmail}");
                    _logger.LogDebug($"Respuesta de Resend: {responseBody}");
                }
                else
                {
                    _logger.LogError($"❌ Error al enviar email. Status: {response.StatusCode}. Respuesta: {responseBody}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error al enviar email con Resend a {toEmail}");
                // No lanzamos la excepción para que el envío de email sea tolerante a fallos
                // El error ya fue registrado en los logs
            }
        }

        /// <summary>
        /// Método para enviar emails con attachments inline usando Resend.
        /// </summary>
        private async Task SendEmailWithAttachmentsAsync(string toEmail, string subject, string htmlContent, List<object> attachments)
        {
            if (!_isConfigured)
            {
                // Modo simulado - solo logs
                _logger.LogInformation("===== EMAIL SIMULADO CON ATTACHMENTS =====");
                _logger.LogInformation($"Para: {toEmail}");
                _logger.LogInformation($"Asunto: {subject}");
                _logger.LogInformation($"Attachments: {attachments.Count}");
                _logger.LogInformation("=========================================");
                return;
            }

            try
            {
                var emailData = new
                {
                    from = $"{_fromName} <{_fromEmail}>",
                    to = new[] { toEmail },
                    subject = subject,
                    html = htmlContent,
                    attachments = attachments
                };

                var jsonContent = JsonSerializer.Serialize(emailData);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://api.resend.com/emails", httpContent);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"✅ Email con attachments enviado exitosamente a {toEmail}");
                    _logger.LogDebug($"Respuesta de Resend: {responseBody}");
                }
                else
                {
                    _logger.LogError($"❌ Error al enviar email con attachments. Status: {response.StatusCode}. Respuesta: {responseBody}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error al enviar email con attachments a {toEmail}");
                // No lanzamos la excepción para que el envío de email sea tolerante a fallos
            }
        }

        #region HTML Templates

        /// <summary>
        /// Genera HTML para email de confirmación de reserva.
        /// </summary>
        private string GenerateBookingConfirmationHtml(string userName, Booking booking, string movieTitle, string theaterRoom, DateTime showTime)
        {
            var seats = string.Join(", ", booking.SeatNumbers);

            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f4f4f4; }}
        .container {{ max-width: 600px; margin: 20px auto; background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ background: #e50914; color: white; padding: 30px 20px; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .content {{ padding: 30px 20px; }}
        .content h2 {{ color: #333; margin-top: 0; }}
        .info-box {{ background: #f9f9f9; padding: 20px; border-radius: 5px; margin: 20px 0; }}
        .info-row {{ display: flex; justify-content: space-between; padding: 10px 0; border-bottom: 1px solid #ddd; }}
        .info-row:last-child {{ border-bottom: none; }}
        .info-label {{ font-weight: bold; color: #666; }}
        .info-value {{ color: #333; }}
        .total-box {{ background: #e50914; color: white; padding: 15px; text-align: center; border-radius: 5px; margin: 20px 0; }}
        .total-box h3 {{ margin: 0; font-size: 24px; }}
        .footer {{ background: #333; color: white; padding: 20px; text-align: center; font-size: 12px; }}
        .button {{ display: inline-block; background: #e50914; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎬 ¡Reserva Confirmada!</h1>
        </div>
        <div class='content'>
            <h2>Hola {userName},</h2>
            <p>Tu reserva ha sido confirmada exitosamente. ¡Nos vemos en el cine!</p>

            <div class='info-box'>
                <h3 style='margin-top: 0; color: #e50914;'>Detalles de tu Reserva</h3>
                <div class='info-row'>
                    <span class='info-label'>Película:</span>
                    <span class='info-value'>{movieTitle}</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>Fecha:</span>
                    <span class='info-value'>{showTime:dd/MM/yyyy}</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>Hora:</span>
                    <span class='info-value'>{showTime:HH:mm}</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>Sala:</span>
                    <span class='info-value'>{theaterRoom}</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>Asientos:</span>
                    <span class='info-value'>{seats}</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>Cantidad:</span>
                    <span class='info-value'>{booking.TicketQuantity} boleto(s)</span>
                </div>
            </div>

            <div class='total-box'>
                <h3>Total Pagado: ${booking.Total:F2}</h3>
            </div>

            <p><strong>ID de Reserva:</strong> {booking.Id}</p>
            <p>Tus boletos digitales con código QR han sido enviados en un correo separado.</p>

            <p style='margin-top: 30px; color: #666; font-size: 14px;'>
                <strong>Importante:</strong> Llega 15 minutos antes de la función y presenta tu código QR en la entrada.
            </p>
        </div>
        <div class='footer'>
            <p>© 2025 Magia Cinema. Todos los derechos reservados.</p>
            <p>Este es un correo automático, por favor no responder.</p>
        </div>
    </div>
</body>
</html>";
        }

        /// <summary>
        /// Genera HTML para email de boletos.
        /// </summary>
        private string GenerateTicketsEmailHtml(string userName, List<Ticket> tickets, string movieTitle)
        {
            var firstTicket = tickets.First();
            var ticketRows = string.Join("", tickets.Select(t => $@"
                <div class='ticket-card'>
                    <div class='ticket-header'>
                        <h3>{t.MovieTitle}</h3>
                        <span class='seat-badge'>Asiento {t.SeatNumber}</span>
                    </div>
                    <div class='ticket-details'>
                        <p><strong>Sala:</strong> {t.TheaterRoomName}</p>
                        <p><strong>Función:</strong> {t.ShowTime:dd/MM/yyyy HH:mm}</p>
                        <p><strong>Boleto ID:</strong> {t.Id}</p>
                    </div>
                    <div class='qr-container'>
                        <img src='data:image/png;base64,{t.QrCode}' alt='QR Code' style='max-width: 200px; height: auto;' />
                        <p style='font-size: 12px; color: #666;'>Presenta este código en la entrada</p>
                    </div>
                </div>
            "));

            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f4f4f4; }}
        .container {{ max-width: 600px; margin: 20px auto; background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ background: #e50914; color: white; padding: 30px 20px; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .content {{ padding: 30px 20px; }}
        .ticket-card {{ border: 2px solid #e50914; border-radius: 8px; padding: 20px; margin: 20px 0; background: #fff; }}
        .ticket-header {{ display: flex; justify-content: space-between; align-items: center; margin-bottom: 15px; }}
        .ticket-header h3 {{ margin: 0; color: #e50914; }}
        .seat-badge {{ background: #e50914; color: white; padding: 5px 15px; border-radius: 20px; font-weight: bold; }}
        .ticket-details {{ margin: 15px 0; }}
        .ticket-details p {{ margin: 8px 0; color: #333; }}
        .qr-container {{ text-align: center; margin: 20px 0; padding: 20px; background: #f9f9f9; border-radius: 5px; }}
        .footer {{ background: #333; color: white; padding: 20px; text-align: center; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎟️ Tus Boletos Digitales</h1>
        </div>
        <div class='content'>
            <h2>Hola {userName},</h2>
            <p>Aquí están tus boletos digitales para <strong>{movieTitle}</strong>.</p>

            {ticketRows}

            <div style='background: #fff3cd; padding: 15px; border-left: 4px solid #ffc107; margin: 20px 0;'>
                <p style='margin: 0; color: #856404;'><strong>⚠️ Instrucciones Importantes:</strong></p>
                <ul style='color: #856404; margin: 10px 0;'>
                    <li>Llega 15 minutos antes de la función</li>
                    <li>Presenta el código QR en la entrada del cine</li>
                    <li>Cada QR es válido para UN solo uso</li>
                    <li>Los boletos expiran 30 minutos después de iniciada la función</li>
                </ul>
            </div>
        </div>
        <div class='footer'>
            <p>© 2025 Magia Cinema. Todos los derechos reservados.</p>
            <p>¡Disfruta la película!</p>
        </div>
    </div>
</body>
</html>";
        }

        /// <summary>
        /// Genera HTML para email de boletos usando CID para imágenes inline.
        /// </summary>
        private string GenerateTicketsEmailHtmlWithCid(string userName, List<Ticket> tickets, string movieTitle)
        {
            var firstTicket = tickets.First();
            var ticketRows = string.Join("", tickets.Select((t, index) => $@"
                <div class='ticket-card'>
                    <div class='ticket-header'>
                        <h3>{t.MovieTitle}</h3>
                        <span class='seat-badge'>Asiento {t.SeatNumber}</span>
                    </div>
                    <div class='ticket-details'>
                        <p><strong>Sala:</strong> {t.TheaterRoomName}</p>
                        <p><strong>Función:</strong> {t.ShowTime:dd/MM/yyyy HH:mm}</p>
                        <p><strong>Boleto ID:</strong> {t.Id}</p>
                    </div>
                    <div class='qr-container'>
                        <img src='cid:qr-ticket-{index}' alt='QR Code' style='max-width: 200px; height: auto;' />
                        <p style='font-size: 12px; color: #666;'>Presenta este código en la entrada</p>
                    </div>
                </div>
            "));

            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f4f4f4; }}
        .container {{ max-width: 600px; margin: 20px auto; background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ background: #e50914; color: white; padding: 30px 20px; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .content {{ padding: 30px 20px; }}
        .ticket-card {{ border: 2px solid #e50914; border-radius: 8px; padding: 20px; margin: 20px 0; background: #fff; }}
        .ticket-header {{ display: flex; justify-content: space-between; align-items: center; margin-bottom: 15px; }}
        .ticket-header h3 {{ margin: 0; color: #e50914; }}
        .seat-badge {{ background: #e50914; color: white; padding: 5px 15px; border-radius: 20px; font-weight: bold; }}
        .ticket-details {{ margin: 15px 0; }}
        .ticket-details p {{ margin: 8px 0; color: #333; }}
        .qr-container {{ text-align: center; margin: 20px 0; padding: 20px; background: #f9f9f9; border-radius: 5px; }}
        .footer {{ background: #333; color: white; padding: 20px; text-align: center; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎟️ Tus Boletos Digitales</h1>
        </div>
        <div class='content'>
            <h2>Hola {userName},</h2>
            <p>Aquí están tus boletos digitales para <strong>{movieTitle}</strong>.</p>

            {ticketRows}

            <div style='background: #fff3cd; padding: 15px; border-left: 4px solid #ffc107; margin: 20px 0;'>
                <p style='margin: 0; color: #856404;'><strong>⚠️ Instrucciones Importantes:</strong></p>
                <ul style='color: #856404; margin: 10px 0;'>
                    <li>Llega 15 minutos antes de la función</li>
                    <li>Presenta el código QR en la entrada del cine</li>
                    <li>Cada QR es válido para UN solo uso</li>
                    <li>Los boletos expiran 30 minutos después de iniciada la función</li>
                </ul>
            </div>
        </div>
        <div class='footer'>
            <p>© 2025 Magia Cinema. Todos los derechos reservados.</p>
            <p>¡Disfruta la película!</p>
        </div>
    </div>
</body>
</html>";
        }

        /// <summary>
        /// Genera HTML para email de factura.
        /// </summary>
        private string GenerateInvoiceEmailHtml(string userName, Invoice invoice)
        {
            var itemRows = string.Join("", invoice.Items.Select(item => $@"
                <tr>
                    <td style='padding: 10px; border-bottom: 1px solid #ddd;'>{item.Description}</td>
                    <td style='padding: 10px; border-bottom: 1px solid #ddd; text-align: center;'>{item.Quantity}</td>
                    <td style='padding: 10px; border-bottom: 1px solid #ddd; text-align: right;'>${item.UnitPrice:F2}</td>
                    <td style='padding: 10px; border-bottom: 1px solid #ddd; text-align: right;'>${item.Total:F2}</td>
                </tr>
            "));

            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f4f4f4; }}
        .container {{ max-width: 600px; margin: 20px auto; background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ background: #e50914; color: white; padding: 30px 20px; text-align: center; }}
        .content {{ padding: 30px 20px; }}
        table {{ width: 100%; border-collapse: collapse; margin: 20px 0; }}
        th {{ background: #f9f9f9; padding: 12px; text-align: left; border-bottom: 2px solid #ddd; }}
        .totals {{ background: #f9f9f9; padding: 15px; border-radius: 5px; margin: 20px 0; }}
        .totals-row {{ display: flex; justify-content: space-between; padding: 8px 0; }}
        .grand-total {{ font-size: 20px; font-weight: bold; color: #e50914; border-top: 2px solid #e50914; margin-top: 10px; padding-top: 10px; }}
        .footer {{ background: #333; color: white; padding: 20px; text-align: center; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>📄 Factura de Compra</h1>
        </div>
        <div class='content'>
            <div style='background: #f9f9f9; padding: 20px; border-radius: 5px; margin-bottom: 30px;'>
                <h3 style='margin-top: 0; color: #e50914;'>Información de Factura</h3>
                <p><strong>Número de Factura:</strong> {invoice.InvoiceNumber}</p>
                <p><strong>Fecha de Emisión:</strong> {invoice.IssuedDate:dd/MM/yyyy HH:mm}</p>
                <p><strong>Cliente:</strong> {invoice.UserName}</p>
                <p><strong>Email:</strong> {invoice.UserEmail}</p>
                <p><strong>Método de Pago:</strong> {invoice.PaymentMethod}</p>
            </div>

            <h3 style='color: #333;'>Detalle de la Compra</h3>
            <table>
                <thead>
                    <tr>
                        <th>Descripción</th>
                        <th style='text-align: center;'>Cant.</th>
                        <th style='text-align: right;'>Precio Unit.</th>
                        <th style='text-align: right;'>Total</th>
                    </tr>
                </thead>
                <tbody>
                    {itemRows}
                </tbody>
            </table>

            <div class='totals'>
                <div class='totals-row'>
                    <span>Subtotal:</span>
                    <span>${invoice.Subtotal:F2}</span>
                </div>
                <div class='totals-row'>
                    <span>Impuestos:</span>
                    <span>${invoice.Tax:F2}</span>
                </div>
                <div class='totals-row grand-total'>
                    <span>TOTAL:</span>
                    <span>${invoice.Total:F2}</span>
                </div>
            </div>

            <p style='color: #666; font-size: 14px; margin-top: 30px;'>
                Gracias por tu compra. Esta factura es un comprobante de tu transacción.
            </p>
        </div>
        <div class='footer'>
            <p>© 2025 Magia Cinema. Todos los derechos reservados.</p>
            <p>Para consultas, contacta a soporte@magiacinema.com</p>
        </div>
    </div>
</body>
</html>";
        }

        #endregion
    }
}
