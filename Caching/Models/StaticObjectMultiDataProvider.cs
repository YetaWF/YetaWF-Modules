/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Core.Support.Serializers;

namespace YetaWF.Modules.Caching.DataProvider {

    /// <summary>
    /// The object as persisted in shared cache.
    /// </summary>
    public class StaticCacheObject {

        public string Key { get; set; }
        public DateTime Created { get; set; }
        public object Value { get; set; }

        public StaticCacheObject() { }
    }

    /// <summary>
    /// A shared cache implementation backed by memory to improve performance.
    /// This is intended as a way to preserve and share "static" in-memory data across instances.
    /// Shared cache will only be retrieved to check if there is a newer cached object available. Once
    /// it is known that a new object is available the data is retrieved.
    /// This is equivalent to StaticObjectSingleDataProvider on a single-instance site.
    /// </summary>
    public class StaticObjectMultiDataProvider : DataProviderImpl, ICacheStaticObject {

        // Implementation

        public StaticObjectMultiDataProvider() : base(0) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<string, SharedCacheObject> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<string, SharedCacheObject> CreateDataProvider() {
            Package package = YetaWF.Modules.Caching.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_SharedCache", Cacheable: false, Parms: new { NoLanguages = true });
        }

        private static Dictionary<string, StaticCacheObject> StaticObjects = new Dictionary<string, StaticCacheObject>();

        // API

        private string GetKey(string key) {
            return $"__static__{key}";
        }
        public async Task AddAsync<TYPE>(string key, TYPE data) {
            key = GetKey(key);
            await StringLocks.DoActionAsync(key, async () => {
                using (DataProviderTransaction trans = DataProvider.StartTransaction()) {
                    // save new version shared and locally
                    SharedCacheObject sharedCacheObj = new SharedCacheObject {
                        Created = DateTime.UtcNow,
                        Key = key,
                        Value = new GeneralFormatter().Serialize(data),
                    };
                    await DataProvider.RemoveAsync(key);
                    await DataProvider.AddAsync(sharedCacheObj); // save shared cached version
                    await trans.CommitAsync();

                    StaticCacheObject cachedObj = new StaticCacheObject {
                        Key = key,
                        Value = data,
                        Created = sharedCacheObj.Created,
                    };
                    StaticObjects.Remove(key);
                    StaticObjects.Add(key, cachedObj);
                }
            });
        }
        public async Task<TYPE> GetAsync<TYPE>(string key, Func<Task<TYPE>> noDataCallback = null) {
            // get cached version
            TYPE data = default(TYPE);
            key = GetKey(key);
            await StringLocks.DoActionAsync(key, async () => {
                StaticCacheObject cachedObj;
                bool localValid = StaticObjects.TryGetValue(key, out cachedObj);
                if (!localValid) {
                    cachedObj = new StaticCacheObject {
                        Key = key,
                        Value = data,
                        Created = DateTime.MinValue,
                    };
                }
                // get shared cached version
                SharedCacheVersion sharedInfo = await SharedCacheVersionDataProvider.SharedCacheVersionDP.GetVersionAsync(key);
                if (sharedInfo != null) {
                    if (sharedInfo.Created != cachedObj.Created) {
                        // shared cached version is different, use callback to set data
                        if (noDataCallback == null) {
                            // shared cached version is different, retrieve and save locally
                            SharedCacheObject sharedCacheObj = await DataProvider.GetAsync(key);
                            if (sharedCacheObj == null) { 
                                // this shouldn't happen, we just got the shared version
                            } else {
                                data = (TYPE)new GeneralFormatter().Deserialize(sharedCacheObj.Value);
                                sharedInfo = sharedCacheObj;
                            }
                        } else {
                            // if there is a data callback, the caller provides all data (nothing, except version, is saved in shared cache)
                            data = await noDataCallback();
                        }
                        cachedObj = new StaticCacheObject {
                            Created = sharedInfo.Created,
                            Key = sharedInfo.Key,
                            Value = data,
                        };
                        StaticObjects.Remove(key);
                        StaticObjects.Add(key, cachedObj);
                        return;
                    } else {
                        // shared version same as local version
                    }
                } else {
                    // there is no shared version
                }
                // return the local data 
                if (!localValid) {
                    if (noDataCallback != null)
                        cachedObj.Value = await noDataCallback();
                }
                data = (TYPE)cachedObj.Value;
            });
            return data;
        }
        public async Task RemoveAsync<TYPE>(string key) {
            // We're adding a new version
            await AddAsync(key, default(TYPE));
        }

        private class StaticLockObject : IDisposable, IStaticLockObject {

            private IDataProvider<string, SharedCacheVersion> DataProvider;
            private string Key;
            private static SemaphoreSlim localLock = new SemaphoreSlim(1, 1);// to protect local instance
            private bool LocalLocked = false;
            private bool Locked = false;

            public StaticLockObject(IDataProvider<string, SharedCacheVersion> dataProvider, string key) {
                this.DataProvider = dataProvider;
                this.Key = key;
                DisposableTracker.AddObject(this);
            }
            internal async Task LockAsync() {
                if (Locked) throw new InternalError($"{nameof(LockAsync)} called while already holding a lock on {Key}");
                for (; !Locked;) {
                    if (YetaWFManager.IsSync()) {
                        if (localLock.Wait(10))
                            LocalLocked = true;
                    } else {
                        await localLock.WaitAsync();
                        LocalLocked = true;
                    }
                    if (LocalLocked) {
                        SharedCacheVersion sharedObj = new SharedCacheVersion {
                            Created = DateTime.UtcNow,
                            Key = Key,
                        };
                        try {
                            if (await DataProvider.AddAsync(sharedObj)) {
                                // we own the lock
                                Locked = true;
                            }
                        } catch (Exception) {
                            throw;
                        } finally {
                            localLock.Release();
                            LocalLocked = false;
                        }
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
                    await DataProvider.RemoveAsync(Key);
                    DataProvider = null;
                    Locked = false;
                }
            }

            public void Dispose() { Dispose(true); }
            protected virtual void Dispose(bool disposing) {
                if (LocalLocked)
                    throw new InternalError("Disposing with local lock present");
                if (disposing) {
                    if (Locked) {
                        DisposableTracker.RemoveObject(this);
                        YetaWFManager.Syncify(async () => // Only used if caller forgets to Unlock
                            await UnlockAsync()
                        );
                        DataProvider = null;
                    }
                }
            }
        }
        /// <summary>
        /// Lock a static shared object.
        /// </summary>
        public async Task<IStaticLockObject> LockAsync<TYPE>(string key) {
            StaticLockObject staticLock = new StaticLockObject(SharedCacheVersionDataProvider.SharedCacheVersionDP.GetDataProvider(), "__LOCK" + GetKey(key));
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

        // IInstallableModel

        public new Task<DataProviderExportChunk> ExportChunkAsync(int chunk, SerializableList<SerializableFile> fileList) {
            return Task.FromResult(new DataProviderExportChunk());
        }
        public new Task ImportChunkAsync(int chunk, SerializableList<SerializableFile> fileList, object obj) {
            return Task.CompletedTask;
        }
    }
}
