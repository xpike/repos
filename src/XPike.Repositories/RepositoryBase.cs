using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using XPike.Caching;
using XPike.Logging;
using XPike.Settings;

namespace XPike.Repositories
{
    public abstract class RepositoryBase<TDataSource, TImplementation>
        : IRepository<TDataSource>
        where TImplementation : class, IRepository<TDataSource>
    {
        protected ISettings<RepositorySettings<TImplementation>> Settings { get; }

        protected ICachingService CachingService { get; }

        protected TDataSource DataSource { get; }

        protected ILog<TImplementation> Logger { get; }

        protected RepositoryBase(ISettings<RepositorySettings<TImplementation>> settings,
            ILog<TImplementation> logger,
            ICachingService cachingService,
            TDataSource dataSource)
        {
            CachingService = cachingService;
            DataSource = dataSource;
            Logger = logger;
            Settings = settings;
        }

        protected virtual RepositoryOperationSettings GetOverrides(string operation, RepositoryOperationSettings specific)
        {
            if (string.IsNullOrWhiteSpace(operation))
                return specific;

            var settings = Settings.Value;
            settings.Operations.TryGetValue(operation, out var overrides);

            return new RepositoryOperationSettings
            {
                RethrowDataSourceExceptions = specific?.RethrowDataSourceExceptions ??
                                              overrides?.RethrowDataSourceExceptions,
                RethrowCacheExceptions = specific?.RethrowCacheExceptions ??
                                         overrides?.RethrowCacheExceptions,
                ExtendedTtl = specific?.ExtendedTtl ??
                              overrides?.ExtendedTtl,
                CacheSetTtl = specific?.CacheSetTtl ??
                              overrides?.CacheSetTtl,
                Timeout = specific?.Timeout ??
                          overrides?.Timeout,
                CacheGetTimeout = specific?.CacheGetTimeout ??
                                  overrides?.CacheGetTimeout,
                CancelOperationsOnTimeout = specific?.CancelOperationsOnTimeout ??
                                            overrides?.CancelOperationsOnTimeout,
                SetExtendedTtl = specific?.SetExtendedTtl ??
                                 overrides?.SetExtendedTtl,
                ThrowOnCacheSetFailure = specific?.ThrowOnCacheSetFailure ??
                                         overrides?.ThrowOnCacheSetFailure,
                SuppressWarningLogs = specific?.SuppressWarningLogs ??
                                      overrides?.SuppressWarningLogs,
                WaitForCacheSet = specific?.WaitForCacheSet ??
                                  overrides?.WaitForCacheSet,
                RethrowTimeouts = specific?.RethrowTimeouts ??
                                  overrides?.RethrowTimeouts,
                DataSourceTimeout = specific?.DataSourceTimeout ??
                                    overrides?.DataSourceTimeout,
                CacheSetTimeout = specific?.CacheSetTimeout ??
                                  overrides?.CacheSetTimeout,
                RespectExtendedTtl = specific?.RespectExtendedTtl ??
                                     overrides?.RespectExtendedTtl
            };
        }

        protected virtual RepositorySettings<TImplementation> GetSettings(string operation,
            RepositoryOperationSettings specific)
        {
            var settings = Settings.Value;
            var overrides = GetOverrides(operation, specific);

            return new RepositorySettings<TImplementation>
            {
                RethrowDataSourceExceptions = overrides?.RethrowDataSourceExceptions ??
                                              settings.RethrowDataSourceExceptions,
                RethrowCacheExceptions = overrides?.RethrowCacheExceptions ??
                                         settings.RethrowCacheExceptions,
                ExtendedTtl = overrides?.ExtendedTtl ??
                              settings.ExtendedTtl,
                CacheSetTtl = overrides?.CacheSetTtl ??
                              settings.CacheSetTtl,
                Timeout = overrides?.Timeout ??
                          settings.Timeout,
                CacheGetTimeout = overrides?.CacheGetTimeout ??
                                  settings.CacheGetTimeout,
                CancelOperationsOnTimeout = overrides?.CancelOperationsOnTimeout ??
                                            settings.CancelOperationsOnTimeout,
                SetExtendedTtl = overrides?.SetExtendedTtl ??
                                 settings.SetExtendedTtl,
                ThrowOnCacheSetFailure = overrides?.ThrowOnCacheSetFailure ??
                                         settings.ThrowOnCacheSetFailure,
                SuppressWarningLogs = overrides?.SuppressWarningLogs ??
                                      settings.SuppressWarningLogs,
                WaitForCacheSet = overrides?.WaitForCacheSet ??
                                  settings.WaitForCacheSet,
                RethrowTimeouts = overrides?.RethrowTimeouts ??
                                  settings.RethrowTimeouts,
                DataSourceTimeout = overrides?.DataSourceTimeout ??
                                    settings.DataSourceTimeout,
                CacheSetTimeout = overrides?.CacheSetTimeout ??
                                  settings.CacheSetTimeout,
                RespectExtendedTtl = overrides?.RespectExtendedTtl ??
                                     settings.RespectExtendedTtl
            };
        }

        protected virtual bool IsCachedItemValid<TResult>(ICachedItem<TResult> item, bool respectExtendedTtl)
            where TResult : class
        {
            if (item?.Value == null)
                return false;

            if (item.IsExpired)
                return false;

            if (item.IsStale && !respectExtendedTtl)
                return false;

            return true;
        }

        protected virtual async Task<TResult> DelayForTimeoutAsync<TResult>(TimeSpan timeout)
            where TResult : class
        {
            await Task.Delay(timeout).ConfigureAwait(false);
            return null;
        }

        protected virtual async Task<TResult> ExecuteWithTimeout<TResult>(Func<TimeSpan, CancellationToken?, Task<TResult>> asyncOperation,
            TimeSpan timeout,
            bool cancelOnTimeout,
            CancellationToken? parentToken = null)
            where TResult : class
        {
            if (cancelOnTimeout)
            {
                using (var timeoutSource = new CancellationTokenSource(timeout))
                using (var cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(timeoutSource.Token, parentToken ?? CancellationToken.None))
                    return await (await Task.WhenAny(asyncOperation(timeout, cancellationSource.Token),
                            DelayForTimeoutAsync<TResult>(timeout)).ConfigureAwait(false))
                        .ConfigureAwait(false);
            }

            return await (await Task.WhenAny(asyncOperation(timeout, parentToken),
                    DelayForTimeoutAsync<TResult>(timeout)).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        protected virtual Task<TResult> WithRepositoryAsync<TResult>(Func<TimeSpan, CancellationToken?, Task<TResult>> dbGetAsync,
            string cacheKey = null,
            string cacheConnection = null,
            RepositoryOperationSettings settings = null,
            CancellationToken? ct = null,
            [CallerMemberName] string caller = null)
            where TResult : class
        {
            var effective = GetSettings(caller, settings);

            return WithRepositoryAsync((cancelAfter, token) =>
                    CachingService.GetItemAsync<TResult>(cacheConnection, cacheKey ?? typeof(TResult).FullName,
                        effective.Timeout,
                        token),
                dbGetAsync,
                (item, cancelAfter, token) => CachingService.SetValueAsync(cacheConnection,
                    cacheKey ?? typeof(TResult).FullName,
                    item,
                    effective.CacheSetTtl,
                    effective.SetExtendedTtl ? effective.ExtendedTtl : null,
                    cancelAfter,
                    token),
                settings,
                ct,
                caller);
        }

        protected virtual Task<TResult> WithRepositoryAsync<TResult>(Func<TimeSpan, CancellationToken?, Task<TResult>> cacheGetAsync,
            Func<TimeSpan, CancellationToken?, Task<TResult>> dbGetAsync,
            Func<TResult, TimeSpan, CancellationToken?, Task<bool>> cacheSetAsync = null,
            RepositoryOperationSettings settings = null,
            CancellationToken? ct = null,
            [CallerMemberName] string caller = null)
            where TResult : class
        {
            var metadata = new Dictionary<string, string>
            {
                {"MethodName", $"{nameof(RepositoryBase<TDataSource, TImplementation>)}.{nameof(WithRepositoryAsync)}"},
                {"MethodVariant", "Item"},
                {"Caller", $"{GetType()}.{caller}"},
                {"CachingService", CachingService.GetType().ToString()},
                {"DataSource", typeof(TDataSource).ToString()},
                {"Settings", JsonConvert.SerializeObject(settings)}
            };

            var effective = GetSettings(caller, settings);

            return ExecuteWithTimeout(async (cancelAfter, token) =>
                {
                    try
                    {
                        try
                        {
                            var item = await cacheGetAsync(cancelAfter, token).ConfigureAwait(false);

                            if (item != null)
                                return item;
                        }
                        catch (Exception ex)
                        {
                            if (!effective.SuppressWarningLogs)
                                Logger.Warn(
                                    $"Suppressed exception from cache get operation: {ex.Message} ({ex.GetType()})", ex,
                                    metadata);

                            if (effective.RethrowCacheExceptions)
                                throw;
                        }

                        TResult liveItem;

                        try
                        {
                            liveItem = await dbGetAsync(cancelAfter, token).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            if (!effective.SuppressWarningLogs)
                                Logger.Warn(
                                    $"Suppressed exception from data source get operation: {ex.Message} ({ex.GetType()})",
                                    ex, metadata);

                            if (effective.RethrowDataSourceExceptions)
                                throw;

                            liveItem = null;
                        }

                        if (liveItem != null)
                        {
                            if (cacheSetAsync != null)
                            {
                                try
                                {
                                    if (effective.WaitForCacheSet)
                                    {
                                        if (!await cacheSetAsync(liveItem, cancelAfter, token).ConfigureAwait(false) &&
                                            !effective.SuppressWarningLogs)
                                            Logger.Warn($"Failed to set item in cache: Operation returned false.", null,
                                                metadata);
                                    }
                                    else
                                    {
                                        _ = Task.Run(async () =>
                                            await cacheSetAsync(liveItem, cancelAfter, token).ConfigureAwait(false));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (!effective.SuppressWarningLogs)
                                        Logger.Warn(
                                            $"Suppressed exception from cache set operation: {ex.Message} ({ex.GetType()})",
                                            ex, metadata);

                                    if (effective.RethrowCacheExceptions)
                                        throw;
                                }
                            }

                            return liveItem;
                        }

                        if (!effective.SuppressWarningLogs)
                            Logger.Trace("No object was found using Repository.  Returning null.", metadata);

                        return null;
                    }
                    catch (TaskCanceledException tce)
                    {
                        if (!effective.SuppressWarningLogs)
                            Logger.Warn($"Repository operation was cancelled: {tce.Message} ({tce.GetType()})", tce, metadata);
                        
                        if (effective.RethrowTimeouts)
                            throw;

                        return null;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to retrieve using Repository: {ex.Message} ({ex.GetType()})", ex, metadata);
                        throw;
                    }
                },
                effective.Timeout,
                effective.CancelOperationsOnTimeout,
                ct);
        }

        protected virtual Task<TResult> WithRepositoryAsync<TResult>(Func<TimeSpan, CancellationToken?, Task<ICachedItem<TResult>>> cacheGetAsync,
            Func<TimeSpan, CancellationToken?, Task<TResult>> dbGetAsync,
            Func<TResult, TimeSpan, CancellationToken?, Task<bool>> cacheSetAsync = null,
            RepositoryOperationSettings settings = null,
            CancellationToken? ct = null,
            [CallerMemberName] string caller = null)
            where TResult : class
        {
            var metadata = new Dictionary<string, string>
            {
                {"MethodName", $"{nameof(RepositoryBase<TDataSource, TImplementation>)}.{nameof(WithRepositoryAsync)}"},
                {"MethodVariant", "CachedItem"},
                {"Caller", $"{GetType()}.{caller}"},
                {"CachingService", CachingService.GetType().ToString()},
                {"DataSource", typeof(TDataSource).ToString()},
                {"Settings", JsonConvert.SerializeObject(settings)}
            };

            var effective = GetSettings(caller, settings);
            metadata["EffectiveSettings"] = JsonConvert.SerializeObject(effective);

            return ExecuteWithTimeout(async (cancelAfter, token) =>
                {
                    try
                    {
                        ICachedItem<TResult> item;

                        try
                        {
                            item = await cacheGetAsync(cancelAfter, token).ConfigureAwait(false);

                            if (IsCachedItemValid(item, false))
                                return item.Value;
                        }
                        catch (Exception ex)
                        {
                            if (!effective.SuppressWarningLogs)
                                Logger.Warn($"Suppressed exception from cache get operation: {ex.Message} ({ex.GetType()})", ex, metadata);

                            if (effective.RethrowCacheExceptions)
                                throw;

                            item = null;
                        }

                        TResult liveItem;

                        try
                        {
                            liveItem = await dbGetAsync(cancelAfter, token).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            if (!effective.SuppressWarningLogs)
                                Logger.Warn($"Suppressed exception from data source get operation: {ex.Message} ({ex.GetType()})", ex, metadata);

                            if (effective.RethrowDataSourceExceptions)
                                throw;

                            liveItem = null;
                        }

                        if (liveItem != null)
                        {
                            if (cacheSetAsync != null)
                            {
                                try
                                {
                                    if (effective.WaitForCacheSet)
                                    {
                                        if (!await cacheSetAsync(liveItem, cancelAfter, token).ConfigureAwait(false) &&
                                            !effective.SuppressWarningLogs)
                                            Logger.Warn($"Failed to set item in cache: Operation returned false.", null, metadata);
                                    }
                                    else
                                    {
                                        _ = Task.Run(async () => await cacheSetAsync(liveItem, cancelAfter, token).ConfigureAwait(false));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (!effective.SuppressWarningLogs)
                                        Logger.Warn($"Suppressed exception from cache set operation: {ex.Message} ({ex.GetType()})", ex, metadata);

                                    if (effective.RethrowCacheExceptions)
                                        throw;
                                }
                            }

                            return liveItem;
                        }

                        if (item != null &&
                            effective.RespectExtendedTtl &&
                            IsCachedItemValid(item, true))
                        {
                            if (!effective.SuppressWarningLogs)
                                Logger.Warn($"Returning stale object due to Extended TTL", null, metadata);

                            return item.Value;
                        }

                        if (!effective.SuppressWarningLogs)
                            Logger.Trace("No object was found using Repository.  Returning null.", metadata);

                        return null;
                    }
                    catch (TaskCanceledException tce)
                    {
                        if (!effective.SuppressWarningLogs)
                            Logger.Warn($"Repository operation was cancelled: {tce.Message} ({tce.GetType()})", tce, metadata);

                        if (effective.RethrowTimeouts)
                            throw;

                        return null;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to retrieve using Repository: {ex.Message} ({ex.GetType()})", ex, metadata);
                        throw;
                    }
                },
                effective.Timeout,
                effective.CancelOperationsOnTimeout,
                ct);
        }
    }
}