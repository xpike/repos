using System;
using System.Threading;
using System.Threading.Tasks;
using Example.Library.Caching;
using Example.Library.DataStores;
using Example.Library.Models;
using XPike.Caching;
using XPike.Logging;
using XPike.Repositories;

namespace Example.Library.Repositories
{
    public class UserRepository
        : RepositoryBase<IUserDataStore, UserRepository>,
          IUserRepository
    {
        private readonly int _SET_USER_TTL_MINS = 30;
        private readonly int _SET_USER_EXTENDED_TTL_MINS = 90;

        private readonly IUserCache _cache;

        public UserRepository(IUserCache cache,
            IRepositorySettingsManager settingsManager, 
            ILog<UserRepository> logger, 
            ICachingService cachingService, 
            IUserDataStore dataSource)
            : base(settingsManager, logger, cachingService, dataSource)
        {
            _cache = cache;
        }

        public Task<User> GetUserAsync(int userId, TimeSpan? timeout = null, CancellationToken? ct = null) =>
            WithRepositoryAsync((cancelAfter, token) =>
                    _cache.GetUserAsync(userId, cancelAfter, token),
                (cancelAfter, token) =>
                    DataSource.GetUserAsync(userId, cancelAfter, token),
                (user, cancelAfter, token) =>
                    _cache.SetUserAsync(user,
                        TimeSpan.FromMinutes(_SET_USER_TTL_MINS),
                        TimeSpan.FromMinutes(_SET_USER_EXTENDED_TTL_MINS),
                        cancelAfter,
                        token));
    }
}