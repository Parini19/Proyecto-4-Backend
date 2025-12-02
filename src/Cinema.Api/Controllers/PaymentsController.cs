using System;
using System.Threading.Tasks;
using Cinema.Api.Services;
using Cinema.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers
{
    /// <summary>
    /// Controlador para procesar pagos simulados.
    /// IMPORTANTE: Este sistema NO procesa pagos reales. Es solo para propósitos educativos.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly FirestorePaymentService _paymentService;
        private readonly FirestoreBookingService _bookingService;
        private readonly PaymentSimulationService _paymentSimulation;
        private readonly TicketService _ticketService;
        private readonly InvoiceService _invoiceService;
        private readonly EmailService _emailService;
        private readonly FirestoreUserService _userService;
        private readonly FirestoreScreeningService _screeningService;
        private readonly FirestoreMovieService _movieService;
        private readonly FirestoreTheaterRoomService _theaterRoomService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(
            FirestorePaymentService paymentService,
            FirestoreBookingService bookingService,
            PaymentSimulationService paymentSimulation,
            TicketService ticketService,
            InvoiceService invoiceService,
            EmailService emailService,
            FirestoreUserService userService,
            FirestoreScreeningService screeningService,
            FirestoreMovieService movieService,
            FirestoreTheaterRoomService theaterRoomService,
            ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _bookingService = bookingService;
            _paymentSimulation = paymentSimulation;
            _ticketService = ticketService;
            _invoiceService = invoiceService;
            _emailService = emailService;
            _userService = userService;
            _screeningService = screeningService;
            _movieService = movieService;
            _theaterRoomService = theaterRoomService;
            _logger = logger;
        }

        /// <summary>
        /// Procesa un pago simulado para una reserva.
        /// POST /api/payments/process
        /// </summary>
        [HttpPost("process")]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
        {
            try
            {
                _logger.LogInformation($"Processing payment for booking {request.BookingId}");

                // Validar que la reserva existe
                var booking = await _bookingService.GetBookingAsync(request.BookingId);
                if (booking == null)
                    return NotFound(new { success = false, message = "Booking not found" });

                // Validar que la reserva está en estado pending
                if (booking.Status != "pending")
                    return BadRequest(new { success = false, message = "Booking is not in pending status" });

                // Validar que el monto coincide
                if (Math.Abs(request.Amount - booking.Total) > 0.01)
                    return BadRequest(new { success = false, message = "Payment amount does not match booking total" });

                // Procesar pago simulado
                var paymentResult = await _paymentSimulation.ProcessPaymentAsync(request);

                // Crear registro de pago
                var payment = new Payment
                {
                    Id = Guid.NewGuid().ToString(),
                    BookingId = request.BookingId,
                    UserId = booking.UserId,
                    Amount = request.Amount,
                    PaymentMethod = "credit_card",
                    CardLastFourDigits = _paymentSimulation.GetLastFourDigits(request.CardNumber),
                    CardBrand = _paymentSimulation.DetectCardBrand(request.CardNumber),
                    Status = paymentResult.Status,
                    CreatedAt = DateTime.UtcNow
                };

                if (paymentResult.Success)
                {
                    payment.TransactionId = paymentResult.TransactionId!;
                    payment.ProcessedAt = DateTime.UtcNow;

                    // Guardar pago
                    await _paymentService.AddPaymentAsync(payment);

                    // Confirmar reserva
                    await _bookingService.ConfirmBookingAsync(booking.Id, payment.Id);

                    // Obtener información para emails
                    var user = await _userService.GetUserAsync(booking.UserId);
                    var screening = await _screeningService.GetScreeningAsync(booking.ScreeningId);
                    var movie = await _movieService.GetMovieAsync(screening!.MovieId);
                    var theaterRoom = await _theaterRoomService.GetTheaterRoomAsync(screening.TheaterRoomId);

                    // Generar boletos con QR
                    var tickets = await _ticketService.GenerateTicketsForBookingAsync(booking);

                    // Determinar email destino: usar ConfirmationEmail si se proporcionó, si no usar el del usuario
                    var destinationEmail = !string.IsNullOrWhiteSpace(request.ConfirmationEmail)
                        ? request.ConfirmationEmail
                        : user!.Email;

                    // Generar factura (con el email de confirmación si fue proporcionado)
                    var invoice = await _invoiceService.GenerateInvoiceAsync(booking, payment, movie!.Title, destinationEmail);

                    _logger.LogInformation($"Sending emails to: {destinationEmail} (provided: {request.ConfirmationEmail ?? "none"}, user: {user!.Email})");

                    // Enviar emails con delays para respetar rate limit de Resend (2 req/seg)
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            // Email 1: Confirmación de reserva
                            await _emailService.SendBookingConfirmationAsync(
                                destinationEmail,
                                user.DisplayName,
                                booking,
                                movie.Title,
                                theaterRoom!.Name,
                                screening.StartTime
                            );

                            // Delay de 600ms para respetar rate limit (2 req/seg = 500ms mínimo)
                            await Task.Delay(600);

                            // Email 2: Boletos con QR
                            await _emailService.SendTicketsAsync(
                                destinationEmail,
                                user.DisplayName,
                                tickets,
                                movie.Title
                            );

                            // Delay de 600ms para respetar rate limit
                            await Task.Delay(600);

                            // Email 3: Factura
                            await _emailService.SendInvoiceAsync(
                                destinationEmail,
                                user.DisplayName,
                                invoice
                            );

                            _logger.LogInformation($"✅ All emails sent successfully for booking {booking.Id} to {destinationEmail}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error sending emails for booking {booking.Id}");
                        }
                    });

                    _logger.LogInformation($"Payment {payment.Id} approved for booking {booking.Id}");

                    return Ok(new
                    {
                        success = true,
                        message = "Payment processed successfully",
                        payment = new
                        {
                            payment.Id,
                            payment.BookingId,
                            payment.UserId,
                            payment.Amount,
                            payment.PaymentMethod,
                            payment.CardLastFourDigits,
                            payment.CardBrand,
                            payment.Status,
                            payment.TransactionId,
                            RejectionReason = (string?)null,
                            payment.CreatedAt,
                            payment.ProcessedAt
                        },
                        booking = new
                        {
                            booking.Id,
                            booking.Status
                        },
                        ticketsGenerated = tickets.Count,
                        invoiceNumber = invoice.InvoiceNumber
                    });
                }
                else
                {
                    payment.RejectionReason = paymentResult.Message;
                    payment.ProcessedAt = DateTime.UtcNow;

                    await _paymentService.AddPaymentAsync(payment);

                    _logger.LogWarning($"Payment rejected for booking {booking.Id}: {paymentResult.Message}");

                    return Ok(new
                    {
                        success = false,
                        message = paymentResult.Message,
                        payment = new
                        {
                            payment.Id,
                            payment.BookingId,
                            payment.UserId,
                            payment.Amount,
                            payment.PaymentMethod,
                            payment.CardLastFourDigits,
                            payment.CardBrand,
                            payment.Status,
                            TransactionId = (string?)null,
                            payment.RejectionReason,
                            payment.CreatedAt,
                            payment.ProcessedAt
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment");
                return StatusCode(500, new { success = false, message = "Failed to process payment", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un pago por ID.
        /// GET /api/payments/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPayment(string id)
        {
            try
            {
                var payment = await _paymentService.GetPaymentAsync(id);
                if (payment == null)
                    return NotFound(new { success = false, message = "Payment not found" });

                return Ok(new { success = true, payment });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting payment {id}");
                return StatusCode(500, new { success = false, message = "Failed to get payment", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el pago asociado a una reserva.
        /// GET /api/payments/booking/{bookingId}
        /// </summary>
        [HttpGet("booking/{bookingId}")]
        public async Task<IActionResult> GetPaymentByBooking(string bookingId)
        {
            try
            {
                var payment = await _paymentService.GetPaymentByBookingIdAsync(bookingId);
                if (payment == null)
                    return NotFound(new { success = false, message = "Payment not found for this booking" });

                return Ok(new { success = true, payment });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting payment for booking {bookingId}");
                return StatusCode(500, new { success = false, message = "Failed to get payment", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene todos los pagos de un usuario.
        /// GET /api/payments/user/{userId}
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserPayments(string userId)
        {
            try
            {
                var payments = await _paymentService.GetPaymentsByUserIdAsync(userId);
                return Ok(new { success = true, payments, count = payments.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting payments for user {userId}");
                return StatusCode(500, new { success = false, message = "Failed to get payments", error = ex.Message });
            }
        }
    }
}
