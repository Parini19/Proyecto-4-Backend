using Google.Cloud.Firestore;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinema.Domain.Entities;

namespace Cinema.Api.Services
{
    public class FirestoreMovieService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "movies";

        public FirestoreMovieService(IConfiguration configuration)
        {
            var projectId = configuration["Firebase:ProjectId"];
            var configPath = configuration["Firebase:ConfigPath"];
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", configPath);
            _firestoreDb = FirestoreDb.Create(projectId);
        }

        public async Task AddMovieAsync(Movie movie)
        {
            if (string.IsNullOrEmpty(movie.Id) || movie.Id == "string")
            {
                movie.Id = Guid.NewGuid().ToString();
            }

            var docRef = _firestoreDb.Collection(CollectionName).Document(movie.Id);
            await docRef.SetAsync(movie);
        }

        public async Task<Movie?> GetMovieAsync(string id)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            var snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists)
                return null;
            return snapshot.ConvertTo<Movie>();
        }

        public async Task<List<Movie>> GetAllMoviesAsync()
        {
            var snapshot = await _firestoreDb.Collection(CollectionName).GetSnapshotAsync();
            var movies = new List<Movie>();
            foreach (var doc in snapshot.Documents)
                movies.Add(doc.ConvertTo<Movie>());
            return movies;
        }

        public async Task UpdateMovieAsync(Movie movie)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(movie.Id);
            await docRef.SetAsync(movie, SetOptions.Overwrite);
        }

        public async Task DeleteMovieAsync(string id)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            await docRef.DeleteAsync();
        }
    }
}