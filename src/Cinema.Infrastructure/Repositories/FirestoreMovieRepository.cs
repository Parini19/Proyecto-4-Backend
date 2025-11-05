using Cinema.Application.Movies;
using Cinema.Domain.Entities;
using Google.Cloud.Firestore;

namespace Cinema.Infrastructure.Repositories
{
    public class FirestoreMovieRepository : IMovieRepository
    {
        private readonly FirestoreDb _db;
        private const string CollectionName = "movies";

        public FirestoreMovieRepository(FirestoreDb db) => _db = db;

        public async Task<IReadOnlyList<Movie>> ListAsync(CancellationToken ct = default)
        {
            var query = _db.Collection(CollectionName);
            var snap = await query.GetSnapshotAsync(ct);
            var list = new List<Movie>();
            foreach (var doc in snap.Documents)
            {
                if (doc.Exists)
                {
                    list.Add(doc.ConvertTo<Movie>());
                }
            }
            return list;
        }

        public async Task<Movie?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            var docRef = _db.Collection(CollectionName).Document(id);
            var snapshot = await docRef.GetSnapshotAsync(ct);

            if (!snapshot.Exists)
                return null;

            return snapshot.ConvertTo<Movie>();
        }

        public async Task AddAsync(Movie movie, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(movie.Id))
            {
                movie.Id = Guid.NewGuid().ToString();
            }

            var docRef = _db.Collection(CollectionName).Document(movie.Id);
            await docRef.SetAsync(movie, cancellationToken: ct);
        }

        public async Task UpdateAsync(Movie movie, CancellationToken ct = default)
        {
            var docRef = _db.Collection(CollectionName).Document(movie.Id);
            await docRef.SetAsync(movie, SetOptions.Overwrite, ct);
        }

        public async Task DeleteAsync(string id, CancellationToken ct = default)
        {
            var docRef = _db.Collection(CollectionName).Document(id);
            await docRef.DeleteAsync(cancellationToken: ct);
        }
    }
}
