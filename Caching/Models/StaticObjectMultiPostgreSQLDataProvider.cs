/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support.Serializers;

namespace YetaWF.Modules.Caching.DataProvider {

    /// <summary>
    /// A shared cache implementation backed by memory to improve performance.
    /// This is intended as a way to preserve and share "static" in-memory data across instances.
    /// Shared cache will only be retrieved to check if there is a newer cached object available. Once
    /// it is known that a new object is available the data is retrieved.
    /// This is equivalent to StaticObjectSingleDataProvider on a single-instance site.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    internal class StaticObjectMultiPostgreSQLDataProvider : DataProviderImpl, ICacheDataProvider {

        public static ICacheDataProvider GetProvider() {
            return new StaticObjectMultiPostgreSQLDataProvider();
        }

        // Implementation

        public StaticObjectMultiPostgreSQLDataProvider() : base(0) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<string, SharedCacheObject> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<string, SharedCacheObject> CreateDataProvider() {
            Package package = YetaWF.Modules.Caching.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_SharedCache", Cacheable: false, Parms: new { NoLanguages = true });
        }

        private static Dictionary<string, StaticCacheObject> StaticObjects = new Dictionary<string, StaticCacheObject>();
        private static object _lockObject = new object();

        // API

        private string GetKey(string key) {
            return $"__static__{key}";
        }
        /// <summary>
        /// Add a shared object.
        /// </summary>
        /// <remarks>This requires an active Lock using a lock provider.</remarks>
        public async Task AddAsync<TYPE>(string key, TYPE data) {
            key = GetKey(key);
            // save new version shared and locally
            SharedCacheObject sharedCacheObj = new SharedCacheObject {
                Created = DateTime.UtcNow,
                Key = key,
                Value = new GeneralFormatter().Serialize(data),
            };
            await DataProvider.RemoveAsync(key);
            await DataProvider.AddAsync(sharedCacheObj); // save shared cached version

            StaticCacheObject cachedObj = new StaticCacheObject {
                Key = key,
                Value = data,
                Created = sharedCacheObj.Created,
            };
            lock (_lockObject) { // used to protect StaticObjects - local only
                StaticObjects.Remove(key);
                StaticObjects.Add(key, cachedObj);
            }
        }
        public async Task<GetObjectInfo<TYPE>> GetAsync<TYPE>(string key) {
            // get cached version
            TYPE data = default(TYPE);
            key = GetKey(key);

            StaticCacheObject cachedObj;
            bool localValid;
            lock (_lockObject) { // used to protect StaticObjects - local only
                localValid = StaticObjects.TryGetValue(key, out cachedObj);
            }
            if (!localValid) {
                cachedObj = new StaticCacheObject {
                    Key = key,
                    Value = default(TYPE),
                    Created = DateTime.MinValue,
                };
            } else {
                data = (TYPE)cachedObj.Value;
            }
            // get shared cached version
            SharedCacheVersion sharedInfo = await SharedCacheVersionPostgreSQLDataProvider.SharedCacheVersionDP.GetVersionAsync(key);
            if (sharedInfo != null) {
                if (sharedInfo.Created != cachedObj.Created) {
                    // shared cached version is different, retrieve and save locally
                    SharedCacheObject sharedCacheObj = await DataProvider.GetAsync(key);
                    if (sharedCacheObj == null) {
                        // this shouldn't happen, we just got the shared version
                    } else {
                        data = new GeneralFormatter().Deserialize<TYPE>(sharedCacheObj.Value);
                        sharedInfo = sharedCacheObj;
                    }
                    cachedObj = new StaticCacheObject {
                        Created = sharedInfo.Created,
                        Key = sharedInfo.Key,
                        Value = data,
                    };
                    localValid = true;
                    lock (_lockObject) { // used to protect StaticObjects - local only
                        StaticObjects.Remove(key);
                        StaticObjects.Add(key, cachedObj);
                    }
                } else {
                    // shared version same as local version
                }
                return new GetObjectInfo<TYPE> {
                    Success = true,
                    Data = data,
                };
            } else {
                // there is no shared version
                // no shared cache
                if (!localValid) {
                    return new GetObjectInfo<TYPE> {
                        Success = false
                    };
                } else {
                    return new GetObjectInfo<TYPE> {
                        Success = true,
                        Data = data,
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
