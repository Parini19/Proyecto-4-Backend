using Google.Cloud.Firestore;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinema.Domain.Entities;

namespace Cinema.Api.Services
{
    public class FirestoreScreeningService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "screenings";

        public FirestoreScreeningService(IConfiguration configuration)
        {
            var projectId = configuration["Firebase:ProjectId"];
            var configPath = configuration["Firebase:ConfigPath"];
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", configPath);
            _firestoreDb = FirestoreDb.Create(projectId);
        }

        public async Task AddScreeningAsync(Screening screening)
        {
            if (string.IsNullOrEmpty(screening.Id) || screening.Id == "string")
            {
                screening.Id = Guid.NewGuid().ToString();
            }

            var docRef = _firestoreDb.Collection(CollectionName).Document(screening.Id);
            await docRef.SetAsync(screening);
        }

        public async Task<Screening?> GetScreeningAsync(string id)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            var snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists)
                return null;
            return snapshot.ConvertTo<Screening>();
        }

        public async Task<List<Screening>> GetAllScreeningsAsync()
        {
            var snapshot = await _firestoreDb.Collection(CollectionName).GetSnapshotAsync();
            var screenings = new List<Screening>();
            foreach (var doc in snapshot.Documents)
                screenings.Add(doc.ConvertTo<Screening>());
            return screenings;
        }

        public async Task UpdateScreeningAsync(Screening screening)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(screening.Id);
            await docRef.SetAsync(screening, SetOptions.Overwrite);
        }

        public async Task DeleteScreeningAsync(string id)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            await docRef.DeleteAsync();
        }
    }
}