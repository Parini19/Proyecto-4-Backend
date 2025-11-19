using Google.Cloud.Firestore;
<<<<<<< Updated upstream
=======
using System.Collections.Generic;
>>>>>>> Stashed changes

namespace Cinema.Domain.Entities
{
    [FirestoreData]
    public class Movie
    {
        [FirestoreProperty]
<<<<<<< Updated upstream
        public string Id { get; set; }

        [FirestoreProperty]
        public string Title { get; set; }

        [FirestoreProperty]
        public string Description { get; set; }

        [FirestoreProperty]
        public int DurationMinutes { get; set; }

        [FirestoreProperty]
        public string Genre { get; set; }

        [FirestoreProperty]
        public string Director { get; set; }
=======
        public string Id { get; set; } = default!;

        [FirestoreProperty]
        public string Title { get; set; } = default!;
>>>>>>> Stashed changes

        [FirestoreProperty]
        public int Year { get; set; }

        [FirestoreProperty]
        public string Description { get; set; } = string.Empty;

        [FirestoreProperty]
        public int DurationMinutes { get; set; }

        [FirestoreProperty]
        public string Genre { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Director { get; set; } = string.Empty;

        [FirestoreProperty]
        public string PosterUrl { get; set; } = string.Empty;

        [FirestoreProperty]
        public string? TrailerUrl { get; set; }

        [FirestoreProperty]
        public double Rating { get; set; }

        [FirestoreProperty]
        public string Classification { get; set; } = string.Empty;

        [FirestoreProperty]
        public bool IsNew { get; set; } = false;

        [FirestoreProperty]
        public List<string> Showtimes { get; set; } = new();
    }
}
