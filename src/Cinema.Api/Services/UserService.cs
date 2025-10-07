using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Cinema.Domain.Entities;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;

namespace Cinema.Api.Services
{
    public class UserService
    {
        private readonly string _firebaseConfigPath;
        private readonly string _firebaseApiKey;
        // Simulación de almacenamiento de roles (reemplaza por tu base de datos real)
        private static readonly ConcurrentDictionary<string, string> UserRoles = new();

        public UserService(IConfiguration configuration)
        {
            _firebaseConfigPath = configuration["Firebase:ConfigPath"];
            _firebaseApiKey = configuration["Firebase:apiKey"];
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

        // Método para autenticar usuario con email y password usando la API REST de Firebase
        public async Task<string> VerifyUserPasswordAsync(string email, string password)
        {
            var client = new HttpClient();
            var requestUri = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_firebaseApiKey}";

            var payload = new
            {
                email,
                password,
                returnSecureToken = true
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(requestUri, content);

            if (!response.IsSuccessStatusCode)
                return null;

            var responseString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseString);
            if (doc.RootElement.TryGetProperty("idToken", out var idToken))
                return idToken.GetString();

            return null;
        }

        // Crea un usuario en Firebase y lo registra en el almacenamiento de roles
        public async Task<User> CreateUserAsync(User user, string password)
        {
            EnsureFirebaseInitialized();
            var args = new UserRecordArgs
            {
                Email = user.Email,
                EmailVerified = user.EmailVerified,
                DisplayName = user.DisplayName,
                Password = password,
                Disabled = user.Disabled
            };
            var record = await FirebaseAuth.DefaultInstance.CreateUserAsync(args);

            user.Uid = record.Uid;
            UserRoles[user.Uid] = string.IsNullOrEmpty(user.Role) ? "user" : user.Role;
            return user;
        }

        // Obtiene un usuario por UID desde Firebase y su rol desde el almacenamiento
        public async Task<User> GetUserAsync(string uid)
        {
            EnsureFirebaseInitialized();
            var record = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
            return new User
            {
                Uid = record.Uid,
                Email = record.Email,
                DisplayName = record.DisplayName,
                EmailVerified = record.EmailVerified,
                Disabled = record.Disabled,
                Role = UserRoles.TryGetValue(record.Uid, out var role) ? role : "user"
            };
        }

        // Obtiene un usuario por email desde Firebase y su rol desde el almacenamiento
        public async Task<User> GetUserByEmailAsync(string email)
        {
            EnsureFirebaseInitialized();
            var record = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);
            return new User
            {
                Uid = record.Uid,
                Email = record.Email,
                DisplayName = record.DisplayName,
                EmailVerified = record.EmailVerified,
                Disabled = record.Disabled,
                Role = UserRoles.TryGetValue(record.Uid, out var role) ? role : "user"
            };
        }

        // Elimina un usuario de Firebase y del almacenamiento de roles
        public async Task DeleteUserAsync(string uid)
        {
            EnsureFirebaseInitialized();
            await FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);
            UserRoles.TryRemove(uid, out _);
        }

        // Actualiza los datos del usuario en Firebase y su rol en el almacenamiento
        public async Task<User> UpdateUserAsync(User user)
        {
            EnsureFirebaseInitialized();
            var args = new UserRecordArgs
            {
                Uid = user.Uid,
                Email = user.Email,
                DisplayName = user.DisplayName,
                EmailVerified = user.EmailVerified,
                Disabled = user.Disabled
            };
            var record = await FirebaseAuth.DefaultInstance.UpdateUserAsync(args);

            if (!string.IsNullOrEmpty(user.Role))
                UserRoles[user.Uid] = user.Role;

            return new User
            {
                Uid = record.Uid,
                Email = record.Email,
                DisplayName = record.DisplayName,
                EmailVerified = record.EmailVerified,
                Disabled = record.Disabled,
                Role = UserRoles.TryGetValue(record.Uid, out var role) ? role : "user"
            };
        }

        // Lista todos los usuarios de Firebase y sus roles
        public async Task<IReadOnlyList<User>> ListUsersAsync()
        {
            EnsureFirebaseInitialized();
            var users = new List<User>();
            var pagedEnumerable = FirebaseAuth.DefaultInstance.ListUsersAsync(null);
            await foreach (var record in pagedEnumerable)
            {
                users.Add(new User
                {
                    Uid = record.Uid,
                    Email = record.Email,
                    DisplayName = record.DisplayName,
                    EmailVerified = record.EmailVerified,
                    Disabled = record.Disabled,
                    Role = UserRoles.TryGetValue(record.Uid, out var role) ? role : "user"
                });
            }
            return users;
        }

        // Utilidad para extraer la API Key de Firebase desde el archivo de configuración
        private string GetFirebaseApiKeyFromConfig(string configPath)
        {
            // Implementa aquí la lógica para extraer la API Key de tu archivo JSON de Firebase
            // Por ejemplo, puedes leer el archivo y buscar el campo "apiKey"
            // Este método debe ser adaptado a tu estructura de configuración
            throw new NotImplementedException("Implementa la extracción de la API Key desde tu archivo de configuración.");
        }
    }
}