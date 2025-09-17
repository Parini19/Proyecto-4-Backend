using Cinema.Application.Movies;
using Cinema.Domain.Entities;

namespace Cinema.Infrastructure.Repositories
{
    public class InMemoryMovieRepository : IMovieRepository
    {
        private static readonly IReadOnlyList<Movie> _seed = new[]
        {
        new Movie { Id = "1", Title = "Inception", Year = 2010 },
        new Movie { Id = "2", Title = "Interstellar", Year = 2014 },
        new Movie { Id = "3", Title = "Dune", Year = 2021 }
    };

        public Task<IReadOnlyList<Movie>> ListAsync(System.Threading.CancellationToken ct = default)
            => Task.FromResult(_seed);
    }
}
