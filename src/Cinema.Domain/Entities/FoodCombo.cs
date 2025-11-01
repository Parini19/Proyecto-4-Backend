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
        public decimal Price { get; set; }

        [FirestoreProperty]
        public List<string> Items { get; set; }
    }
}