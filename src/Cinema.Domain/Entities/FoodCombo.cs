using Google.Cloud.Firestore;
using System.Collections.Generic;

namespace Cinema.Domain.Entities
{
    [FirestoreData]
    public class FoodCombo
    {
        [FirestoreProperty]
        public string Id { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public string Description { get; set; }

        [FirestoreProperty]
        public double Price { get; set; }

        [FirestoreProperty]
        public List<string> Items { get; set; }

        [FirestoreProperty]
        public string ImageUrl { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Category { get; set; } = string.Empty;

        [FirestoreProperty]
        public bool IsAvailable { get; set; } = true;
    }
}