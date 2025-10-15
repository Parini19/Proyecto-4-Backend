using Google.Cloud.Firestore;

namespace Cinema.Domain.Entities
{
    [FirestoreData]
    public class User
    {
        [FirestoreProperty]
        public string Uid { get; set; }

        [FirestoreProperty]
        public string Email { get; set; }

        [FirestoreProperty]
        public string DisplayName { get; set; }

        [FirestoreProperty]
        public bool EmailVerified { get; set; }

        [FirestoreProperty]
        public bool Disabled { get; set; }

        [FirestoreProperty]
        public string Role { get; set; }

        [FirestoreProperty]
        public string Password { get; set; }
    }
}