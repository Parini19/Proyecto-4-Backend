using Google.Cloud.Firestore;
using System;

namespace Cinema.Domain.Entities
{
    [FirestoreData]
    public class AuditLog
    {
        [FirestoreProperty]
        public string Id { get; set; }

        [FirestoreProperty]
        public string Action { get; set; } = string.Empty;

        [FirestoreProperty]
        public string EntityType { get; set; } = string.Empty;

        [FirestoreProperty]
        public string EntityId { get; set; } = string.Empty;

        [FirestoreProperty]
        public string UserId { get; set; } = string.Empty;

        [FirestoreProperty]
        public string UserEmail { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Description { get; set; } = string.Empty;

        [FirestoreProperty]
        public DateTime Timestamp { get; set; }

        [FirestoreProperty]
        public string IpAddress { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Details { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Severity { get; set; } = "Info"; // Info, Warning, Error, Critical
    }
}
