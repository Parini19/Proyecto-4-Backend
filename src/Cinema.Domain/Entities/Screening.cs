using Google.Cloud.Firestore;
using System;

namespace Cinema.Domain.Entities
{
    [FirestoreData]
    public class Screening
    {
        [FirestoreProperty]
        public string Id { get; set; }

        [FirestoreProperty]
        public string MovieId { get; set; }

        [FirestoreProperty]
        public string TheaterRoomId { get; set; }

        [FirestoreProperty]
        public DateTime StartTime { get; set; }

        [FirestoreProperty]
        public DateTime EndTime { get; set; }
    }
}