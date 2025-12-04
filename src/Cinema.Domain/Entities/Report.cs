using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace Cinema.Domain.Entities
{
    [FirestoreData]
    public class Report
    {
        [FirestoreProperty]
        public string Id { get; set; }

        [FirestoreProperty]
        public string Name { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Type { get; set; } = string.Empty; // Sales, Attendance, Popular Movies, Revenue, etc.

        [FirestoreProperty]
        public DateTime GeneratedDate { get; set; }

        [FirestoreProperty]
        public DateTime StartDate { get; set; }

        [FirestoreProperty]
        public DateTime EndDate { get; set; }

        [FirestoreProperty]
        public string GeneratedBy { get; set; } = string.Empty;

        [FirestoreProperty]
        public string GeneratedByEmail { get; set; } = string.Empty;

        [FirestoreProperty]
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();

        [FirestoreProperty]
        public string Summary { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Status { get; set; } = "Completed"; // Pending, Processing, Completed, Failed
    }
}
