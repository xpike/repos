using System;

namespace XPike.Repositories
{
    public class RepositoryOperationSettings
    {
        public bool? RespectExtendedTtl { get; set; }

        public bool? SetExtendedTtl { get; set; }

        public bool? RethrowCacheExceptions { get; set; }

        public bool? RethrowDataSourceExceptions { get; set; }
        
        public bool? RethrowTimeouts { get; set; }

        public bool? SuppressWarningLogs { get; set; }

        public bool? WaitForCacheSet { get; set; }

        public bool? CancelOperationsOnTimeout { get; set; }

        public bool? ThrowOnCacheSetFailure { get; set; }

        public TimeSpan? CacheSetTtl { get; set; }
        
        public TimeSpan? ExtendedTtl { get; set; }

        public TimeSpan? CacheGetTimeout { get; set; }

        public TimeSpan? CacheSetTimeout { get; set; }

        public TimeSpan? DataSourceTimeout { get; set; }

        public TimeSpan? Timeout { get; set; }
    }
}