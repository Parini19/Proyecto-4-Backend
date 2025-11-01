using Google.Cloud.Firestore;

namespace Cinema.Domain.Entities
{
    [FirestoreData]
    public class TheaterRoom
    {
        [FirestoreProperty]
        public string Id { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public int Capacity { get; set; }
    }
}