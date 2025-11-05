using Cinema.Domain.Entities;

namespace Cinema.Application.Screenings;

public interface IScreeningRepository
{
    Task<IReadOnlyList<Screening>> ListAsync(CancellationToken ct = default);
    Task<Screening?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyList<Screening>> GetByMovieIdAsync(string movieId, CancellationToken ct = default);
    Task AddAsync(Screening screening, CancellationToken ct = default);
    Task UpdateAsync(Screening screening, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}
