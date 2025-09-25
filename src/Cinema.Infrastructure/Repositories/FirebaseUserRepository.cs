using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cinema.Application.Cinema;
using Cinema.Domain.Entities;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

namespace Cinema.Infrastructure.Repositories
{
    public class FirebaseUserRepository : IUserRepository
    {
        private readonly string _firebaseConfigPath;

        public FirebaseUserRepository()
        {
            // You may want to inject config path via DI; for now, use a default or environment variable
            _firebaseConfigPath = Environment.GetEnvironmentVariable("FIREBASE_CONFIG_PATH") ?? "firebase.json";
            EnsureFirebaseInitialized();
        }

        private void EnsureFirebaseInitialized()
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(_firebaseConfigPath)
                });
            }
        }

        public async Task<User?> GetByIdAsync(string uid, CancellationToken ct = default)
        {
            EnsureFirebaseInitialized();
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid, ct);
            return userRecord == null ? null : new User
            {
                Uid = userRecord.Uid,
                Email = userRecord.Email,
                DisplayName = userRecord.DisplayName,
                EmailVerified = userRecord.EmailVerified,
                Disabled = userRecord.Disabled
            };
        }

        public async Task<IReadOnlyList<User>> ListAsync(CancellationToken ct = default)
        {
            EnsureFirebaseInitialized();
            var users = new List<User>();
            var pagedEnumerable = FirebaseAuth.DefaultInstance.ListUsersAsync(null); // Removed ct argument
            await foreach (var userRecord in pagedEnumerable.WithCancellation(ct))
            {
                users.Add(new User
                {
                    Uid = userRecord.Uid,
                    Email = userRecord.Email,
                    DisplayName = userRecord.DisplayName,
                    EmailVerified = userRecord.EmailVerified,
                    Disabled = userRecord.Disabled
                });
            }
            return users;
        }

        public async Task<User> AddAsync(User user, string password, CancellationToken ct = default)
        {
            EnsureFirebaseInitialized();
            var args = new UserRecordArgs()
            {
                Email = user.Email,
                EmailVerified = user.EmailVerified,
                Password = password,
                DisplayName = user.DisplayName,
                Disabled = user.Disabled,
            };
            var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(args, ct);
            return new User
            {
                Uid = userRecord.Uid,
                Email = userRecord.Email,
                DisplayName = userRecord.DisplayName,
                EmailVerified = userRecord.EmailVerified,
                Disabled = userRecord.Disabled
            };
        }

        public async Task<User> UpdateAsync(User user, CancellationToken ct = default)
        {
            EnsureFirebaseInitialized();
            var args = new UserRecordArgs()
            {
                Uid = user.Uid,
                Email = user.Email,
                DisplayName = user.DisplayName,
                EmailVerified = user.EmailVerified,
                Disabled = user.Disabled,
            };
            var userRecord = await FirebaseAuth.DefaultInstance.UpdateUserAsync(args, ct);
            return new User
            {
                Uid = userRecord.Uid,
                Email = userRecord.Email,
                DisplayName = userRecord.DisplayName,
                EmailVerified = userRecord.EmailVerified,
                Disabled = userRecord.Disabled
            };
        }

        public async Task DeleteAsync(string uid, CancellationToken ct = default)
        {
            EnsureFirebaseInitialized();
            await FirebaseAuth.DefaultInstance.DeleteUserAsync(uid, ct);
        }

        Task<List<User>> IUserRepository.ListAsync(CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}