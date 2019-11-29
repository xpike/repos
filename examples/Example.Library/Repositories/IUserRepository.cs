using System;
using System.Threading;
using System.Threading.Tasks;
using Example.Library.DataStores;
using Example.Library.Models;
using XPike.Repositories;

namespace Example.Library.Repositories
{
    public interface IUserRepository
        : IRepository<IUserDataStore>
    {
        Task<User> GetUserAsync(int userId, TimeSpan? timeout = null, CancellationToken? ct = null);

        Task<int?> CreateUserAsync(User user, TimeSpan? timeout = null, CancellationToken? ct = null);

        Task<bool> UpdateUserAsync(User user, TimeSpan? timeout = null, CancellationToken? ct = null);

        Task<bool> DeleteUserAsync(int userId, TimeSpan? timeout = null, CancellationToken? ct = null);
    }
}