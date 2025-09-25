using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cinema.Application.Cinema;
using Cinema.Domain.Entities;

namespace Cinema.Infrastructure.Repositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly List<User> _users = new();

        public Task<User?> GetByIdAsync(string uid, CancellationToken ct = default)
            => Task.FromResult(_users.FirstOrDefault(u => u.Uid == uid));

        public Task<IReadOnlyList<User>> ListAsync(CancellationToken ct = default)
            => Task.FromResult((IReadOnlyList<User>)_users);

        public Task<User> AddAsync(User user, string password, CancellationToken ct = default)
        {
            _users.Add(user);
            return Task.FromResult(user);
        }

        public Task<User> UpdateAsync(User user, CancellationToken ct = default)
        {
            var existing = _users.FirstOrDefault(u => u.Uid == user.Uid);
            if (existing != null)
            {
                existing.Email = user.Email;
                existing.DisplayName = user.DisplayName;
                existing.EmailVerified = user.EmailVerified;
                existing.Disabled = user.Disabled;
            }
            return Task.FromResult(existing ?? user);
        }

        public Task DeleteAsync(string uid, CancellationToken ct = default)
        {
            _users.RemoveAll(u => u.Uid == uid);
            return Task.CompletedTask;
        }

        Task<List<User>> IUserRepository.ListAsync(CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}