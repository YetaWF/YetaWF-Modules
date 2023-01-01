/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Support;
using YetaWF.Modules.Caching.Startup;

namespace YetaWF.Modules.Caching.DataProvider {

    /// <summary>
    /// Redis locking provider.
    /// An instance of this class is instantiated during application startup and registers low-level data providers implementing locking using Redis.
    /// </summary>
    /// <remarks>
    /// Uses a Redis server for locking.</remarks>
    public class LockRedisProvider : ILockProvider, IInitializeApplicationStartupFirstNodeOnly {

        private static ConnectionMultiplexer Redis { get; set; } = null!;
        private static string KeyPrefix { get; set; } = null!;
        private static Guid Id { get; set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() { Dispose(true); }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">true to release the DisposableTracker reference count, false otherwise.</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                DisposableTracker.RemoveObject(this);
            }
        }

        /// <summary>
        /// Called when the first node of a multi-instance site is starting up.
        /// </summary>
        public async Task InitializeFirstNodeStartupAsync() {
            if (YetaWF.Modules.Caching.Startup.Application.LockProvider != YetaWF.Modules.Caching.Startup.Application.RedisCacheProvider) return;
            // this is the first node, so clear all data
            IDatabase db = Redis.GetDatabase();

            string keyPrefix = WebConfigHelper.GetValue(YetaWF.Modules.Caching.AreaRegistration.CurrentPackage.AreaName, "RedisKeyPrefix", Application.DefaultRedisKeyPrefix)!;
            System.Net.EndPoint endPoint = Redis.GetEndPoints().First();
            RedisKey[] keys = Redis.GetServer(endPoint).Keys(pattern: $"{keyPrefix}*").ToArray();
            //await db.ExecuteAsync("FLUSHALL");
            foreach (RedisKey key in keys) {
                if (!key.ToString().EndsWith(YetaWF.Core.Support.Startup.FirstNodeIndicator)) {// don't remove the current first-time startup lock
                    if (YetaWFManager.IsSync()) {
                        db.KeyDelete(key);
                    } else {
                        await db.KeyDeleteAsync(key);
                    }
                }
            }
        }

        // Implementation

        /// <summary>
        /// Constructor.
        /// </summary>
        public LockRedisProvider() { }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="configString">The Redis configuration string used to connect to the Redis server.</param>
        /// <param name="keyPrefix">The string used to prefix all keys used.</param>
        public LockRedisProvider(string configString, string keyPrefix) {
            Redis = ConnectionMultiplexer.Connect(configString);
            KeyPrefix = keyPrefix;
            Id = Guid.NewGuid();
            DisposableTracker.AddObject(this);
        }

        // API

        private class MultiLockObject : ILockObject {

            private string Key;
            private IDatabase Db;
            private static SemaphoreSlim localLock = new SemaphoreSlim(1, 1);// to protect local instance
            private bool LocalLocked = false;
            private bool Locked = false;

            public MultiLockObject(string key) {
                Db = Redis.GetDatabase();
                Key = KeyPrefix + key;
                DisposableTracker.AddObject(this);
            }
            internal async Task LockAsync() {
                if (Locked) throw new InternalError($"{nameof(LockAsync)} called while already holding a lock on {Key}");
                for ( ; !Locked; ) {
                    if (YetaWFManager.IsSync()) {
                        if (localLock.Wait(10))
                            LocalLocked = true;
                    } else {
                        await localLock.WaitAsync();
                        LocalLocked = true;
                    }
                    if (LocalLocked) {
                        if (YetaWFManager.IsSync()) {
                            if (Db.LockTake(Key, Id.ToString(), new TimeSpan(0, 10, 0)))
                                Locked = true;
                        } else {
                            if (await Db.LockTakeAsync(Key, Id.ToString(), new TimeSpan(0, 10, 0)))
                                Locked = true;
                        }
                        localLock.Release();
                        LocalLocked = false;
                        if (Locked)
                            break;
                    }
                    if (YetaWFManager.IsSync())
                        Thread.Sleep(new TimeSpan(0, 0, 0, 0, 25));// wait a while - this is bad, only works because "other" instance has lock
                    else
                        await Task.Delay(new TimeSpan(0, 0, 0, 0, 25));// wait a while
                }
            }
            public async Task UnlockAsync() {
                if (Locked) {
                    if (YetaWFManager.IsSync())
                        Db.LockRelease(Key, Id.ToString());
                    else
                        await Db.LockReleaseAsync(Key, Id.ToString());
                    Locked = false;
                }
            }

            public void Dispose() { Dispose(true); }
            protected virtual void Dispose(bool disposing) {
                if (disposing) {
                    DisposableTracker.RemoveObject(this);
                    YetaWFManager.Syncify(async () => // Only used if caller forgets to Unlock
                        await UnlockAsync()
                    );
                }
            }
        }

        /// <summary>
        /// Lock a key by name until disposed.
        /// </summary>
        public async Task<ILockObject> LockResourceAsync(string key) {
            // If we're running in a single instance, a simple lock by name is sufficient.
            // On multi instances, we need a locking implementation using a lock file. This is somewhat expensive.
            MultiLockObject lockObject = new MultiLockObject(key);
            await lockObject.LockAsync();
            return (ILockObject)lockObject;
        }
    }
}
