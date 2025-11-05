using Cinema.Application.Movies;
using Cinema.Domain.Entities;

namespace Cinema.Infrastructure.Repositories
{
    public class InMemoryMovieRepository : IMovieRepository
    {
        private static readonly List<Movie> _movies = new()
        {
            new Movie { Id = "1", Title = "Inception", Year = 2010, Description = "A thief who steals corporate secrets", DurationMinutes = 148, Genre = "Science Fiction", Director = "Christopher Nolan" },
            new Movie { Id = "2", Title = "Interstellar", Year = 2014, Description = "A team of explorers travel through a wormhole", DurationMinutes = 169, Genre = "Science Fiction", Director = "Christopher Nolan" },
            new Movie { Id = "3", Title = "Dune", Year = 2021, Description = "Paul Atreides arrives on the dangerous planet Arrakis", DurationMinutes = 155, Genre = "Science Fiction", Director = "Denis Villeneuve" }
        };

        public Task<IReadOnlyList<Movie>> ListAsync(System.Threading.CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<Movie>>(_movies);

        public Task<Movie?> GetByIdAsync(string id, System.Threading.CancellationToken ct = default)
        {
            var movie = _movies.FirstOrDefault(m => m.Id == id);
            return Task.FromResult(movie);
        }

        public Task AddAsync(Movie movie, System.Threading.CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(movie.Id))
            {
                movie.Id = Guid.NewGuid().ToString();
            }
            _movies.Add(movie);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Movie movie, System.Threading.CancellationToken ct = default)
        {
            var existing = _movies.FirstOrDefault(m => m.Id == movie.Id);
            if (existing != null)
            {
                _movies.Remove(existing);
                _movies.Add(movie);
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id, System.Threading.CancellationToken ct = default)
        {
            var movie = _movies.FirstOrDefault(m => m.Id == id);
            if (movie != null)
            {
                _movies.Remove(movie);
            }
            return Task.CompletedTask;
        }
    }
}
