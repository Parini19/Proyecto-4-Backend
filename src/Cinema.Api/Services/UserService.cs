using Cinema.Domain.Entities;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

namespace Cinema.Api.Services
{
    public class UserService
    {
        private readonly string _firebaseConfigPath;

        public UserService(string firebaseConfigPath)
        {
            _firebaseConfigPath = firebaseConfigPath;
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

        public async Task<User> CreateUserAsync(User user, string password)
        {
            var args = new UserRecordArgs()
            {
                Email = user.Email,
                EmailVerified = user.EmailVerified,
                Password = password,
                DisplayName = user.DisplayName,
                Disabled = user.Disabled,
            };
            var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(args);
            return new User
            {
                Uid = userRecord.Uid,
                Email = userRecord.Email,
                DisplayName = userRecord.DisplayName,
                EmailVerified = userRecord.EmailVerified,
                Disabled = userRecord.Disabled
            };
        }

        public async Task<User> GetUserAsync(string uid)
        {
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
            return new User
            {
                Uid = userRecord.Uid,
                Email = userRecord.Email,
                DisplayName = userRecord.DisplayName,
                EmailVerified = userRecord.EmailVerified,
                Disabled = userRecord.Disabled
            };
        }

        public async Task DeleteUserAsync(string uid)
        {
            await FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            var args = new UserRecordArgs()
            {
                Uid = user.Uid,
                Email = user.Email,
                DisplayName = user.DisplayName,
                EmailVerified = user.EmailVerified,
                Disabled = user.Disabled,
            };
            var userRecord = await FirebaseAuth.DefaultInstance.UpdateUserAsync(args);
            return new User
            {
                Uid = userRecord.Uid,
                Email = userRecord.Email,
                DisplayName = userRecord.DisplayName,
                EmailVerified = userRecord.EmailVerified,
                Disabled = userRecord.Disabled,
            };
        }

        public async Task<IReadOnlyList<User>> ListUsersAsync()
        {
            var users = new List<User>();
            EnsureFirebaseInitialized();

            var pagedEnumerable = FirebaseAuth.DefaultInstance.ListUsersAsync(null);
            await foreach (var userRecord in pagedEnumerable)
            {
                users.Add(new User
                {
                    Uid = userRecord.Uid,
                    Email = userRecord.Email,
                    DisplayName = userRecord.DisplayName,
                    EmailVerified = userRecord.EmailVerified,
                    Disabled = userRecord.Disabled,
                });
            }
            return users;
        }
    }
}