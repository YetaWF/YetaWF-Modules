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

    // 
    public class SharedCacheVersion {

        public const int MaxKey = 100;

        [Data_Identity]
        public int Identity { get; set; }
        [Data_PrimaryKey, StringLength(MaxKey)]
        public string Key { get; set; }
        public DateTime Created { get; set; }

        public SharedCacheVersion() { }
    }

    public class SharedCacheVersionDataProvider : DataProviderImpl, IInitializeApplicationStartup {

        public static SharedCacheVersionDataProvider SharedCacheVersionDP { get; private set; }

        // Startup

        public Task InitializeApplicationStartupAsync(bool firstNode) {
            SharedCacheVersionDP = this;
            return Task.CompletedTask;
        }

        // Implementation

        public SharedCacheVersionDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public SharedCacheVersionDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProviderIdentity<string, object, SharedCacheVersion> DataProvider { get { return GetDataProvider(); } }

        private IDataProviderIdentity<string, object, SharedCacheVersion> CreateDataProvider() {
            Package package = YetaWF.Modules.Caching.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_SharedCache", Cacheable: false, Parms: new { NoLanguages = true });
        }

        // API

        public Task<SharedCacheVersion> GetVersionAsync(string key) {
            return DataProvider.GetAsync(key, null);
        }
    }

    /// <summary>
    /// The object as persisted in shared cache.
    /// </summary>
    public class SharedCacheObject : SharedCacheVersion {

        [Data_Binary]
        public byte[] Value { get; set; }

        public SharedCacheObject() { }
    }
    /// <summary>
    /// The object as persisted in local cache, representing a local copy of the object in shared cache
    /// </summary>
    public class LocalSharedCacheObject {

        public string Key { get; set; }
        public byte[] Value { get; set; }
        public DateTime Created { get; set; }

        public LocalSharedCacheObject() { }
    }

    public class SharedCacheObjectDataProvider : DataProviderImpl, ICacheObject, IInstallableModel, IInitializeApplicationStartup {

        // Startup

        public Task InitializeApplicationStartupAsync(bool firstNode) {
            YetaWF.Core.IO.Caching.SharedCacheProvider = this;
            return Task.CompletedTask;
        }

        // Implementation

        public SharedCacheObjectDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }

        private IDataProviderIdentity<string, object, SharedCacheObject> DataProvider { get { return GetDataProvider(); } }

        private IDataProviderIdentity<string, object, SharedCacheObject> CreateDataProvider() {
            Package package = YetaWF.Modules.Caching.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_SharedCache", Cacheable: false, Parms: new { NoLanguages = true });
        }

        // API

        public async Task AddAsync<TYPE>(string key, TYPE data) {
            using (DataProviderTransaction trans = DataProvider.StartTransaction()) {
                // save new version shared and locally
                byte[] cacheData = new GeneralFormatter().Serialize(data);
                SharedCacheObject sharedCacheObj = new SharedCacheObject {
                    Created = DateTime.UtcNow,
                    Key = key,
                    Value = cacheData,
                };
                await DataProvider.RemoveAsync(key, null);
                await DataProvider.AddAsync(sharedCacheObj); // save shared cached version
                LocalSharedCacheObject localCacheObj = new LocalSharedCacheObject {
                    Created = sharedCacheObj.Created,
                    Key = sharedCacheObj.Key,
                    Value = sharedCacheObj.Value,
                };
                await YetaWF.Core.IO.Caching.LocalCacheProvider.AddAsync(key, localCacheObj); // save locally cached version
                await trans.CommitAsync();
            }
        }
        public async Task<GetObjectInfo<TYPE>> GetAsync<TYPE>(string key) {
            // get locally cached version
            GetObjectInfo<LocalSharedCacheObject> localInfo = await YetaWF.Core.IO.Caching.LocalCacheProvider.GetAsync<LocalSharedCacheObject>(key);
            if (!localInfo.Success) {
                // no locally cached data, create one
                localInfo = new GetObjectInfo<LocalSharedCacheObject> {
                    Data = new LocalSharedCacheObject {
                        Created = DateTime.MinValue,
                        Key = key,
                        Value = null,
                    },
                    Success = true,
                };
            }
            // get shared cached version
            SharedCacheVersion sharedInfo = await SharedCacheVersionDataProvider.SharedCacheVersionDP.GetVersionAsync(key);
            if (sharedInfo != null) {
                if (sharedInfo.Created != localInfo.Data.Created) {
                    // shared cached version is different, retrieve and save locally
                    SharedCacheObject sharedCacheObj = await DataProvider.GetAsync(key, null);
                    if (sharedCacheObj == null) { // this shouldn't happen, we just got the shared version
                        // return the local data instead
                    } else {
                        LocalSharedCacheObject localCacheObj = new LocalSharedCacheObject {
                            Created = sharedCacheObj.Created,
                            Key = sharedCacheObj.Key,
                            Value = sharedCacheObj.Value,
                        };
                        await YetaWF.Core.IO.Caching.LocalCacheProvider.AddAsync(key, localCacheObj); // save as locally cached version
                        return new GetObjectInfo<TYPE> {
                            Success = true,
                            Data = (TYPE) new GeneralFormatter().Deserialize(sharedCacheObj.Value),
                        };
                    }
                } else {
                    // shared version same as local version
                }
            } else {
                // there is no shared version
            }
            // return the local data 
            return new GetObjectInfo<TYPE> {
                Success = true,
                Data = (TYPE) new GeneralFormatter().Deserialize(localInfo.Data.Value),
            };
        }

        public async Task RemoveAsync(string key) {
            // We're adding a new version
            await AddAsync(key, default(Type));
        }

        // API for Module

        /// <summary>
        /// Retrieve the complete cached object including version information.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<SharedCacheObject> GetItemAsync(string key) {
            return DataProvider.GetAsync(key, null);
        }
        public Task<DataProviderGetRecords<SharedCacheObject>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            return DataProvider.GetRecordsAsync(skip, take, sort, filters);
        }
        public Task<int> RemoveItemsAsync(List<DataProviderFilterInfo> filters) {
            return DataProvider.RemoveRecordsAsync(filters);
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
