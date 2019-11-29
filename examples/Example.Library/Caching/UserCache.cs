using System;
using System.Threading;
using System.Threading.Tasks;
using Example.Library.Models;
using XPike.Caching;

namespace Example.Library.Caching
{
    public class UserCache
        : IUserCache
    {
        private const int _DEFAULT_TTL_MINS = 15;
        private const int _DEFAULT_CACHE_GET_TIMEOUT_MS = 500;
        private const int _DEFAULT_CACHE_SET_TIMEOUT_MS = 1500;

        private readonly ICachingService _cachingService;

        public UserCache(ICachingService cachingService)
        {
            _cachingService = cachingService;
        }

        private string CreateKey(int userId) =>
            $"{typeof(User)}.{nameof(userId)}.{userId}";

        public Task<bool> SetUserAsync(User user,
            TimeSpan? ttl = null,
            TimeSpan? extendedTtl = null,
            TimeSpan? timeout = null,
            CancellationToken? ct = null) =>
            _cachingService.SetValueAsync(null,
                CreateKey(user.UserId),
                user,
                ttl.GetValueOrDefault(TimeSpan.FromMinutes(_DEFAULT_TTL_MINS)),
                extendedTtl,
                timeout.GetValueOrDefault(TimeSpan.FromMilliseconds(_DEFAULT_CACHE_SET_TIMEOUT_MS)),
                ct);

        public Task<ICachedItem<User>> GetUserAsync(int userId,
            TimeSpan? timeout = null,
            CancellationToken? ct = null) =>
            _cachingService.GetItemAsync<User>(null,
                CreateKey(userId),
                timeout.GetValueOrDefault(TimeSpan.FromMilliseconds(_DEFAULT_CACHE_GET_TIMEOUT_MS)),
                ct);
    }
}