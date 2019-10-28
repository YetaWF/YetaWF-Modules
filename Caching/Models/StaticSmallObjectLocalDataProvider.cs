/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Support;
using YetaWF.Modules.Caching.Controllers;

namespace YetaWF.Modules.Caching.DataProvider {

    /// <summary>
    /// A local static cache implementation (in-memory ONLY) for a single instance (really not shared) for SMALL objects.
    /// This is typically used for small objects such as grid/propertylist definition files.
    ///
    /// The cache can be cleared explicitly and there is a configurable cache expiration time after which the entry is removed
    /// (see AppSettings.json YetaWF_Caching:SmallObjectCacheDuration defined in minutes. 0 no expiration, -1 don't cache, any
    /// other value is the cache duration in minutes.
    ///
    /// In development/debug objects are not cached, for release mode the default is caching that doesn't expire.
    /// </summary>
    internal class StaticSmallObjectLocalDataProvider : ICacheDataProvider, ICacheClearable {

        static StaticSmallObjectLocalDataProvider() {
            DurationMinutes = WebConfigHelper.GetValue<int>(AreaRegistration.CurrentPackage.AreaName, "SmallObjectCacheDuration", YetaWFManager.GetDeployed() ? 0 : -1);
        }

        public static ICacheDataProvider GetProvider() {
            return new StaticSmallObjectLocalDataProvider();
        }

        public YetaWFManager Manager { get { return YetaWFManager.Manager; } }
        public bool HaveManager { get { return YetaWFManager.HaveManager; } }

        public class CachedObject {
            public DateTime Created { get; set; }
            public object Data { get; set; }
        }

        private static Dictionary<string, CachedObject> StaticObjects = new Dictionary<string, CachedObject>();
        private static object _lockObject = new object();

        private static int DurationMinutes { get; set; }
        private bool DontCache { get { return DurationMinutes < 0; } }
        private bool NoExpiration { get { return DurationMinutes == 0; } }

        public StaticSmallObjectLocalDataProvider() {
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

        private bool HasExpired(CachedObject cache) {
            if (NoExpiration) return false;
            if (DontCache) return true;
            return DateTime.Now > cache.Created.AddMinutes(DurationMinutes);
        }
        //~StaticSmallObjectLocalDataProvider() { Dispose(false); }

        // API

        private string GetKey(string key) {
            return $"__staticSM__{key}";
        }
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
            }
            return Task.CompletedTask;
        }
        public Task<GetObjectInfo<TYPE>> GetAsync<TYPE>(string key) {
            if (!DontCache) {
                // get cached version
                key = GetKey(key);
                CachedObject cache;
                lock (_lockObject) { // used to protect StaticObjects - local only
                    if (StaticObjects.TryGetValue(key, out cache)) {
                        if (!HasExpired(cache)) {
                            return Task.FromResult(new GetObjectInfo<TYPE> {
                                Success = true,
                                Data = (TYPE)cache.Data,
                            });
                        }
                    }
                }
            }
            return Task.FromResult(new GetObjectInfo<TYPE> {
                Success = false,
            });
        }
        public Task RemoveAsync<TYPE>(string key) {
            if (DontCache) return Task.CompletedTask;
            key = GetKey(key);
            lock (_lockObject) { // used to protect StaticObjects - local only
                StaticObjects.Remove(key);
            }
            return Task.CompletedTask;
        }

        public Task ClearAllAsync() {
            lock (_lockObject) { // used to protect StaticObjects - local only
                StaticObjects = new Dictionary<string, CachedObject>();
            }
            return Task.CompletedTask;
        }
    }
}
