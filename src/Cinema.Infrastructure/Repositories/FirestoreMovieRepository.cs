using Cinema.Application.Movies;
using Cinema.Domain.Entities;
using Google.Cloud.Firestore;

namespace Cinema.Infrastructure.Repositories
{
    public class FirestoreMovieRepository : IMovieRepository
    {
        private readonly FirestoreDb _db;
        public FirestoreMovieRepository(FirestoreDb db) => _db = db;

        public async Task<IReadOnlyList<Movie>> ListAsync(CancellationToken ct = default)
        {
            var query = _db.Collection("movies");
            var snap = await query.GetSnapshotAsync(ct);
            var list = new List<Movie>();
            foreach (var doc in snap.Documents)
            {
                var d = doc.ToDictionary();
                list.Add(new Movie
                {
                    Id = doc.Id,
                    Title = d.TryGetValue("title", out var t) ? t?.ToString() ?? "" : "",
                    Year = d.TryGetValue("year", out var y) && int.TryParse(y?.ToString(), out var yy) ? yy : 0
                });
            }
            return list;
        }
    }
}
