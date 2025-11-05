using Google.Cloud.Firestore;

namespace Cinema.Domain.Entities
{
    [FirestoreData]
    public class User
    {
        [FirestoreProperty]
        public string? Uid { get; set; }  // Nullable - se genera automáticamente si no se proporciona

        [FirestoreProperty]
        public string Email { get; set; } = string.Empty;

        [FirestoreProperty]
        public string DisplayName { get; set; } = string.Empty;

        [FirestoreProperty]
        public bool EmailVerified { get; set; }

        [FirestoreProperty]
        public bool Disabled { get; set; }

        [FirestoreProperty]
        public string Role { get; set; } = "user";

        [FirestoreProperty]
        public string Password { get; set; } = string.Empty;
    }
}