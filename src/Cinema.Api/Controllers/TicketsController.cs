using System;
using System.Linq;
using System.Threading.Tasks;
using Cinema.Api.Services;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Cinema.Api.Controllers
{
    /// <summary>
    /// Controlador para gestionar boletos digitales.
    /// Permite consultar boletos, validar QR y descargar PDFs.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly FirestoreTicketService _ticketService;
        private readonly TicketService _ticketBusinessService;
        private readonly ILogger<TicketsController> _logger;

        public TicketsController(
            FirestoreTicketService ticketService,
            TicketService ticketBusinessService,
            ILogger<TicketsController> logger)
        {
            _ticketService = ticketService;
            _ticketBusinessService = ticketBusinessService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene un boleto por ID.
        /// GET /api/tickets/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicket(string id)
        {
            try
            {
                var ticket = await _ticketService.GetTicketAsync(id);
                if (ticket == null)
                    return NotFound(new { success = false, message = "Ticket not found" });

                return Ok(new { success = true, ticket });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting ticket {id}");
                return StatusCode(500, new { success = false, message = "Failed to get ticket", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene todos los boletos de un usuario.
        /// GET /api/tickets/user/{userId}
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserTickets(string userId)
        {
            try
            {
                var tickets = await _ticketBusinessService.GetUserTicketsAsync(userId);
                return Ok(new { success = true, tickets, count = tickets.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting tickets for user {userId}");
                return StatusCode(500, new { success = false, message = "Failed to get tickets", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene boletos activos de un usuario (no usados y no expirados).
        /// GET /api/tickets/user/{userId}/active
        /// </summary>
        [HttpGet("user/{userId}/active")]
        public async Task<IActionResult> GetActiveUserTickets(string userId)
        {
            try
            {
                var tickets = await _ticketBusinessService.GetActiveUserTicketsAsync(userId);
                return Ok(new { success = true, tickets, count = tickets.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting active tickets for user {userId}");
                return StatusCode(500, new { success = false, message = "Failed to get active tickets", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene todos los boletos de una reserva.
        /// GET /api/tickets/booking/{bookingId}
        /// </summary>
        [HttpGet("booking/{bookingId}")]
        public async Task<IActionResult> GetBookingTickets(string bookingId)
        {
            try
            {
                var tickets = await _ticketService.GetTicketsByBookingIdAsync(bookingId);
                return Ok(new { success = true, tickets, count = tickets.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting tickets for booking {bookingId}");
                return StatusCode(500, new { success = false, message = "Failed to get tickets", error = ex.Message });
            }
        }

        /// <summary>
        /// Valida y marca un boleto como usado (para entrada al cine).
        /// POST /api/tickets/validate
        /// </summary>
        [HttpPost("validate")]
        public async Task<IActionResult> ValidateTicket([FromBody] ValidateTicketDto dto)
        {
            try
            {
                _logger.LogInformation($"Validating ticket with QR data");

                var result = await _ticketBusinessService.ValidateAndUseTicketAsync(dto.QrCodeData);

                if (result.IsValid)
                {
                    _logger.LogInformation($"Ticket validated successfully");
                    return Ok(new
                    {
                        success = true,
                        message = result.Message,
                        ticket = new
                        {
                            result.Ticket!.Id,
                            result.Ticket.MovieTitle,
                            result.Ticket.TheaterRoomName,
                            result.Ticket.SeatNumber,
                            result.Ticket.ShowTime
                        }
                    });
                }
                else
                {
                    _logger.LogWarning($"Ticket validation failed: {result.Message}");
                    return BadRequest(new
                    {
                        success = false,
                        message = result.Message,
                        usedAt = result.UsedAt,
                        expiresAt = result.ExpiresAt
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating ticket");
                return StatusCode(500, new { success = false, message = "Failed to validate ticket", error = ex.Message });
            }
        }

        /// <summary>
        /// Descarga un boleto en formato PDF.
        /// GET /api/tickets/{id}/download
        /// </summary>
        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadTicket(string id)
        {
            try
            {
                var ticket = await _ticketService.GetTicketAsync(id);
                if (ticket == null)
                    return NotFound(new { success = false, message = "Ticket not found" });

                // Generar PDF del boleto
                var pdfBytes = GenerateTicketPdf(ticket);

                var fileName = $"Ticket-{ticket.SeatNumber}-{ticket.Id}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading ticket {id}");
                return StatusCode(500, new { success = false, message = "Failed to download ticket", error = ex.Message });
            }
        }

        /// <summary>
        /// Genera un PDF simple del boleto con QR.
        /// </summary>
        private byte[] GenerateTicketPdf(Domain.Entities.Ticket ticket)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A6);
                    page.Margin(20);
                    page.PageColor(Colors.White);

                    page.Content().Column(column =>
                    {
                        column.Spacing(10);

                        // Header
                        column.Item().AlignCenter().Text("MAGIA CINEMA").FontSize(20).Bold().FontColor(Colors.Red.Darken2);
                        column.Item().AlignCenter().Text("BOLETO DIGITAL").FontSize(12).Bold();

                        column.Item().PaddingTop(10).LineHorizontal(2).LineColor(Colors.Red.Darken2);

                        // Información de la película
                        column.Item().PaddingTop(10).Text(ticket.MovieTitle).FontSize(16).Bold();
                        column.Item().Text($"Sala: {ticket.TheaterRoomName}").FontSize(12);
                        column.Item().Text($"Asiento: {ticket.SeatNumber}").FontSize(14).Bold().FontColor(Colors.Red.Darken2);
                        column.Item().Text($"Fecha: {ticket.ShowTime:dd/MM/yyyy}").FontSize(12);
                        column.Item().Text($"Hora: {ticket.ShowTime:HH:mm}").FontSize(12);

                        // QR Code
                        column.Item().PaddingTop(15).AlignCenter().Column(col =>
                        {
                            var qrBytes = Convert.FromBase64String(ticket.QrCode);
                            col.Item().Image(qrBytes).FitWidth();
                            col.Item().PaddingTop(5).Text("Presenta este código en la entrada").FontSize(8).Italic();
                        });

                        // Info adicional
                        column.Item().PaddingTop(10).Text($"ID: {ticket.Id}").FontSize(8).FontColor(Colors.Grey.Medium);
                        column.Item().Text($"Válido hasta: {ticket.ExpiresAt:dd/MM/yyyy HH:mm}").FontSize(8).FontColor(Colors.Grey.Medium);

                        if (ticket.IsUsed)
                        {
                            column.Item().PaddingTop(5).Background(Colors.Red.Lighten3).Padding(5)
                                .Text("BOLETO USADO").FontSize(10).Bold().FontColor(Colors.Red.Darken4);
                        }
                    });
                });
            });

            return document.GeneratePdf();
        }
    }

    #region DTOs

    /// <summary>
    /// DTO para validar un boleto.
    /// </summary>
    public class ValidateTicketDto
    {
        public string QrCodeData { get; set; } = string.Empty;
    }

    #endregion
}
