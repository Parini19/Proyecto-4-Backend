using Cinema.Application.Screenings;
using Cinema.Domain.Entities;
using Google.Cloud.Firestore;

namespace Cinema.Infrastructure.Repositories
{
    public class FirestoreScreeningRepository : IScreeningRepository
    {
        private readonly FirestoreDb _db;
        private const string CollectionName = "screenings";

        public FirestoreScreeningRepository(FirestoreDb db) => _db = db;

        public async Task<IReadOnlyList<Screening>> ListAsync(CancellationToken ct = default)
        {
            var query = _db.Collection(CollectionName);
            var snap = await query.GetSnapshotAsync(ct);
            var list = new List<Screening>();
            foreach (var doc in snap.Documents)
            {
                if (doc.Exists)
                {
                    list.Add(doc.ConvertTo<Screening>());
                }
            }
            return list;
        }

        public async Task<Screening?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            var docRef = _db.Collection(CollectionName).Document(id);
            var snapshot = await docRef.GetSnapshotAsync(ct);

            if (!snapshot.Exists)
                return null;

            return snapshot.ConvertTo<Screening>();
        }

        public async Task<IReadOnlyList<Screening>> GetByMovieIdAsync(string movieId, CancellationToken ct = default)
        {
            var query = _db.Collection(CollectionName)
                .WhereEqualTo("MovieId", movieId);
            var snap = await query.GetSnapshotAsync(ct);
            var list = new List<Screening>();
            foreach (var doc in snap.Documents)
            {
                if (doc.Exists)
                {
                    list.Add(doc.ConvertTo<Screening>());
                }
            }
            return list;
        }

        public async Task AddAsync(Screening screening, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(screening.Id))
            {
                screening.Id = Guid.NewGuid().ToString();
            }

            var docRef = _db.Collection(CollectionName).Document(screening.Id);
            await docRef.SetAsync(screening, cancellationToken: ct);
        }

        public async Task UpdateAsync(Screening screening, CancellationToken ct = default)
        {
            var docRef = _db.Collection(CollectionName).Document(screening.Id);
            await docRef.SetAsync(screening, SetOptions.Overwrite, ct);
        }

        public async Task DeleteAsync(string id, CancellationToken ct = default)
        {
            var docRef = _db.Collection(CollectionName).Document(id);
            await docRef.DeleteAsync(cancellationToken: ct);
        }
    }
}
