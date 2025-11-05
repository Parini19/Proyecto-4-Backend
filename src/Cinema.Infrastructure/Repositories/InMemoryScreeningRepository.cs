using Cinema.Application.Screenings;
using Cinema.Domain.Entities;

namespace Cinema.Infrastructure.Repositories
{
    public class InMemoryScreeningRepository : IScreeningRepository
    {
        private static readonly List<Screening> _screenings = new()
        {
            new Screening
            {
                Id = "1",
                MovieId = "1",
                TheaterRoomId = "1",
                StartTime = DateTime.Now.AddHours(2),
                EndTime = DateTime.Now.AddHours(4)
            },
            new Screening
            {
                Id = "2",
                MovieId = "2",
                TheaterRoomId = "1",
                StartTime = DateTime.Now.AddHours(5),
                EndTime = DateTime.Now.AddHours(8)
            }
        };

        public Task<IReadOnlyList<Screening>> ListAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<Screening>>(_screenings);

        public Task<Screening?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            var screening = _screenings.FirstOrDefault(s => s.Id == id);
            return Task.FromResult(screening);
        }

        public Task<IReadOnlyList<Screening>> GetByMovieIdAsync(string movieId, CancellationToken ct = default)
        {
            var screenings = _screenings.Where(s => s.MovieId == movieId).ToList();
            return Task.FromResult<IReadOnlyList<Screening>>(screenings);
        }

        public Task AddAsync(Screening screening, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(screening.Id))
            {
                screening.Id = Guid.NewGuid().ToString();
            }
            _screenings.Add(screening);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Screening screening, CancellationToken ct = default)
        {
            var existing = _screenings.FirstOrDefault(s => s.Id == screening.Id);
            if (existing != null)
            {
                _screenings.Remove(existing);
                _screenings.Add(screening);
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id, CancellationToken ct = default)
        {
            var screening = _screenings.FirstOrDefault(s => s.Id == id);
            if (screening != null)
            {
                _screenings.Remove(screening);
            }
            return Task.CompletedTask;
        }
    }
}
