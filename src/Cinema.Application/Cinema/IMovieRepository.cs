using Cinema.Domain.Entities;

namespace Cinema.Application.Movies;

public interface IMovieRepository
{
    Task<IReadOnlyList<Movie>> ListAsync(CancellationToken ct = default);
}
