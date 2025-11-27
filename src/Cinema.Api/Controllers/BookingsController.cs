using System;
using System.Linq;
using System.Threading.Tasks;
using Cinema.Api.Services;
using Cinema.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers
{
    /// <summary>
    /// Controlador para gestionar reservas de boletos.
    /// Maneja la creación, consulta, confirmación y cancelación de reservas.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly FirestoreBookingService _bookingService;
        private readonly FirestoreScreeningService _screeningService;
        private readonly FirestoreFoodOrderService _foodOrderService;
        private readonly ILogger<BookingsController> _logger;
        private readonly double _taxRate;

        public BookingsController(
            FirestoreBookingService bookingService,
            FirestoreScreeningService screeningService,
            FirestoreFoodOrderService foodOrderService,
            IConfiguration configuration,
            ILogger<BookingsController> logger)
        {
            _bookingService = bookingService;
            _screeningService = screeningService;
            _foodOrderService = foodOrderService;
            _logger = logger;
            _taxRate = configuration.GetValue<double>("Invoice:TaxRate", 0.13);
        }

        /// <summary>
        /// Crea una nueva reserva de boletos.
        /// POST /api/bookings/create
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
        {
            try
            {
                _logger.LogInformation($"Creating booking for user {dto.UserId}");

                // Validar que la función existe
                var screening = await _screeningService.GetScreeningAsync(dto.ScreeningId);
                if (screening == null)
                    return NotFound(new { success = false, message = "Screening not found" });

                // Validar que la función es futura
                if (screening.StartTime <= DateTime.UtcNow)
                    return BadRequest(new { success = false, message = "Cannot book past screenings" });

                // Validar cantidad de boletos (máximo 10)
                if (dto.SeatNumbers.Count > 10)
                    return BadRequest(new { success = false, message = "Maximum 10 tickets per booking" });

                // Validar que no haya asientos duplicados
                if (dto.SeatNumbers.Distinct().Count() != dto.SeatNumbers.Count)
                    return BadRequest(new { success = false, message = "Duplicate seats in booking" });

                // Obtener orden de comida si existe
                double subtotalFood = 0;
                if (!string.IsNullOrEmpty(dto.FoodOrderId))
                {
                    var foodOrder = await _foodOrderService.GetFoodOrderAsync(dto.FoodOrderId);
                    if (foodOrder != null)
                    {
                        subtotalFood = foodOrder.TotalPrice;
                    }
                }

                // Calcular totales
                var subtotalTickets = dto.TicketQuantity * dto.TicketPrice;
                var subtotal = subtotalTickets + subtotalFood;
                var tax = subtotal * _taxRate;
                var total = subtotal + tax;

                // Crear reserva
                var booking = new Booking
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = dto.UserId,
                    ScreeningId = dto.ScreeningId,
                    SeatNumbers = dto.SeatNumbers,
                    TicketQuantity = dto.TicketQuantity,
                    TicketPrice = dto.TicketPrice,
                    SubtotalTickets = subtotalTickets,
                    FoodOrderId = dto.FoodOrderId,
                    SubtotalFood = subtotalFood,
                    Tax = tax,
                    Total = total,
                    Status = "pending",
                    CreatedAt = DateTime.UtcNow
                };

                await _bookingService.AddBookingAsync(booking);

                _logger.LogInformation($"Booking {booking.Id} created successfully");

                return Ok(new
                {
                    success = true,
                    booking = new
                    {
                        booking.Id,
                        booking.UserId,
                        booking.ScreeningId,
                        booking.SeatNumbers,
                        booking.TicketQuantity,
                        booking.SubtotalTickets,
                        booking.SubtotalFood,
                        booking.Tax,
                        booking.Total,
                        booking.Status,
                        booking.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                return StatusCode(500, new { success = false, message = "Failed to create booking", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una reserva por ID.
        /// GET /api/bookings/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(string id)
        {
            try
            {
                var booking = await _bookingService.GetBookingAsync(id);
                if (booking == null)
                    return NotFound(new { success = false, message = "Booking not found" });

                return Ok(new { success = true, booking });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting booking {id}");
                return StatusCode(500, new { success = false, message = "Failed to get booking", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene todas las reservas de un usuario.
        /// GET /api/bookings/user/{userId}
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserBookings(string userId)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);
                return Ok(new { success = true, bookings, count = bookings.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting bookings for user {userId}");
                return StatusCode(500, new { success = false, message = "Failed to get bookings", error = ex.Message });
            }
        }

        /// <summary>
        /// Confirma una reserva (después de pago exitoso).
        /// PUT /api/bookings/{id}/confirm
        /// </summary>
        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> ConfirmBooking(string id, [FromBody] ConfirmBookingDto dto)
        {
            try
            {
                var booking = await _bookingService.GetBookingAsync(id);
                if (booking == null)
                    return NotFound(new { success = false, message = "Booking not found" });

                if (booking.Status != "pending")
                    return BadRequest(new { success = false, message = "Booking is not in pending status" });

                await _bookingService.ConfirmBookingAsync(id, dto.PaymentId);

                _logger.LogInformation($"Booking {id} confirmed with payment {dto.PaymentId}");

                return Ok(new { success = true, message = "Booking confirmed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error confirming booking {id}");
                return StatusCode(500, new { success = false, message = "Failed to confirm booking", error = ex.Message });
            }
        }

        /// <summary>
        /// Cancela una reserva.
        /// DELETE /api/bookings/{id}/cancel
        /// </summary>
        [HttpDelete("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(string id)
        {
            try
            {
                var booking = await _bookingService.GetBookingAsync(id);
                if (booking == null)
                    return NotFound(new { success = false, message = "Booking not found" });

                if (booking.Status == "confirmed")
                    return BadRequest(new { success = false, message = "Cannot cancel confirmed bookings. Please contact support." });

                await _bookingService.CancelBookingAsync(id);

                _logger.LogInformation($"Booking {id} cancelled");

                return Ok(new { success = true, message = "Booking cancelled successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling booking {id}");
                return StatusCode(500, new { success = false, message = "Failed to cancel booking", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene todas las reservas (solo para admins).
        /// GET /api/bookings/all
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllBookings()
        {
            try
            {
                var bookings = await _bookingService.GetAllBookingsAsync();
                return Ok(new { success = true, bookings, count = bookings.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all bookings");
                return StatusCode(500, new { success = false, message = "Failed to get bookings", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene los asientos ocupados para una función específica.
        /// GET /api/bookings/occupied-seats/{screeningId}
        /// </summary>
        [HttpGet("occupied-seats/{screeningId}")]
        public async Task<IActionResult> GetOccupiedSeats(string screeningId)
        {
            try
            {
                var allBookings = await _bookingService.GetAllBookingsAsync();
                
                // Filter bookings for this screening that are confirmed or pending
                var screeningBookings = allBookings
                    .Where(b => b.ScreeningId == screeningId && 
                               (b.Status == "confirmed" || b.Status == "pending"))
                    .ToList();

                // Collect all occupied seat numbers
                var occupiedSeats = screeningBookings
                    .SelectMany(b => b.SeatNumbers)
                    .Distinct()
                    .ToList();

                return Ok(new { 
                    success = true, 
                    screeningId = screeningId,
                    occupiedSeats = occupiedSeats,
                    count = occupiedSeats.Count 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting occupied seats for screening {screeningId}");
                return StatusCode(500, new { success = false, message = "Failed to get occupied seats", error = ex.Message });
            }
        }
    }

    #region DTOs

    /// <summary>
    /// DTO para crear una reserva.
    /// </summary>
    public class CreateBookingDto
    {
        public string UserId { get; set; } = string.Empty;
        public string ScreeningId { get; set; } = string.Empty;
        public List<string> SeatNumbers { get; set; } = new List<string>();
        public int TicketQuantity { get; set; }
        public double TicketPrice { get; set; }
        public string? FoodOrderId { get; set; }
    }

    /// <summary>
    /// DTO para confirmar una reserva.
    /// </summary>
    public class ConfirmBookingDto
    {
        public string PaymentId { get; set; } = string.Empty;
    }

    #endregion
}
