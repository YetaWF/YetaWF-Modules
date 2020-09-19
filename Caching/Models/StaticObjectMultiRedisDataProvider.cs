/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Support;
using YetaWF.Core.Support.Serializers;

namespace YetaWF.Modules.Caching.DataProvider {

    /// <summary>
    /// A shared cache implementation backed by memory to improve performance.
    /// This is intended as a way to preserve and share "static" in-memory data across instances.
    /// Shared cache will only be retrieved to check if there is a newer cached object available. Once
    /// it is known that a new object is available the data is retrieved.
    /// This is equivalent to StaticObjectSingleDataProvider on a single-instance site.
    /// </summary>
    internal class StaticObjectMultiRedisDataProvider : ICacheDataProvider, IDisposable {

        public static ICacheDataProvider GetProvider() {
            return new StaticObjectMultiRedisDataProvider();
        }

        // Implementation

        private static ConnectionMultiplexer Redis { get; set; }
        private static string KeyPrefix { get; set; }
        private static Guid Id { get; set; }

        public StaticObjectMultiRedisDataProvider() {
            DisposableTracker.AddObject(this);
        }
        public void Dispose() { Dispose(true); }
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                DisposableTracker.RemoveObject(this);
            }
        }

        public static Task InitAsync(string configString, string keyPrefix) {
            Redis = ConnectionMultiplexer.Connect(configString);
            KeyPrefix = keyPrefix;
            Id = Guid.NewGuid();
            return Task.CompletedTask;
        }

        private RedisKey GetVersionKey(string key) {
            return $"{key}__Version__";
        }
        private RedisKey GetDataKey(string key) {
            return $"{key}__Data__";
        }
        private string GetKey(string key) {
            return $"{key}__static__";
        }

        private static Dictionary<string, StaticCacheObject> StaticObjects = new Dictionary<string, StaticCacheObject>();
        private static object _lockObject = new object();

        // API

        /// <summary>
        /// Add a shared object.
        /// </summary>
        /// <remarks>This requires an active Lock using a lock provider.</remarks>
        public async Task AddAsync<TYPE>(string key, TYPE data) {
            key = KeyPrefix + key;
            key = GetKey(key);
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

            StaticCacheObject cachedObj = new StaticCacheObject {
                Key = key,
                Value = data,
                Created = created,
            };
            lock (_lockObject) { // used to protect StaticObjects - local only
                StaticObjects.Remove(key);
                StaticObjects.Add(key, cachedObj);
            }
        }
        public async Task<GetObjectInfo<TYPE>> GetAsync<TYPE>(string key) {
            key = KeyPrefix + key;
            key = GetKey(key);
            // get cached version
            TYPE data = default(TYPE);

            StaticCacheObject cachedObj;
            bool localValid;
            lock (_lockObject) { // used to protect StaticObjects - local only
                localValid = StaticObjects.TryGetValue(key, out cachedObj);
            }
            if (!localValid) {
                cachedObj = new StaticCacheObject {
                    Key = key,
                    Value = data,
                    Created = DateTime.MinValue,
                };
            } else {
                data = (TYPE)cachedObj.Value;
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
                if (sharedCacheCreated != cachedObj.Created) {
                    // shared cached version is different, retrieve and save locally
                    byte[] sharedCacheData;
                    if (YetaWFManager.IsSync()) {
                        sharedCacheData = db.StringGet(GetDataKey(key));
                    } else {
                        sharedCacheData = await db.StringGetAsync(GetDataKey(key));
                    }
                    if (sharedCacheData == null) {
                        data = default(TYPE);
                    } else {
                        data = new GeneralFormatter().Deserialize<TYPE>(sharedCacheData);
                    }
                    cachedObj = new StaticCacheObject {
                        Created = sharedCacheCreated,
                        Key = key,
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
            key = KeyPrefix + key;
            // We're adding a new version
            await AddAsync(key, default(TYPE));
        }
    }
}
