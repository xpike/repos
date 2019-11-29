using System;
using System.Threading;
using System.Threading.Tasks;
using Example.Library.Models;
using XPike.DataStores;

namespace Example.Library.DataStores
{
    public interface IUserDataStore
        : IDataStore
    {
        Task<User> GetUserAsync(int userId, TimeSpan? timeout = null, CancellationToken? ct = null);

        Task<bool> CreateUserAsync(User user, TimeSpan? timeout = null, CancellationToken? ct = null);

        Task<bool> UpdateUserAsync(User user, TimeSpan? timeout = null, CancellationToken? ct = null);

        Task<bool> DeleteUserAsync(int userId, TimeSpan? timeout = null, CancellationToken? ct = null);
    }
}