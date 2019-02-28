/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Core.Support.Serializers;

namespace YetaWF.Modules.Caching.DataProvider {

    internal class SharedCacheVersion {

        public const int MaxKey = 200;

        [Data_PrimaryKey, Data_Index, StringLength(MaxKey)]
        public string Key { get; set; }
        public DateTime Created { get; set; }

        public SharedCacheVersion() { }
    }

    internal class SharedCacheVersionSQLDataProvider : DataProviderImpl, IInitializeApplicationStartup, IInitializeApplicationStartupFirstNodeOnly {

        public static SharedCacheVersionSQLDataProvider SharedCacheVersionDP { get; private set; }

        // Startup

        public Task InitializeApplicationStartupAsync() {
            SharedCacheVersionDP = this;
            return Task.CompletedTask;
        }
        public async Task InitializeFirstNodeStartupAsync() {
            if (YetaWF.Modules.Caching.Startup.Application.CacheProvider == YetaWF.Modules.Caching.Startup.Application.SQLCacheProvider)
                await DataProvider.RemoveRecordsAsync(null);// remove all records
        }

        // Implementation

        public SharedCacheVersionSQLDataProvider() : base(0) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<string, SharedCacheVersion> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<string, SharedCacheVersion> CreateDataProvider() {
            Package package = YetaWF.Modules.Caching.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_SharedCache", Cacheable: false, Parms: new { NoLanguages = true });
        }

        // API

        public Task<SharedCacheVersion> GetVersionAsync(string key) {
            return DataProvider.GetAsync(key);
        }
    }

    /// <summary>
    /// The object as persisted in shared cache.
    /// </summary>
    internal class SharedCacheObject : SharedCacheVersion {

        [Data_Binary]
        public byte[] Value { get; set; }

        public SharedCacheObject() { }
    }
    /// <summary>
    /// The object as persisted in local cache, representing a local copy of the object in shared cache
    /// </summary>
    internal class LocalSharedCacheObject {

        public string Key { get; set; }
        public byte[] Value { get; set; }
        public DateTime Created { get; set; }

        public LocalSharedCacheObject() { }
    }

    /// <summary>
    /// A shared cache implementation backed by local cache to improve performance.
    /// Shared cache will only be retrieved to check if there is a newer cached object available. Once
    /// it is known that a new object is available, the data is retrieved.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    internal class SharedCacheObjectSQLDataProvider : DataProviderImpl, ICacheDataProvider, IInstallableModel {

        public static ICacheDataProvider GetProvider() {
            return new SharedCacheObjectSQLDataProvider();
        }

        // Implementation

        public SharedCacheObjectSQLDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<string, SharedCacheObject> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<string, SharedCacheObject> CreateDataProvider() {
            Package package = YetaWF.Modules.Caching.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_SharedCache", Cacheable: false, Parms: new { NoLanguages = true });
        }

        // API

        public async Task AddAsync<TYPE>(string key, TYPE data) {
            // save new version shared and locally
            byte[] cacheData = new GeneralFormatter().Serialize(data);
            SharedCacheObject sharedCacheObj = new SharedCacheObject {
                Created = DateTime.UtcNow,
                Key = key,
                Value = cacheData,
            };
            await DataProvider.RemoveAsync(key);
            await DataProvider.AddAsync(sharedCacheObj); // save shared cached version
            LocalSharedCacheObject localCacheObj = new LocalSharedCacheObject {
                Created = sharedCacheObj.Created,
                Key = sharedCacheObj.Key,
                Value = sharedCacheObj.Value,
            };
            using (ICacheDataProvider localCacheDP = YetaWF.Core.IO.Caching.GetLocalCacheProvider()) {
                await localCacheDP.AddAsync(key, localCacheObj); // save locally cached version
            }
        }
        public async Task<GetObjectInfo<TYPE>> GetAsync<TYPE>(string key) {
            // get locally cached version
            GetObjectInfo<LocalSharedCacheObject> localInfo;
            using (ICacheDataProvider localCacheDP = YetaWF.Core.IO.Caching.GetLocalCacheProvider()) {
                localInfo = await localCacheDP.GetAsync<LocalSharedCacheObject>(key);
                if (!localInfo.Success) {
                    // no locally cached data
                    localInfo = new GetObjectInfo<LocalSharedCacheObject> {
                        Data = new LocalSharedCacheObject {
                            Created = DateTime.MinValue,
                            Key = key,
                            Value = null,
                        },
                        Success = false,
                    };
                }
                // get shared cached version
                SharedCacheVersion sharedInfo = await SharedCacheVersionSQLDataProvider.SharedCacheVersionDP.GetVersionAsync(key);
                if (sharedInfo != null) {
                    if (sharedInfo.Created != localInfo.Data.Created) {
                        // shared cached version is different, retrieve and save locally
                        SharedCacheObject sharedCacheObj = await DataProvider.GetAsync(key);
                        if (sharedCacheObj == null) { // this shouldn't happen, we just got the shared version
                                                      // return the local data instead
                        } else {
                            LocalSharedCacheObject localCacheObj = new LocalSharedCacheObject {
                                Created = sharedCacheObj.Created,
                                Key = sharedCacheObj.Key,
                                Value = sharedCacheObj.Value,
                            };
                            await localCacheDP.AddAsync(key, localCacheObj); // save as locally cached version
                            return new GetObjectInfo<TYPE> {
                                Success = true,
                                Data = new GeneralFormatter().Deserialize<TYPE>(sharedCacheObj.Value),
                            };
                        }
                    } else {
                        // shared version same as local version
                    }
                } else {
                    // there is no shared version
                }
                // return the local data
                if (localInfo.Success) {
                    return new GetObjectInfo<TYPE> {
                        Success = true,
                        Data = new GeneralFormatter().Deserialize<TYPE>(localInfo.Data.Value),
                    };
                } else {
                    return new GetObjectInfo<TYPE> {
                        Success = false,
                    };
                }
            }
        }
        public async Task RemoveAsync<TYPE>(string key) {
            // We're adding a new version
            await AddAsync(key, default(TYPE));
        }

        // IInstallableModel

        public new Task<DataProviderExportChunk> ExportChunkAsync(int chunk, SerializableList<SerializableFile> fileList) {
            return Task.FromResult(new DataProviderExportChunk());
        }
        public new Task ImportChunkAsync(int chunk, SerializableList<SerializableFile> fileList, object obj) {
            return Task.CompletedTask;
        }
    }
}
