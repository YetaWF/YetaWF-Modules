/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Support;
using YetaWF.Core.Support.Serializers;

namespace YetaWF.Modules.Caching.DataProvider {

    /// <summary>
    /// A shared cache implementation backed by local cache to improve performance.
    /// Shared cache will only be retrieved to check if there is a newer cached object available. Once
    /// it is known that a new object is available, the data is retrieved.
    /// </summary>
    internal class SharedCacheObjectRedisDataProvider : ICacheDataProvider, IDisposable {

        public static ICacheDataProvider GetProvider() {
            return new SharedCacheObjectRedisDataProvider();
        }

        // Implementation

        private static ConnectionMultiplexer Redis { get; set; }
        private static Guid Id { get; set; }

        public SharedCacheObjectRedisDataProvider() {
            DisposableTracker.AddObject(this);
        }
        public void Dispose() { Dispose(true); }
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                DisposableTracker.RemoveObject(this);
            }
        }

        public static Task InitAsync(string configString) {
            Redis = ConnectionMultiplexer.Connect(configString);
            Id = Guid.NewGuid();
            return Task.CompletedTask;
        }

        private RedisKey GetVersionKey(string key) {
            return "__Version__" + key;
        }
        private RedisKey GetDataKey(string key) {
            return "__Data__" + key;
        }

        // API

        public async Task AddAsync<TYPE>(string key, TYPE data) {
            // save new version shared and locally
            byte[] cacheData = new GeneralFormatter().Serialize(data);
            DateTime created = DateTime.UtcNow;

            IDatabase db = Redis.GetDatabase();
            if (YetaWFManager.IsSync()) {
                db.KeyDelete(GetVersionKey(key));
                db.KeyDelete(GetDataKey(key));
                db.StringSet(GetVersionKey(key), created.Ticks);
                db.StringSet(GetDataKey(key), cacheData);
            } else {
                await db.KeyDeleteAsync(GetVersionKey(key));
                await db.KeyDeleteAsync(GetDataKey(key));
                await db.StringSetAsync(GetVersionKey(key), created.Ticks);
                await db.StringSetAsync(GetDataKey(key), cacheData);
            }

            LocalSharedCacheObject localCacheObj = new LocalSharedCacheObject {
                Created = created,
                Key = key,
                Value = cacheData,
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
                IDatabase db = Redis.GetDatabase();
                long? val;
                if (YetaWFManager.IsSync()) {
                    val = (long?)db.StringGet(GetVersionKey(key));
                } else {
                    val = (long?)await db.StringGetAsync(GetVersionKey(key));
                }
                if (val != null) {
                    DateTime sharedCacheCreated = new DateTime((long)val);
                    if (sharedCacheCreated != localInfo.Data.Created) {
                        // shared cached version is different, retrieve and save locally
                        byte[] sharedCacheData;
                        if (YetaWFManager.IsSync()) {
                            sharedCacheData = db.StringGet(GetDataKey(key));
                        } else {
                            sharedCacheData = await db.StringGetAsync(GetDataKey(key));
                        }
                        if (sharedCacheData == null) { // this shouldn't happen, we just got the shared version
                                                      // return the local data instead
                        } else {
                            LocalSharedCacheObject localCacheObj = new LocalSharedCacheObject {
                                Created = sharedCacheCreated,
                                Key = key,
                                Value = sharedCacheData,
                            };
                            await localCacheDP.AddAsync(key, localCacheObj); // save as locally cached version
                            return new GetObjectInfo<TYPE> {
                                Success = true,
                                Data = new GeneralFormatter().Deserialize<TYPE>(sharedCacheData),
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
    }
}
