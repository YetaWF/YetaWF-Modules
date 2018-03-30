/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;

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
    /// Shared cache will only be retrieved to check if there is a newer cached object available. Once
    /// it is known that a new object is available the data is retrieved.
    /// </summary>
    public class StaticSharedCacheObjectDataProvider : DataProviderImpl, ICacheStaticObject {

        // Implementation

        public StaticSharedCacheObjectDataProvider() : base(0) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<string, SharedCacheVersion> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<string, SharedCacheVersion> CreateDataProvider() {
            Package package = YetaWF.Modules.Caching.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_SharedCache", Cacheable: false, Parms: new { NoLanguages = true });
        }

        private static Dictionary<string, StaticCacheObject> StaticObjects = new Dictionary<string, StaticCacheObject>();

        // API

        private string GetKey<TYPE>() {
            int identity = (YetaWFManager.HaveManager && Manager.HaveCurrentSite) ? Manager.CurrentSite.Identity : 0;
            return $"__static_{identity}/{typeof(TYPE).FullName}";
        }
        public async Task AddAsync<TYPE>(TYPE data) {
            string key = GetKey<TYPE>();
            await StringLocks.DoActionAsync(key, async () => {
                using (DataProviderTransaction trans = DataProvider.StartTransaction()) {
                    // save new version shared and locally
                    SharedCacheVersion sharedCacheObj = new SharedCacheVersion {
                        Created = DateTime.UtcNow,
                        Key = key,
                    };
                    await DataProvider.RemoveAsync(key);
                    await DataProvider.AddAsync(sharedCacheObj); // save shared cached version
                    await trans.CommitAsync();

                    StaticCacheObject cachedObj = new StaticCacheObject {
                        Key = key,
                        Value = data,
                        Created = DateTime.UtcNow,
                    };
                    StaticObjects.Remove(key);
                    StaticObjects.Add(key, cachedObj);
                }
            });
        }
        public async Task<GetObjectInfo<TYPE>> GetAsync<TYPE>(Func<Task<TYPE>> noDataCallback) {
            // get cached version
            GetObjectInfo<TYPE> retVal = null;
            string key = GetKey<TYPE>();
            await StringLocks.DoActionAsync(key, async () => {
                StaticCacheObject cachedObj;
                bool localValid = StaticObjects.TryGetValue(key, out cachedObj);
                if (!localValid) {
                    cachedObj = new StaticCacheObject {
                        Key = key,
                        Value = null,
                        Created = DateTime.MinValue,
                    };
                }
                // get shared cached version
                SharedCacheVersion sharedInfo = await DataProvider.GetAsync(key);
                if (sharedInfo != null) {
                    if (sharedInfo.Created != cachedObj.Created) {
                        // shared cached version is different, use callback to set data
                        TYPE data = await noDataCallback();
                        cachedObj = new StaticCacheObject {
                            Created = sharedInfo.Created,
                            Key = sharedInfo.Key,
                            Value = data,
                        };
                        StaticObjects.Remove(key);
                        StaticObjects.Add(key, cachedObj);
                        retVal = new GetObjectInfo<TYPE> {
                            Success = true,
                            Data = data,
                        };
                        return;
                    } else {
                        // shared version same as local version
                    }
                } else {
                    // there is no shared version
                }
                // return the local data 
                if (!localValid)
                    cachedObj.Value = await noDataCallback();
                retVal = new GetObjectInfo<TYPE> {
                    Success = true,
                    Data = (TYPE)cachedObj.Value,
                };
                return;
            });
            return retVal;
        }
        public async Task RemoveAsync<TYPE>() {
            // We're adding a new version
            await AddAsync(default(TYPE));
        }

        private class StaticLockObject : IDisposable, IStaticLockObject {

            private IDataProvider<string, SharedCacheVersion> dataProvider;
            private string key;

            public StaticLockObject(IDataProvider<string, SharedCacheVersion> dataProvider, string key) {
                this.dataProvider = dataProvider;
                this.key = key;
                DisposableTracker.AddObject(this);
            }

            public async Task UnlockAsync() {
                await dataProvider.RemoveAsync(key);
                dataProvider = null;
            }

            public void Dispose() { Dispose(true); }
            protected virtual void Dispose(bool disposing) {
                if (disposing) {
                    if (dataProvider != null) {
                        DisposableTracker.RemoveObject(this);
                        YetaWFManager.Syncify(async () => // Only used if caller forgets to Unlock
                            await dataProvider.RemoveAsync(key)
                        );
                        dataProvider = null;
                    }
                }
            }
        }
        /// <summary>
        /// Lock a static shared object.
        /// </summary>
        /// <returns>This does not support reentrancy.</returns>
        public async Task<IStaticLockObject> LockAsync<TYPE>() {
            string key = "__LOCK" + GetKey<TYPE>();
            SharedCacheVersion sharedObj = new SharedCacheVersion {
                Created = DateTime.UtcNow,
                Key = key,
            };
            for (;;) {
                if (await DataProvider.AddAsync(sharedObj))
                    break;// we own the lock
                await Task.Delay(new TimeSpan(0, 0, 0, 50));// wait a while
            }
            return new StaticLockObject(DataProvider, key);
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
