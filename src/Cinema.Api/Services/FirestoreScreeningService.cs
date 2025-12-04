using Google.Cloud.Firestore;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinema.Domain.Entities;
using Cinema.Domain.Common;

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

        /// <summary>
        /// Get screenings with pagination support
        /// </summary>
        public async Task<PaginatedResult<Screening>> GetScreeningsPaginatedAsync(int pageNumber = 1, int pageSize = 50)
        {
            // Clamp page size to maximum 100
            pageSize = Math.Min(pageSize, 100);
            var skip = (pageNumber - 1) * pageSize;

            // Get total count (this is expensive, but necessary for pagination)
            var allSnapshot = await _firestoreDb.Collection(CollectionName).GetSnapshotAsync();
            var totalCount = allSnapshot.Count;

            // Get paginated results
            var screenings = new List<Screening>();
            var documents = allSnapshot.Documents.Skip(skip).Take(pageSize);

            foreach (var doc in documents)
            {
                screenings.Add(doc.ConvertTo<Screening>());
            }

            return new PaginatedResult<Screening>(screenings, totalCount, pageNumber, pageSize);
        }

        /// <summary>
        /// Get screenings by movie with limit
        /// </summary>
        public async Task<List<Screening>> GetScreeningsByMovieIdAsync(string movieId, int limit = 50)
        {
            var query = _firestoreDb.Collection(CollectionName)
                .WhereEqualTo(nameof(Screening.MovieId), movieId)
                .Limit(limit);

            var snapshot = await query.GetSnapshotAsync();
            var screenings = new List<Screening>();

            foreach (var doc in snapshot.Documents)
            {
                screenings.Add(doc.ConvertTo<Screening>());
            }

            return screenings;
        }

        /// <summary>
        /// Get screenings by cinema with limit
        /// </summary>
        public async Task<List<Screening>> GetScreeningsByCinemaIdAsync(string cinemaId, int limit = 50)
        {
            var query = _firestoreDb.Collection(CollectionName)
                .WhereEqualTo(nameof(Screening.CinemaId), cinemaId)
                .Limit(limit);

            var snapshot = await query.GetSnapshotAsync();
            var screenings = new List<Screening>();

            foreach (var doc in snapshot.Documents)
            {
                screenings.Add(doc.ConvertTo<Screening>());
            }

            return screenings;
        }

        /// <summary>
        /// Get ONLY future screenings with limit (most common use case)
        /// </summary>
        public async Task<List<Screening>> GetFutureScreeningsAsync(int limit = 50)
        {
            var now = DateTime.UtcNow;
            var query = _firestoreDb.Collection(CollectionName)
                .WhereGreaterThan(nameof(Screening.StartTime), now)
                .OrderBy(nameof(Screening.StartTime))
                .Limit(limit);

            var snapshot = await query.GetSnapshotAsync();
            var screenings = new List<Screening>();

            foreach (var doc in snapshot.Documents)
            {
                screenings.Add(doc.ConvertTo<Screening>());
            }

            return screenings;
        }

        /// <summary>
        /// Delete ALL screenings (use with caution - for testing/cleanup only)
        /// </summary>
        public async Task<int> DeleteAllScreeningsAsync()
        {
            var snapshot = await _firestoreDb.Collection(CollectionName).GetSnapshotAsync();
            var batch = _firestoreDb.StartBatch();
            int count = 0;

            foreach (var doc in snapshot.Documents)
            {
                batch.Delete(doc.Reference);
                count++;
            }

            await batch.CommitAsync();
            return count;
        }
    }
}