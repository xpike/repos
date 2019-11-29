using System;
using System.Threading;
using System.Threading.Tasks;
using Example.Library.Models;
using XPike.Caching;

namespace Example.Library.Caching
{
    public interface IUserCache
    {
        Task<bool> SetUserAsync(User user, 
            TimeSpan? ttl = null,
            TimeSpan? extendedTtl = null,
            TimeSpan? timeout = null, 
            CancellationToken? ct = null);

        Task<ICachedItem<User>> GetUserAsync(int userId, TimeSpan? timeout = null, CancellationToken? ct = null);

        Task<bool> InvalidateUserAsync(int userId, TimeSpan? timeout = null, CancellationToken? ct = null);
    }
}