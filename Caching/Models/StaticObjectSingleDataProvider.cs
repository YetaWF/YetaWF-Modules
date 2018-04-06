/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Caching.DataProvider {

    /// <summary>
    /// A shared cache implementation in-memory ONLY for a single instance. (Really not shared).
    /// This is intended as a way to preserve "static" in-memory data. This is equivalent to StaticObjectMultiDataProvider on a multi-instance site.
    /// </summary>
    public class StaticObjectSingleDataProvider : ICacheStaticDataProvider {

        public static ICacheStaticDataProvider GetProvider() {
            return new StaticObjectSingleDataProvider();
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
    }
}
