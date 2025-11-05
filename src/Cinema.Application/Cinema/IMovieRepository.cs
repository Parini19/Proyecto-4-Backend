using Cinema.Domain.Entities;

namespace Cinema.Application.Movies;

public interface IMovieRepository
{
    Task<IReadOnlyList<Movie>> ListAsync(CancellationToken ct = default);
    Task<Movie?> GetByIdAsync(string id, CancellationToken ct = default);
    Task AddAsync(Movie movie, CancellationToken ct = default);
    Task UpdateAsync(Movie movie, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}
