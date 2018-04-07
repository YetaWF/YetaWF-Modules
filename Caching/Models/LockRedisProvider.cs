/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/FileSystem#License */

using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Caching.DataProvider {

    /// <summary>
    /// Redis locking provider.
    /// </summary>
    /// <remarks>
    /// Uses a Redis server for locking.</remarks>
    public class LockRedisProvider : ILockProvider, IInitializeApplicationStartupFirstNodeOnly {

        private static ConnectionMultiplexer Redis { get; set; }
        private static Guid Id { get; set; }

        public void Dispose() { Dispose(true); }
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                DisposableTracker.RemoveObject(this);
            }
        }

        public async Task InitializeFirstNodeStartupAsync() {
            if (YetaWF.Modules.Caching.Startup.Application.LockProvider != YetaWF.Modules.Caching.Startup.Application.ReditCacheProvider) return;
            // this is the first node, so clear all data
            IDatabase db = Redis.GetDatabase();
            if (YetaWFManager.IsSync()) {
                db.Execute("FLUSHALL");
            } else {
                await db.ExecuteAsync("FLUSHALL");
            }
        }

        // Implementation

        public LockRedisProvider() { }
        public LockRedisProvider(string configString) {
            Redis = ConnectionMultiplexer.Connect(configString);
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
                Key = key;
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
