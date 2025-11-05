using Google.Cloud.Firestore;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinema.Domain.Entities;
using BCrypt.Net;

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

            // Hash the password before storing
            if (!string.IsNullOrEmpty(user.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
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
            // Hash the password if it's being updated and is not already hashed
            if (!string.IsNullOrEmpty(user.Password) && !user.Password.StartsWith("$2"))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }

            var docRef = _firestoreDb.Collection(CollectionName).Document(user.Uid);
            await docRef.SetAsync(user, SetOptions.Overwrite);
        }

        public async Task DeleteUserAsync(string uid)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(uid);
            await docRef.DeleteAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var query = _firestoreDb.Collection(CollectionName)
                .WhereEqualTo("Email", email)
                .Limit(1);
            var snapshot = await query.GetSnapshotAsync();
            var doc = snapshot.Documents.FirstOrDefault();
            return doc?.ConvertTo<User>();
        }

        public async Task<bool> VerifyUserPasswordAsync(string email, string password)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null)
                return false;

            return BCrypt.Net.BCrypt.Verify(password, user.Password);
        }
    }
}
