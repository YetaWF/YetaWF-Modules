/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using System;
using System.IO;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Caching.DataProvider;

namespace YetaWF.Modules.Caching.Startup;

/// <summary>
/// An instance of this class is instantiated during application startup and registers all low-level data providers
/// for file and folder I/O, caching and locking.
/// </summary>
/// <remarks>
/// Depending on appsettings.json settings, low-level data providers for single- or multi-instance sites are used.
///
/// Applications do not access these low-level data providers directly.
/// Caching and locking services provided by YetaWF.Core.IO.Caching and
/// file system services provided by YetaWF.Core.IO.FileSystem, YetaWF.Core.IO.DataFilesProvider and YetaWF.Core.IO.FileIO&lt;TObj&gt; should be used instead.
/// </remarks>
public class Application : IInitializeApplicationStartup, IInitializeApplicationStartupFirstNodeOnly {

    internal const string Distributed = "Distributed";
    internal const string DefaultRedisConfig = "localhost:6379";
    internal const string DefaultRedisKeyPrefix = "";
    internal const string DefaultRedisPubSubPrefix = "";

    internal static string LockProvider { get; private set; } = null!;
    internal static string CacheProvider { get; private set; } = null!;
    internal static string? PubSubProvider { get; private set; }
    internal const string SQLCacheProvider = "sql";
    internal const string PostgreSQLCacheProvider = "postgresql";
    internal const string FileCacheProvider = "file";
    internal const string RedisCacheProvider = "redis";
    internal const string LocalLockProvider = "local";

    // Using a Redis server:
    // Start a Redis server using "docker run --name redis -d -p 6379:6379 redis".
    // Run Redis CLI: docker run -it --link redis:redis --rm redis redis-cli -h redis -p 6379
    //                docker run -it --network ..networlk..  --rm redis redis-cli -h redis -p 6379
    // Show all keys: KEYS *
    // Clear DB: FLUSHALL

    /// <summary>
    /// Called when any node of a (single- or multi-instance) site is starting up.
    /// </summary>
    /// <remarks>Installs low-level data providers for single- or multi-instance sites, based on appsettings.json settings.</remarks>
    public async Task InitializeApplicationStartupAsync() {

        Package package = YetaWF.Modules.Caching.AreaRegistration.CurrentPackage;

        // permanently created data providers (never disposed)
        bool distributed = WebConfigHelper.GetValue<bool>(AreaRegistration.CurrentPackage.AreaName, Distributed, false);

        YetaWF.Core.IO.Caching.GetLocalCacheProvider = LocalCacheObjectDataProvider.GetProvider;
        YetaWF.Core.IO.Caching.GetStaticSmallObjectCacheProvider = StaticSmallObjectLocalDataProvider.GetProvider;
        if (distributed) {
            YetaWF.Core.Support.Startup.MultiInstance = true;
            // distributed caching uses local and shared cache
            CacheProvider = WebConfigHelper.GetValue(package.AreaName, "CacheProvider", RedisCacheProvider)!.ToLower();
            if (CacheProvider == RedisCacheProvider) {
                string configString = WebConfigHelper.GetValue(package.AreaName, "RedisCacheConfig", DefaultRedisConfig)!;
                string keyPrefix = WebConfigHelper.GetValue(package.AreaName, "RedisKeyPrefix", DefaultRedisKeyPrefix)!;
                await SharedCacheObjectRedisDataProvider.InitAsync(configString, keyPrefix);
                await StaticObjectMultiRedisDataProvider.InitAsync(configString, keyPrefix);
                YetaWF.Core.IO.Caching.GetSharedCacheProvider = SharedCacheObjectRedisDataProvider.GetProvider;
                YetaWF.Core.IO.Caching.GetStaticCacheProvider = StaticObjectMultiRedisDataProvider.GetProvider;
            } else if (CacheProvider == SQLCacheProvider) {
                YetaWF.Core.IO.Caching.GetSharedCacheProvider = SharedCacheObjectSQLDataProvider.GetProvider;
                YetaWF.Core.IO.Caching.GetStaticCacheProvider = StaticObjectMultiSQLDataProvider.GetProvider;
            } else if (CacheProvider == PostgreSQLCacheProvider) {
                YetaWF.Core.IO.Caching.GetSharedCacheProvider = SharedCacheObjectPostgreSQLDataProvider.GetProvider;
                YetaWF.Core.IO.Caching.GetStaticCacheProvider = StaticObjectMultiPostgreSQLDataProvider.GetProvider;
            } else {
                throw new InternalError($"Unsupported cache provider: {CacheProvider}");
            }
        } else {
            YetaWF.Core.Support.Startup.MultiInstance = false;
            // non-distributed caching uses local cache only
            YetaWF.Core.IO.Caching.GetSharedCacheProvider = LocalCacheObjectDataProvider.GetProvider;
            YetaWF.Core.IO.Caching.GetStaticCacheProvider = StaticObjectSingleDataProvider.GetProvider;
        }

        // Lock provider
        LockProvider = WebConfigHelper.GetValue(package.AreaName, "LockProvider", distributed ? FileCacheProvider : LocalLockProvider)!.ToLower();
        if (LockProvider == FileCacheProvider) {
            // create the lock folder if it doesn't exist
            string rootFolder = GetRootFolder();
            await YetaWF.Core.IO.FileSystem.FileSystemProvider.CreateDirectoryAsync(rootFolder);
            YetaWF.Core.IO.Caching.LockProvider = new LockFileProvider(rootFolder);
        } else if (LockProvider == RedisCacheProvider) {
            string configString = WebConfigHelper.GetValue(package.AreaName, "RedisLockConfig", DefaultRedisConfig)!;
            string keyPrefix = WebConfigHelper.GetValue(package.AreaName, "RedisKeyPrefix", DefaultRedisKeyPrefix)!;
            YetaWF.Core.IO.Caching.LockProvider = new LockRedisProvider(configString, keyPrefix);
        } else if (LockProvider == LocalLockProvider) {
            YetaWF.Core.IO.Caching.LockProvider = new LockSingleProvider();
        } else {
            throw new InternalError($"Unsupported lock provider: {LockProvider}");
        }

        // PubSub provider
        PubSubProvider = WebConfigHelper.GetValue<string>(package.AreaName, "PubSubProvider")?.ToLower();
        if (PubSubProvider == RedisCacheProvider) {
            string configString = WebConfigHelper.GetValue(package.AreaName, "RedisPubSubConfig", DefaultRedisConfig)!;
            string keyPrefix = WebConfigHelper.GetValue(package.AreaName, "RedisPubSubPrefix", DefaultRedisPubSubPrefix)!;
            YetaWF.Core.IO.Caching.PubSubProvider = new PubSubRedisProvider(configString, keyPrefix);
        } else if (PubSubProvider == null) {
            YetaWF.Core.IO.Caching.PubSubProvider = new DefaultPubSubProvider();
        } else {
            throw new InternalError($"Unsupported pub/sub provider: {PubSubProvider}");
        }
    }
    /// <summary>
    /// Called when the first node of a multi-instance site is starting up.
    /// </summary>
    public async Task InitializeFirstNodeStartupAsync() {
        if (LockProvider == FileCacheProvider) {// TODO: This should be in a file data provider
            foreach (string file in await YetaWF.Core.IO.FileSystem.FileSystemProvider.GetFilesAsync(GetRootFolder())) {
                if (!file.EndsWith("++" + YetaWF.Core.Support.Startup.FirstNodeIndicator))
                    await YetaWF.Core.IO.FileSystem.FileSystemProvider.DeleteFileAsync(file);
            }
        }
    }
    private static string GetRootFolder() {
        Package package = YetaWF.Modules.Caching.AreaRegistration.CurrentPackage;
        return WebConfigHelper.GetValue(package.AreaName, "FileLockFolder", Path.Combine(YetaWFManager.DataFolder, package.AreaName, "__LOCKS"))!;
    }

    private class DefaultPubSubProvider : IPubSubProvider {
        public void Dispose() { }
        public ValueTask DisposeAsync() { return ValueTask.CompletedTask; }

        public Task PublishAsync(string channel, object message) {
            throw new InternalError("No PubSubProvider available");
        }
        public Task SubscribeAsync(string channel, Func<string, object, Task> callback) {
            throw new InternalError("No PubSubProvider available");
        }
        public Task UnsubscribeAsync(string channel) {
            throw new InternalError("No PubSubProvider available");
        }
    }
}
