using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cinema.Domain.Entities;

namespace Cinema.Application.Cinema
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string uid, CancellationToken ct = default);

        Task<List<User>> ListAsync(CancellationToken ct = default);

        Task<User> AddAsync(User user, string password, CancellationToken ct = default);

        Task<User> UpdateAsync(User user, CancellationToken ct = default);

        Task DeleteAsync(string uid, CancellationToken ct = default);
    }
}