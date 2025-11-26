using Cinema.Domain.Entities;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema.Api.Services
{
    /// <summary>
    /// Servicio para gestionar operaciones CRUD de reservas (Bookings) en Firestore.
    /// </summary>
    public class FirestoreBookingService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "bookings";

        public FirestoreBookingService(IConfiguration configuration)
        {
            var projectId = configuration["Firebase:ProjectId"];
            var credentialPath = configuration["Firebase:ConfigPath"];

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);
            _firestoreDb = FirestoreDb.Create(projectId);
        }

        /// <summary>
        /// Agrega una nueva reserva a Firestore.
        /// </summary>
        public async Task AddBookingAsync(Booking booking)
        {
            if (string.IsNullOrEmpty(booking.Id))
            {
                booking.Id = Guid.NewGuid().ToString();
            }

            var docRef = _firestoreDb.Collection(CollectionName).Document(booking.Id);
            await docRef.SetAsync(booking);
        }

        /// <summary>
        /// Obtiene una reserva por su ID.
        /// </summary>
        public async Task<Booking?> GetBookingAsync(string id)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            var snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                return snapshot.ConvertTo<Booking>();
            }

            return null;
        }

        /// <summary>
        /// Obtiene todas las reservas de un usuario específico.
        /// </summary>
        public async Task<List<Booking>> GetBookingsByUserIdAsync(string userId)
        {
            var query = _firestoreDb.Collection(CollectionName).WhereEqualTo("UserId", userId);
            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents.Select(d => d.ConvertTo<Booking>()).ToList();
        }

        /// <summary>
        /// Obtiene todas las reservas de una función específica.
        /// </summary>
        public async Task<List<Booking>> GetBookingsByScreeningIdAsync(string screeningId)
        {
            var query = _firestoreDb.Collection(CollectionName).WhereEqualTo("ScreeningId", screeningId);
            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents.Select(d => d.ConvertTo<Booking>()).ToList();
        }

        /// <summary>
        /// Actualiza una reserva existente.
        /// </summary>
        public async Task UpdateBookingAsync(Booking booking)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(booking.Id);
            await docRef.SetAsync(booking, SetOptions.Overwrite);
        }

        /// <summary>
        /// Elimina una reserva por su ID.
        /// </summary>
        public async Task DeleteBookingAsync(string id)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            await docRef.DeleteAsync();
        }

        /// <summary>
        /// Obtiene todas las reservas.
        /// </summary>
        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            var snapshot = await _firestoreDb.Collection(CollectionName).GetSnapshotAsync();
            return snapshot.Documents.Select(d => d.ConvertTo<Booking>()).ToList();
        }

        /// <summary>
        /// Confirma una reserva (actualiza estado y fecha de confirmación).
        /// </summary>
        public async Task ConfirmBookingAsync(string bookingId, string paymentId)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(bookingId);
            var updates = new Dictionary<string, object>
            {
                { "Status", "confirmed" },
                { "ConfirmedAt", DateTime.UtcNow },
                { "PaymentId", paymentId }
            };

            await docRef.UpdateAsync(updates);
        }

        /// <summary>
        /// Cancela una reserva.
        /// </summary>
        public async Task CancelBookingAsync(string bookingId)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(bookingId);
            await docRef.UpdateAsync("Status", "cancelled");
        }
    }
}
