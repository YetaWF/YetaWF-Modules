/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Support;
using YetaWF.Core.Support.Serializers;

namespace YetaWF.Modules.Caching.DataProvider {

    /// <summary>
    /// Local cache data provider.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    internal class LocalCacheObjectDataProvider : DataProviderImpl, ICacheDataProvider {

        private const string EmptyCachedObject = "Empty";

        public static ICacheDataProvider GetProvider() {
            return new LocalCacheObjectDataProvider();
        }

        // Implementation

        public LocalCacheObjectDataProvider() : base(0) { }

        public Task AddAsync<TYPE>(string key, TYPE? data) {
            if (data == null) {
                YetaWFManager.MemoryCache.Set<object>(key, EmptyCachedObject);
            } else {
                // we can't save the entire object, just the data that we actually marked as savable (Properties)
                // the main reason the object is not savable is because it may be derived from other classes with
                // volatile data which is expected to be cleared for every invocation.
                byte[] cacheData;
                if (typeof(TYPE) == typeof(byte[]))
                    cacheData = (byte[])(object) data;
                else
                    cacheData = new GeneralFormatter().Serialize(data);
                YetaWFManager.MemoryCache.Set<byte[]>(key, cacheData);
            }
            return Task.CompletedTask;
        }
        public Task<GetObjectInfo<TYPE>> GetAsync<TYPE>(string key) {
            object? data = null;
            data = YetaWFManager.MemoryCache.Get(key);
            if (data != null) {
                if (data.GetType() == typeof(string) && (string)data == EmptyCachedObject) {
                    return Task.FromResult(new GetObjectInfo<TYPE> {
                        Data = default,
                        Success = true,
                    });
                } else {
                    TYPE? desData;
                    if (typeof(TYPE) == typeof(byte[]))
                        desData = (TYPE)data;
                    else
                        desData = new GeneralFormatter().Deserialize<TYPE>((byte[])data);
                    return Task.FromResult(new GetObjectInfo<TYPE> {
                        Data = desData,
                        Success = true,
                    });
                }
            } else
                return Task.FromResult(new GetObjectInfo<TYPE>());
        }

        public Task RemoveAsync<TYPE>(string key) {
            YetaWFManager.MemoryCache.Remove(key);
            return Task.CompletedTask;
        }
    }
}
