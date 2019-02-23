﻿/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using System.IO;
using System.Threading.Tasks;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Caching.Controllers;
using YetaWF.Modules.Caching.DataProvider;

namespace YetaWF.Modules.Caching.Startup {

    public class Application : IInitializeApplicationStartup, IInitializeApplicationStartupFirstNodeOnly {

        public const string Distributed = "Distributed";
        public const string DefaultRedisConfig = "localhost:6379";

        public static string LockProvider { get; private set; }
        public static string CacheProvider { get; private set; }
        public const string SQLCacheProvider = "sql";
        public const string RedisCacheProvider = "redis";

        // Using a Redis server:
        // Start a Redis server using "docker run --name redis -d -p 6379:6379 redis".
        // Run Redis CLI: docker run -it --link redis:redis --rm redis redis-cli -h redis -p 6379
        // Show all keys: KEYS *
        // Clear DB: FLUSHALL
        public async Task InitializeApplicationStartupAsync() {

            Package package = YetaWF.Modules.Caching.Controllers.AreaRegistration.CurrentPackage;

            // permanently created dataproviders (never disposed)
            bool distributed = WebConfigHelper.GetValue<bool>(AreaRegistration.CurrentPackage.AreaName, Distributed, false);

            if (distributed) {
                YetaWF.Core.Support.Startup.MultiInstance = true;
                // distributed caching uses local and shared cache
                YetaWF.Core.IO.Caching.GetLocalCacheProvider = LocalCacheObjectDataProvider.GetProvider;

                CacheProvider = WebConfigHelper.GetValue(package.AreaName, "CacheProvider", "redis").ToLower();
                if (CacheProvider == RedisCacheProvider) {
                    string configString = WebConfigHelper.GetValue(package.AreaName, "RedisCacheConfig", DefaultRedisConfig);
                    await SharedCacheObjectRedisDataProvider.InitAsync(configString);
                    await StaticObjectMultiRedisDataProvider.InitAsync(configString);
                    YetaWF.Core.IO.Caching.GetSharedCacheProvider = SharedCacheObjectRedisDataProvider.GetProvider;
                    YetaWF.Core.IO.Caching.GetStaticCacheProvider = StaticObjectMultiRedisDataProvider.GetProvider;
                } else if (CacheProvider == SQLCacheProvider) {
                    YetaWF.Core.IO.Caching.GetSharedCacheProvider = SharedCacheObjectSQLDataProvider.GetProvider;
                    YetaWF.Core.IO.Caching.GetStaticCacheProvider = StaticObjectMultiSQLDataProvider.GetProvider;
                } else {
                    throw new InternalError($"Unsupported cache provider: {LockProvider}");
                }
                // Lock provider
                LockProvider = WebConfigHelper.GetValue(package.AreaName, "LockProvider", "file").ToLower();
                if (LockProvider == "file") {
                    // create the lock folder if it doesn't exists yet
                    await YetaWF.Core.IO.FileSystem.FileSystemProvider.CreateDirectoryAsync(GetRootFolder());
                    string rootFolder = WebConfigHelper.GetValue(package.AreaName, "LockFolder", Path.Combine(YetaWFManager.DataFolder, package.AreaName, "__LOCKS"));
                    YetaWF.Core.IO.Caching.LockProvider = new LockFileProvider(rootFolder);
                } else if (LockProvider == RedisCacheProvider) {
                    string configString = WebConfigHelper.GetValue(package.AreaName, "RedisLockConfig", DefaultRedisConfig);
                    YetaWF.Core.IO.Caching.LockProvider = new LockRedisProvider(configString);
                } else {
                    throw new InternalError($"Unsupported lock provider: {LockProvider}");
                }
            } else {
                YetaWF.Core.Support.Startup.MultiInstance = false;
                // non-distributed caching uses local cache only
                YetaWF.Core.IO.Caching.GetLocalCacheProvider = LocalCacheObjectDataProvider.GetProvider;
                YetaWF.Core.IO.Caching.GetSharedCacheProvider = LocalCacheObjectDataProvider.GetProvider;
                YetaWF.Core.IO.Caching.GetStaticCacheProvider = StaticObjectSingleDataProvider.GetProvider;
                YetaWF.Core.IO.Caching.LockProvider = new LockSingleProvider();
            }
        }
        public async Task InitializeFirstNodeStartupAsync() {
            if (LockProvider == "file") {
                foreach (string file in await YetaWF.Core.IO.FileSystem.FileSystemProvider.GetFilesAsync(GetRootFolder())) {
                    if (!file.EndsWith("++" + YetaWF.Core.Support.Startup.FirstNodeIndicator))
                        await YetaWF.Core.IO.FileSystem.FileSystemProvider.DeleteFileAsync(file);
                }
            }
        }
        private static string GetRootFolder() {
            Package package = YetaWF.Modules.Caching.Controllers.AreaRegistration.CurrentPackage;
            return WebConfigHelper.GetValue(package.AreaName, "LockFolder", Path.Combine(YetaWFManager.DataFolder, package.AreaName, "__LOCKS"));
        }
    }
}
