using Cinema.Domain.Entities;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema.Api.Services
{
    /// <summary>
    /// Servicio para gestionar operaciones CRUD de facturas (Invoices) en Firestore.
    /// </summary>
    public class FirestoreInvoiceService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "invoices";
        private const string CounterCollection = "counters";
        private const string InvoiceCounterDoc = "invoice_counter";

        public FirestoreInvoiceService(IConfiguration configuration)
        {
            var projectId = configuration["Firebase:ProjectId"];
            var credentialPath = configuration["Firebase:ConfigPath"];

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);
            _firestoreDb = FirestoreDb.Create(projectId);
        }

        /// <summary>
        /// Agrega una nueva factura a Firestore.
        /// </summary>
        public async Task AddInvoiceAsync(Invoice invoice)
        {
            if (string.IsNullOrEmpty(invoice.Id))
            {
                invoice.Id = Guid.NewGuid().ToString();
            }

            // Generar número de factura secuencial si no existe
            if (string.IsNullOrEmpty(invoice.InvoiceNumber))
            {
                invoice.InvoiceNumber = await GenerateInvoiceNumberAsync();
            }

            var docRef = _firestoreDb.Collection(CollectionName).Document(invoice.Id);
            await docRef.SetAsync(invoice);
        }

        /// <summary>
        /// Genera un número de factura secuencial único.
        /// Formato: INV-YYYY-NNNN (ej: INV-2025-0001)
        /// </summary>
        private async Task<string> GenerateInvoiceNumberAsync()
        {
            var counterRef = _firestoreDb.Collection(CounterCollection).Document(InvoiceCounterDoc);

            // Usar transacción para garantizar números únicos
            return await _firestoreDb.RunTransactionAsync<string>(async transaction =>
            {
                var snapshot = await transaction.GetSnapshotAsync(counterRef);
                long currentCount = 0;

                if (snapshot.Exists)
                {
                    currentCount = snapshot.GetValue<long>("count");
                }

                long newCount = currentCount + 1;
                transaction.Set(counterRef, new { count = newCount }, SetOptions.Overwrite);

                var year = DateTime.UtcNow.Year;
                return $"INV-{year}-{newCount:D4}";
            });
        }

        /// <summary>
        /// Obtiene una factura por su ID.
        /// </summary>
        public async Task<Invoice?> GetInvoiceAsync(string id)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            var snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                return snapshot.ConvertTo<Invoice>();
            }

            return null;
        }

        /// <summary>
        /// Obtiene la factura asociada a una reserva específica.
        /// </summary>
        public async Task<Invoice?> GetInvoiceByBookingIdAsync(string bookingId)
        {
            var query = _firestoreDb.Collection(CollectionName).WhereEqualTo("BookingId", bookingId).Limit(1);
            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents.FirstOrDefault()?.ConvertTo<Invoice>();
        }

        /// <summary>
        /// Obtiene todas las facturas de un usuario específico.
        /// </summary>
        public async Task<List<Invoice>> GetInvoicesByUserIdAsync(string userId)
        {
            var query = _firestoreDb.Collection(CollectionName).WhereEqualTo("UserId", userId);
            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents.Select(d => d.ConvertTo<Invoice>()).ToList();
        }

        /// <summary>
        /// Busca una factura por su número.
        /// </summary>
        public async Task<Invoice?> GetInvoiceByNumberAsync(string invoiceNumber)
        {
            var query = _firestoreDb.Collection(CollectionName).WhereEqualTo("InvoiceNumber", invoiceNumber).Limit(1);
            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents.FirstOrDefault()?.ConvertTo<Invoice>();
        }

        /// <summary>
        /// Actualiza una factura existente.
        /// </summary>
        public async Task UpdateInvoiceAsync(Invoice invoice)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(invoice.Id);
            await docRef.SetAsync(invoice, SetOptions.Overwrite);
        }

        /// <summary>
        /// Elimina una factura por su ID.
        /// </summary>
        public async Task DeleteInvoiceAsync(string id)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            await docRef.DeleteAsync();
        }

        /// <summary>
        /// Obtiene todas las facturas.
        /// </summary>
        public async Task<List<Invoice>> GetAllInvoicesAsync()
        {
            var snapshot = await _firestoreDb.Collection(CollectionName).GetSnapshotAsync();
            return snapshot.Documents.Select(d => d.ConvertTo<Invoice>()).ToList();
        }

        /// <summary>
        /// Obtiene facturas emitidas en un rango de fechas.
        /// </summary>
        public async Task<List<Invoice>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var query = _firestoreDb.Collection(CollectionName)
                .WhereGreaterThanOrEqualTo("IssuedDate", startDate)
                .WhereLessThanOrEqualTo("IssuedDate", endDate);

            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(d => d.ConvertTo<Invoice>()).ToList();
        }
    }
}
