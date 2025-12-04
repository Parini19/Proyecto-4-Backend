using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace Cinema.Domain.Entities
{
    [FirestoreData]
    public class FoodOrder
    {
        [FirestoreProperty]
        public string Id { get; set; }

        [FirestoreProperty]
        public string UserId { get; set; }

        [FirestoreProperty]
        public List<string> FoodComboIds { get; set; }

        [FirestoreProperty]
        public double TotalPrice { get; set; }

        [FirestoreProperty]
        public string Status { get; set; }

        private DateTime? _createdAt;
        [FirestoreProperty]
        public DateTime? CreatedAt
        {
            get => _createdAt;
            set => _createdAt = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
        }

        private DateTime? _updatedAt;
        [FirestoreProperty]
        public DateTime? UpdatedAt
        {
            get => _updatedAt;
            set => _updatedAt = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
        }
    }
}