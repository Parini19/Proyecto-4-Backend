using Cinema.Domain.Entities;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema.Api.Services
{
    /// <summary>
    /// Servicio para gestionar operaciones CRUD de pagos (Payments) en Firestore.
    /// </summary>
    public class FirestorePaymentService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "payments";

        public FirestorePaymentService(IConfiguration configuration)
        {
            var projectId = configuration["Firebase:ProjectId"];
            var credentialPath = configuration["Firebase:ConfigPath"];

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);
            _firestoreDb = FirestoreDb.Create(projectId);
        }

        /// <summary>
        /// Agrega un nuevo pago a Firestore.
        /// </summary>
        public async Task AddPaymentAsync(Payment payment)
        {
            if (string.IsNullOrEmpty(payment.Id))
            {
                payment.Id = Guid.NewGuid().ToString();
            }

            var docRef = _firestoreDb.Collection(CollectionName).Document(payment.Id);
            await docRef.SetAsync(payment);
        }

        /// <summary>
        /// Obtiene un pago por su ID.
        /// </summary>
        public async Task<Payment?> GetPaymentAsync(string id)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            var snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                return snapshot.ConvertTo<Payment>();
            }

            return null;
        }

        /// <summary>
        /// Obtiene el pago asociado a una reserva específica.
        /// </summary>
        public async Task<Payment?> GetPaymentByBookingIdAsync(string bookingId)
        {
            var query = _firestoreDb.Collection(CollectionName).WhereEqualTo("BookingId", bookingId).Limit(1);
            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents.FirstOrDefault()?.ConvertTo<Payment>();
        }

        /// <summary>
        /// Obtiene todos los pagos de un usuario específico.
        /// </summary>
        public async Task<List<Payment>> GetPaymentsByUserIdAsync(string userId)
        {
            var query = _firestoreDb.Collection(CollectionName).WhereEqualTo("UserId", userId);
            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents.Select(d => d.ConvertTo<Payment>()).ToList();
        }

        /// <summary>
        /// Actualiza un pago existente.
        /// </summary>
        public async Task UpdatePaymentAsync(Payment payment)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(payment.Id);
            await docRef.SetAsync(payment, SetOptions.Overwrite);
        }

        /// <summary>
        /// Elimina un pago por su ID.
        /// </summary>
        public async Task DeletePaymentAsync(string id)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            await docRef.DeleteAsync();
        }

        /// <summary>
        /// Obtiene todos los pagos.
        /// </summary>
        public async Task<List<Payment>> GetAllPaymentsAsync()
        {
            var snapshot = await _firestoreDb.Collection(CollectionName).GetSnapshotAsync();
            return snapshot.Documents.Select(d => d.ConvertTo<Payment>()).ToList();
        }

        /// <summary>
        /// Aprueba un pago (actualiza estado y fecha de procesamiento).
        /// </summary>
        public async Task ApprovePaymentAsync(string paymentId, string transactionId)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(paymentId);
            var updates = new Dictionary<string, object>
            {
                { "Status", "approved" },
                { "ProcessedAt", DateTime.UtcNow },
                { "TransactionId", transactionId }
            };

            await docRef.UpdateAsync(updates);
        }

        /// <summary>
        /// Rechaza un pago.
        /// </summary>
        public async Task RejectPaymentAsync(string paymentId, string reason)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(paymentId);
            var updates = new Dictionary<string, object>
            {
                { "Status", "rejected" },
                { "ProcessedAt", DateTime.UtcNow },
                { "RejectionReason", reason }
            };

            await docRef.UpdateAsync(updates);
        }
    }
}
