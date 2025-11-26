using Cinema.Domain.Entities;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema.Api.Services
{
    /// <summary>
    /// Servicio para gestionar operaciones CRUD de boletos (Tickets) en Firestore.
    /// </summary>
    public class FirestoreTicketService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "tickets";

        public FirestoreTicketService(IConfiguration configuration)
        {
            var projectId = configuration["Firebase:ProjectId"];
            var credentialPath = configuration["Firebase:ConfigPath"];

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);
            _firestoreDb = FirestoreDb.Create(projectId);
        }

        /// <summary>
        /// Agrega un nuevo boleto a Firestore.
        /// </summary>
        public async Task AddTicketAsync(Ticket ticket)
        {
            if (string.IsNullOrEmpty(ticket.Id))
            {
                ticket.Id = Guid.NewGuid().ToString();
            }

            var docRef = _firestoreDb.Collection(CollectionName).Document(ticket.Id);
            await docRef.SetAsync(ticket);
        }

        /// <summary>
        /// Agrega múltiples boletos en batch.
        /// </summary>
        public async Task AddTicketsBatchAsync(List<Ticket> tickets)
        {
            WriteBatch batch = _firestoreDb.StartBatch();

            foreach (var ticket in tickets)
            {
                if (string.IsNullOrEmpty(ticket.Id))
                {
                    ticket.Id = Guid.NewGuid().ToString();
                }

                var docRef = _firestoreDb.Collection(CollectionName).Document(ticket.Id);
                batch.Set(docRef, ticket);
            }

            await batch.CommitAsync();
        }

        /// <summary>
        /// Obtiene un boleto por su ID.
        /// </summary>
        public async Task<Ticket?> GetTicketAsync(string id)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            var snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                return snapshot.ConvertTo<Ticket>();
            }

            return null;
        }

        /// <summary>
        /// Obtiene todos los boletos de un usuario específico.
        /// </summary>
        public async Task<List<Ticket>> GetTicketsByUserIdAsync(string userId)
        {
            var query = _firestoreDb.Collection(CollectionName).WhereEqualTo("UserId", userId);
            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents.Select(d => d.ConvertTo<Ticket>()).ToList();
        }

        /// <summary>
        /// Obtiene todos los boletos de una reserva específica.
        /// </summary>
        public async Task<List<Ticket>> GetTicketsByBookingIdAsync(string bookingId)
        {
            var query = _firestoreDb.Collection(CollectionName).WhereEqualTo("BookingId", bookingId);
            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents.Select(d => d.ConvertTo<Ticket>()).ToList();
        }

        /// <summary>
        /// Busca un boleto por sus datos de QR.
        /// </summary>
        public async Task<Ticket?> GetTicketByQrCodeDataAsync(string qrCodeData)
        {
            var query = _firestoreDb.Collection(CollectionName).WhereEqualTo("QrCodeData", qrCodeData).Limit(1);
            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents.FirstOrDefault()?.ConvertTo<Ticket>();
        }

        /// <summary>
        /// Actualiza un boleto existente.
        /// </summary>
        public async Task UpdateTicketAsync(Ticket ticket)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(ticket.Id);
            await docRef.SetAsync(ticket, SetOptions.Overwrite);
        }

        /// <summary>
        /// Elimina un boleto por su ID.
        /// </summary>
        public async Task DeleteTicketAsync(string id)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            await docRef.DeleteAsync();
        }

        /// <summary>
        /// Obtiene todos los boletos.
        /// </summary>
        public async Task<List<Ticket>> GetAllTicketsAsync()
        {
            var snapshot = await _firestoreDb.Collection(CollectionName).GetSnapshotAsync();
            return snapshot.Documents.Select(d => d.ConvertTo<Ticket>()).ToList();
        }

        /// <summary>
        /// Marca un boleto como usado (escaneado en la entrada).
        /// </summary>
        public async Task UseTicketAsync(string ticketId)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(ticketId);
            var updates = new Dictionary<string, object>
            {
                { "IsUsed", true },
                { "UsedAt", DateTime.UtcNow }
            };

            await docRef.UpdateAsync(updates);
        }

        /// <summary>
        /// Obtiene boletos activos (no usados y no expirados) de un usuario.
        /// </summary>
        public async Task<List<Ticket>> GetActiveTicketsByUserIdAsync(string userId)
        {
            var query = _firestoreDb.Collection(CollectionName)
                .WhereEqualTo("UserId", userId)
                .WhereEqualTo("IsUsed", false);

            var snapshot = await query.GetSnapshotAsync();
            var tickets = snapshot.Documents.Select(d => d.ConvertTo<Ticket>()).ToList();

            // Filtrar por fecha de expiración en memoria (Firestore no soporta comparaciones de fecha en queries complejos)
            return tickets.Where(t => t.ExpiresAt > DateTime.UtcNow).ToList();
        }
    }
}
