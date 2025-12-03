using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinema.Domain.Entities;

namespace Cinema.Api.Services
{
    public class FirestoreClaimTicketService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "claimTickets";

        public FirestoreClaimTicketService(IConfiguration configuration)
        {
            var projectId = configuration["Firebase:ProjectId"];
            var configPath = configuration["Firebase:ConfigPath"];
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", configPath);
            _firestoreDb = FirestoreDb.Create(projectId);
        }

        public async Task AddClaimTicketAsync(ClaimTicket ticket)
        {
            if (string.IsNullOrEmpty(ticket.Id))
                ticket.Id = Guid.NewGuid().ToString();

            var docRef = _firestoreDb.Collection(CollectionName).Document(ticket.Id);
            await docRef.SetAsync(ticket);
        }

        public async Task<ClaimTicket?> GetClaimTicketAsync(string id)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            var snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists)
                return null;
            return snapshot.ConvertTo<ClaimTicket>();
        }

        public async Task<List<ClaimTicket>> GetAllClaimTicketsAsync()
        {
            var snapshot = await _firestoreDb.Collection(CollectionName).GetSnapshotAsync();
            var tickets = new List<ClaimTicket>();
            foreach (var doc in snapshot.Documents)
                tickets.Add(doc.ConvertTo<ClaimTicket>());
            return tickets;
        }

        public async Task UpdateClaimTicketAsync(ClaimTicket ticket)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(ticket.Id);
            await docRef.SetAsync(ticket, SetOptions.Overwrite);
        }

        public async Task DeleteClaimTicketAsync(string id)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            await docRef.DeleteAsync();
        }
    }
}