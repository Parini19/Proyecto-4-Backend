using Cinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema.Api.Services
{
    /// <summary>
    /// Servicio de lógica de negocio para manejo de boletos.
    /// Coordina la generación de QR y creación de boletos.
    /// </summary>
    public class TicketService
    {
        private readonly FirestoreTicketService _firestoreTicketService;
        private readonly FirestoreScreeningService _screeningService;
        private readonly FirestoreMovieService _movieService;
        private readonly FirestoreTheaterRoomService _theaterRoomService;
        private readonly QRCodeService _qrCodeService;
        private readonly ILogger<TicketService> _logger;
        private readonly int _expirationMinutes;

        public TicketService(
            FirestoreTicketService firestoreTicketService,
            FirestoreScreeningService screeningService,
            FirestoreMovieService movieService,
            FirestoreTheaterRoomService theaterRoomService,
            QRCodeService qrCodeService,
            IConfiguration configuration,
            ILogger<TicketService> logger)
        {
            _firestoreTicketService = firestoreTicketService;
            _screeningService = screeningService;
            _movieService = movieService;
            _theaterRoomService = theaterRoomService;
            _qrCodeService = qrCodeService;
            _logger = logger;
            _expirationMinutes = configuration.GetValue<int>("Tickets:ExpirationMinutes", 30);
        }

        /// <summary>
        /// Genera boletos para una reserva confirmada.
        /// </summary>
        public async Task<List<Ticket>> GenerateTicketsForBookingAsync(Booking booking)
        {
            _logger.LogInformation($"Generating tickets for booking {booking.Id}");

            // Obtener información de la función
            var screening = await _screeningService.GetScreeningAsync(booking.ScreeningId);
            if (screening == null)
                throw new InvalidOperationException("Screening not found");

            // Obtener información de la película
            var movie = await _movieService.GetMovieAsync(screening.MovieId);
            if (movie == null)
                throw new InvalidOperationException("Movie not found");

            // Obtener información de la sala
            var theaterRoom = await _theaterRoomService.GetTheaterRoomAsync(screening.TheaterRoomId);
            if (theaterRoom == null)
                throw new InvalidOperationException("Theater room not found");

            var tickets = new List<Ticket>();

            // Crear un boleto por cada asiento
            foreach (var seat in booking.SeatNumbers)
            {
                var ticket = new Ticket
                {
                    Id = Guid.NewGuid().ToString(),
                    BookingId = booking.Id,
                    UserId = booking.UserId,
                    ScreeningId = screening.Id,
                    MovieTitle = movie.Title,
                    TheaterRoomName = theaterRoom.Name,
                    SeatNumber = seat,
                    ShowTime = screening.StartTime,
                    IsUsed = false,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = screening.StartTime.AddMinutes(_expirationMinutes)
                };

                // Generar código QR
                ticket.QrCodeData = _qrCodeService.EncodeTicketData(ticket);
                ticket.QrCode = _qrCodeService.GenerateQrCodeForTicket(ticket);

                tickets.Add(ticket);
            }

            // Guardar todos los boletos en Firestore
            await _firestoreTicketService.AddTicketsBatchAsync(tickets);

            _logger.LogInformation($"Generated {tickets.Count} tickets for booking {booking.Id}");
            return tickets;
        }

        /// <summary>
        /// Valida y marca un boleto como usado (para entrada al cine).
        /// </summary>
        public async Task<TicketValidationResult> ValidateAndUseTicketAsync(string qrCodeData)
        {
            _logger.LogInformation("Validating ticket QR code");

            // Validar formato del QR
            if (!_qrCodeService.ValidateQrCodeFormat(qrCodeData))
            {
                return new TicketValidationResult
                {
                    IsValid = false,
                    Message = "Código QR inválido"
                };
            }

            // Buscar el boleto
            var ticket = await _firestoreTicketService.GetTicketByQrCodeDataAsync(qrCodeData);
            if (ticket == null)
            {
                return new TicketValidationResult
                {
                    IsValid = false,
                    Message = "Boleto no encontrado"
                };
            }

            // Verificar si ya fue usado
            if (ticket.IsUsed)
            {
                return new TicketValidationResult
                {
                    IsValid = false,
                    Message = "Este boleto ya fue utilizado",
                    UsedAt = ticket.UsedAt
                };
            }

            // Verificar si expiró
            if (DateTime.UtcNow > ticket.ExpiresAt)
            {
                return new TicketValidationResult
                {
                    IsValid = false,
                    Message = "Este boleto ha expirado",
                    ExpiresAt = ticket.ExpiresAt
                };
            }

            // Marcar como usado
            await _firestoreTicketService.UseTicketAsync(ticket.Id);

            _logger.LogInformation($"Ticket {ticket.Id} validated and marked as used");

            return new TicketValidationResult
            {
                IsValid = true,
                Message = "Boleto válido. Bienvenido a la función.",
                Ticket = ticket
            };
        }

        /// <summary>
        /// Obtiene todos los boletos de un usuario.
        /// </summary>
        public async Task<List<Ticket>> GetUserTicketsAsync(string userId)
        {
            return await _firestoreTicketService.GetTicketsByUserIdAsync(userId);
        }

        /// <summary>
        /// Obtiene boletos activos de un usuario (no usados y no expirados).
        /// </summary>
        public async Task<List<Ticket>> GetActiveUserTicketsAsync(string userId)
        {
            return await _firestoreTicketService.GetActiveTicketsByUserIdAsync(userId);
        }
    }

    /// <summary>
    /// Resultado de validación de boleto.
    /// </summary>
    public class TicketValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public Ticket? Ticket { get; set; }
        public DateTime? UsedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
