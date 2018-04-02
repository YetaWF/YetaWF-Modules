/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Caching.DataProvider {

    /// <summary>
    /// A shared cache implementation in-memory ONLY for a single instance. (Really not shared).
    /// This is intended as a way to preserve "static" in-memory data. This is equivalent to StaticObjectMultiDataProvider on a multi-instance site.
    /// </summary>
    public class StaticObjectSingleDataProvider : ICacheStaticDataProvider {

        public static ICacheStaticDataProvider GetLocalCacheProvider() {
            return new StaticObjectMultiDataProvider();
        }

        protected YetaWFManager Manager { get { return YetaWFManager.Manager; } }
        protected bool HaveManager { get { return YetaWFManager.HaveManager; } }

        private static Dictionary<string, object> StaticObjects = new Dictionary<string, object>();
        private static object _lockObject = new object();

        protected StaticObjectSingleDataProvider() {
            DisposableTracker.AddObject(this);
        }
        public void Dispose() {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                DisposableTracker.RemoveObject(this);
            }
        }
        //~StaticObjectSingleDataProvider() { Dispose(false); }

        // API

        private string GetKey(string key) {
            return $"__static__{key}";
        }
        public Task AddAsync<TYPE>(string key, TYPE data) {
            key = GetKey(key);
            lock (_lockObject) { // used to protect StaticObjects - local only
                StaticObjects.Remove(key);
                StaticObjects.Add(key, data);
            }
            return Task.CompletedTask;
        }
        public async Task<TYPE> GetAsync<TYPE>(string key, Func<Task<TYPE>> noDataCallback = null) {
            // get cached version
            TYPE data = default(TYPE);
            key = GetKey(key);
            object obj;
            if (!StaticObjects.TryGetValue(key, out obj)) {
                if (noDataCallback != null) {
                    obj = await noDataCallback();
                    data = (TYPE)obj;
                }
            }
            return data;
        }
        public Task RemoveAsync<TYPE>(string key) {
            key = GetKey(key);
            lock (_lockObject) { // used to protect StaticObjects - local only
                StaticObjects.Remove(key);
            }
            return Task.CompletedTask;
        }

        private class StaticLockObject : IDisposable, IStaticLockObject {

            private static List<string> LockKeys = new List<string>();
            private static SemaphoreSlim localLock = new SemaphoreSlim(1, 1);// to protect local instance
            private string Key;
            private bool LocalLocked = false;
            private bool Locked = false;

            public StaticLockObject(string key) {
                Key = key;
                DisposableTracker.AddObject(this);
            }
            internal async Task LockAsync() {
                if (Locked) throw new InternalError($"{nameof(LockAsync)} called while already holding a lock for key {Key}");
                for (; !Locked;) {
                    if (YetaWFManager.IsSync()) {
                        if (localLock.Wait(10))
                            LocalLocked = true;
                    } else {
                        await localLock.WaitAsync();
                        LocalLocked = true;
                    }
                    if (LocalLocked) {
                        if (!LockKeys.Contains(Key)) {
                            LockKeys.Add(Key);
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
                for (; Locked;) {
                    if (YetaWFManager.IsSync()) {
                        if (localLock.Wait(10))
                            LocalLocked = true;
                    } else {
                        await localLock.WaitAsync();
                        LocalLocked = true;
                    }
                    if (LocalLocked) {
                        LockKeys.Remove(Key);
                        localLock.Release();
                        LocalLocked = false;
                        Locked = false;
                        break;
                    }
                    if (YetaWFManager.IsSync())
                        Thread.Sleep(new TimeSpan(0, 0, 0, 0, 25));// wait a while - this is bad, only works because "other" instance has lock
                    else
                        await Task.Delay(new TimeSpan(0, 0, 0, 0, 25));// wait a while
                }
            }
            public void Dispose() { Dispose(true); }
            protected virtual void Dispose(bool disposing) {
                if (disposing) {
                    YetaWFManager.Syncify(async () => // Only used if caller forgets to Unlock
                        await UnlockAsync()
                    );
                }
            }
        }
        /// <summary>
        /// Lock a static shared object.
        /// </summary>
        public async Task<IStaticLockObject> LockAsync<TYPE>(string key) {
            StaticLockObject staticLock = new StaticLockObject(key);
            await staticLock.LockAsync();
            return staticLock;
        }

        //$$// API for Module

        ///// <summary>
        ///// Retrieve the complete cached object including version information.
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //public Task<StaticCacheObject> GetItemAsync(string key) {
        //    return DataProvider.GetAsync(key, null);
        //}
        //public Task<DataProviderGetRecords<StaticCacheObject>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
        //    return DataProvider.GetRecordsAsync(skip, take, sort, filters);
        //}
        //public Task<int> RemoveItemsAsync(List<DataProviderFilterInfo> filters) {
        //    return DataProvider.RemoveRecordsAsync(filters);
        //}

    }
}
