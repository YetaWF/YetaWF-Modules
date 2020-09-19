/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Support;
using YetaWF.Modules.Caching.Controllers;
using System.Linq;

namespace YetaWF.Modules.Caching.DataProvider {

    /// <summary>
    /// A local static cache implementation (in-memory ONLY) for a single instance (really not shared) for SMALL objects.
    /// This is typically used for small objects such as grid/propertylist definition files.
    ///
    /// The cache can be cleared explicitly using the Control Panel and there is a configurable cache expiration time after which the entry is removed
    /// (see AppSettings.json YetaWF_Caching:SmallObjectCacheDuration defined in minutes. 0 no expiration, -1 don't cache, any
    /// other value is the cache duration in minutes.
    ///
    /// In development/debug objects are not cached, for release mode the default is caching that doesn't expire.
    /// </summary>
    public class StaticSmallObjectLocalDataProvider : ICacheDataProvider, ICacheClearable {

        /// <summary>
        /// Constructor.
        /// </summary>
        public StaticSmallObjectLocalDataProvider() {
            DisposableTracker.AddObject(this);
            DurationSeconds = WebConfigHelper.GetValue<int>(AreaRegistration.CurrentPackage.AreaName, "SmallObjectCacheDuration", YetaWFManager.Deployed ? 0 : -1);
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="durationSeconds">The number of minutes a cached object is retained. 0 means forever, -1 means don't cache at all.</param>
        public StaticSmallObjectLocalDataProvider(int durationSeconds) {
            DisposableTracker.AddObject(this);
            DurationSeconds = durationSeconds;
        }

        /// <summary>
        /// StaticSmallObjectLocalDataProvider factory.
        /// </summary>
        /// <returns>An instance of an ICacheDataProvider interface.</returns>
        /// <remarks>The returned ICacheDataProvider interface must be Dispose'd once it is no longer needed.</remarks>
        public static ICacheDataProvider GetProvider() {
            return new StaticSmallObjectLocalDataProvider();
        }

        private class CachedObject {
            public DateTime Created { get; set; }
            public object Data { get; set; }
        }

        private static Dictionary<string, CachedObject> StaticObjects = new Dictionary<string, CachedObject>();
        private static object _lockObject = new object();

        private int DurationSeconds { get; set; }
        private bool DontCache { get { return DurationSeconds < 0; } }
        private bool NoExpiration { get { return DurationSeconds == 0; } }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            Dispose(true);
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">true to release the DisposableTracker reference count, false otherwise.</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                DisposableTracker.RemoveObject(this);
            }
        }

        private bool HasExpired(CachedObject cache) {
            if (NoExpiration) return false;
            if (DontCache) return true;
            return DateTime.Now > cache.Created.AddMinutes(DurationSeconds);
        }

        // API

        private string GetKey(string key) {
            return $"__staticSM__{key}";
        }

        /// <summary>
        /// Adds an object to the cache.
        /// </summary>
        /// <typeparam name="TYPE">The type of the object.</typeparam>
        /// <param name="key">The resource name.</param>
        /// <param name="data">The data to cache.</param>
        public Task AddAsync<TYPE>(string key, TYPE data) {
            if (DontCache) return Task.CompletedTask;
            key = GetKey(key);
            CachedObject cache = new CachedObject {
                Created = DateTime.Now,
                Data = data,
            };
            lock (_lockObject) { // used to protect StaticObjects - local only
                StaticObjects.Remove(key);
                StaticObjects.Add(key, cache);
                RemoveExpired();
            }
            return Task.CompletedTask;
        }
        /// <summary>
        /// Retrieves a cached object.
        /// </summary>
        /// <typeparam name="TYPE">The type of the object.</typeparam>
        /// <param name="key">The resource name.</param>
        /// <returns>Returns an object containing success indicators and the data, if available.</returns>
        public Task<GetObjectInfo<TYPE>> GetAsync<TYPE>(string key) {
            if (!DontCache) {
                // get cached version
                key = GetKey(key);
                CachedObject cache;
                lock (_lockObject) { // used to protect StaticObjects - local only
                    RemoveExpired();
                    if (StaticObjects.TryGetValue(key, out cache)) {
                        if (!HasExpired(cache)) {
                            return Task.FromResult(new GetObjectInfo<TYPE> {
                                Success = true,
                                Data = (TYPE)cache.Data,
                            });
                        } else {
                            StaticObjects.Remove(key);
                        }
                    }
                }
            }
            return Task.FromResult(new GetObjectInfo<TYPE> {
                Success = false,
            });
        }
        /// <summary>
        /// Removes the cached object.
        /// </summary>
        /// <typeparam name="TYPE"></typeparam>
        /// <param name="key">The resource name.</param>
        /// <remarks>
        /// It is permissible to remove non-existent objects.
        /// </remarks>
        public Task RemoveAsync<TYPE>(string key) {
            if (DontCache) return Task.CompletedTask;
            key = GetKey(key);
            lock (_lockObject) { // used to protect StaticObjects - local only
                StaticObjects.Remove(key);
                RemoveExpired();
            }
            return Task.CompletedTask;
        }

        private void RemoveExpired() {
            // already locked
            StaticObjects = (from s in StaticObjects where DateTime.Now.AddMinutes(-DurationSeconds) < s.Value.Created select s).ToDictionary(x => x.Key, x =>x.Value);
        }

        /// <summary>
        /// Clears the cache completely.
        /// </summary>
        public Task ClearAllAsync() {
            lock (_lockObject) { // used to protect StaticObjects - local only
                StaticObjects = new Dictionary<string, CachedObject>();
            }
            return Task.CompletedTask;
        }
    }
}
