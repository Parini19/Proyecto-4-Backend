using Google.Cloud.Firestore;
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
    }
}