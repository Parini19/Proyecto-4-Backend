using Google.Cloud.Firestore;
using System;

namespace Cinema.Domain.Entities
{
    [FirestoreData]
    public class ClaimTicket
    {
        [FirestoreProperty]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [FirestoreProperty]
        public string UserId { get; set; } = string.Empty;

        [FirestoreProperty]
        public string UserEmail { get; set; } = string.Empty; // Added for contact

        [FirestoreProperty]
        public string Title { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Description { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Status { get; set; } = "Open";

        private DateTime _createdAt = DateTime.UtcNow;

        [FirestoreProperty]
        public DateTime CreatedAt
        {
            get => _createdAt;
            set => _createdAt = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        private DateTime? _closedAt;

        [FirestoreProperty]
        public DateTime? ClosedAt
        {
            get => _closedAt;
            set => _closedAt = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
        }
    }
}