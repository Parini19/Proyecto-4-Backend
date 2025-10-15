using Google.Cloud.Firestore;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinema.Domain.Entities;

namespace Cinema.Api.Services
{
    public class FirestoreUserService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "users";

        public FirestoreUserService(IConfiguration configuration)
        {
            var projectId = configuration["Firebase:ProjectId"];
            var configPath = configuration["Firebase:ConfigPath"];
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", configPath);
            _firestoreDb = FirestoreDb.Create(projectId);
        }

        public async Task AddUserAsync(User user)
        {
            if (string.IsNullOrEmpty(user.Uid) || user.Uid == "string")
            {
                user.Uid = Guid.NewGuid().ToString();
            }
            var docRef = _firestoreDb.Collection(CollectionName).Document(user.Uid);
            await docRef.SetAsync(user);
        }

        public async Task<User?> GetUserAsync(string uid)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(uid);
            var snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists)
                return null;
            return snapshot.ConvertTo<User>();
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var snapshot = await _firestoreDb.Collection(CollectionName).GetSnapshotAsync();
            var users = new List<User>();
            foreach (var doc in snapshot.Documents)
                users.Add(doc.ConvertTo<User>());
            return users;
        }

        public async Task UpdateUserAsync(User user)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(user.Uid);
            await docRef.SetAsync(user, SetOptions.Overwrite);
        }

        public async Task DeleteUserAsync(string uid)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(uid);
            await docRef.DeleteAsync();
        }
    }
}
