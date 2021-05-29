/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Caching.DataProvider {

    /// <summary>
    /// A in-memory ONLY cache implementation for a single instance (really shared) without serialization/deserialization (not suitable for ModuleDefinition).
    /// This is intended as a way to preserve "static" in-memory data. This is equivalent to the StaticObjectMultiDataProvider class on a multi-instance site.
    /// </summary>
    internal class StaticObjectSingleDataProvider : ICacheDataProvider {

        public static ICacheDataProvider GetProvider() {
            return new StaticObjectSingleDataProvider();
        }

        public YetaWFManager Manager { get { return YetaWFManager.Manager; } }
        public bool HaveManager { get { return YetaWFManager.HaveManager; } }

        private static Dictionary<string, object?> StaticObjects = new Dictionary<string, object?>();
        private static object _lockObject = new object();

        public StaticObjectSingleDataProvider() {
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
        public Task AddAsync<TYPE>(string key, TYPE? data) {
            key = GetKey(key);
            lock (_lockObject) { // used to protect StaticObjects - local only
                StaticObjects.Remove(key);
                StaticObjects.Add(key, data);
            }
            return Task.CompletedTask;
        }
        public Task<GetObjectInfo<TYPE>> GetAsync<TYPE>(string key) {
            // get cached version
            key = GetKey(key);
            object? obj;
            lock (_lockObject) { // used to protect StaticObjects - local only
                if (StaticObjects.TryGetValue(key, out obj)) {
                    return Task.FromResult(new GetObjectInfo<TYPE> {
                        Success = true,
                        Data = (TYPE?)obj,
                    });
                }
            }
            return Task.FromResult(new GetObjectInfo<TYPE> {
                Success = false,
            });
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
