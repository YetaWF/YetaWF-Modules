/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Caching.DataProvider {

    /// <summary>
    /// A shared cache implementation in-memory ONLY for a single instance. (Really not shared).
    /// </summary>
    public class StaticSingleCacheObjectDataProvider : ICacheStaticObject {

        protected YetaWFManager Manager { get { return YetaWFManager.Manager; } }
        protected bool HaveManager { get { return YetaWFManager.HaveManager; } }

        private static Dictionary<string, object> StaticObjects = new Dictionary<string, object>();

        // API

        private string GetKey<TYPE>() {
            int identity = (YetaWFManager.HaveManager && Manager.HaveCurrentSite) ? Manager.CurrentSite.Identity : 0;
            return $"__static_{identity}/{typeof(TYPE).FullName}";
        }
        public Task AddAsync<TYPE>(TYPE data) {
            string key = GetKey<TYPE>();
            StringLocks.DoAction(key, () => {
                StaticObjects.Remove(key);
                StaticObjects.Add(key, data);
            });
            return Task.CompletedTask;
        }
        public async Task<GetObjectInfo<TYPE>> GetAsync<TYPE>(Func<Task<TYPE>> noDataCallback) {
            // get cached version
            GetObjectInfo<TYPE> retVal = null;
            string key = GetKey<TYPE>();
            await StringLocks.DoActionAsync(key, async () => {
                object obj;
                if (!StaticObjects.TryGetValue(key, out obj))
                    obj = await noDataCallback();
                retVal = new GetObjectInfo<TYPE> {
                    Success = true,
                    Data = (TYPE)obj,
                };
            });
            return retVal;
        }
        public Task RemoveAsync<TYPE>() {
            string key = GetKey<TYPE>();
            StringLocks.DoAction(key, () => {
                StaticObjects.Remove(key);
            });
            return Task.CompletedTask;
        }

        private class StaticLockObject : IDisposable, IStaticLockObject {

            private static List<string> LockKeys = new List<string>();
            private static object _lockObject = new object();

            public StaticLockObject() {
                DisposableTracker.AddObject(this);
            }
            public Task UnlockAsync() {
                return Task.CompletedTask;
            }
            public void Dispose() { Dispose(true); }
            protected virtual void Dispose(bool disposing) {
                if (disposing) {
                    DisposableTracker.RemoveObject(this);
                }
            }
        }
        /// <summary>
        /// Lock a static shared object.
        /// </summary>
        /// <returns>This does not support reentrancy.</returns>
        public Task<IStaticLockObject> LockAsync<TYPE>() {
            IStaticLockObject lockObject = new StaticLockObject();
            return Task.FromResult(lockObject);
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

    }
}
