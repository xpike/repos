using System;
using System.Collections.Generic;

namespace XPike.Repositories
{
    public class RepositorySettings<TRepository>
        where TRepository : IRepository
    {
        public Dictionary<string, RepositoryOperationSettings> Operations { get; set; }

        public bool RespectExtendedTtl { get; set; }

        public bool SetExtendedTtl { get; set; }

        public bool RethrowCacheExceptions { get; set; }

        public bool RethrowDataSourceExceptions { get; set; }

        public bool RethrowTimeouts { get; set; }

        public bool SuppressWarningLogs { get; set; }

        public bool WaitForCacheSet { get; set; }

        public bool CancelOperationsOnTimeout { get; set; }

        public bool ThrowOnCacheSetFailure { get; set; }

        public TimeSpan CacheSetTtl { get; set; }

        public TimeSpan? ExtendedTtl { get; set; }

        public TimeSpan CacheGetTimeout { get; set; }

        public TimeSpan CacheSetTimeout { get; set; }

        public TimeSpan DataSourceTimeout { get; set; }

        public TimeSpan Timeout { get; set; }

        public RepositorySettings()
        {
            Operations = new Dictionary<string, RepositoryOperationSettings>();

            // This can be specified in the callback except in automatic mode
            CacheGetTimeout = TimeSpan.FromSeconds(1);

            // This can be specified in the callback except in automatic mode
            CacheSetTtl = TimeSpan.FromMinutes(15);

            // This can be specified in the callback except in automatic mode
            CacheSetTimeout = TimeSpan.FromSeconds(2);

            // This can be specified in the callback
            DataSourceTimeout = TimeSpan.FromSeconds(15);

            // This can be specified in the callback except in automatic mode
            ExtendedTtl = TimeSpan.FromHours(24);

            RethrowCacheExceptions = false;
            RethrowDataSourceExceptions = false;
            RespectExtendedTtl = true;

            // This can be specified in the callback except in automatic mode
            SetExtendedTtl = true;

            // This can be implemented by the consumer and passed as a token
            Timeout = TimeSpan.FromSeconds(20);

            RethrowTimeouts = false;
            SuppressWarningLogs = false;
            
            // This can be specified in the callback except in automatic mode
            WaitForCacheSet = false;

            // This can be specified in the callback except in automatic mode
            ThrowOnCacheSetFailure = false;

            // The callback can use a passed in token except in automatic mode
            CancelOperationsOnTimeout = false;
        }
    }
}