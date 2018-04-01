/* Copyright � 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Support.Serializers;

namespace YetaWF.Modules.Caching.DataProvider {

    /// <summary>
    /// Local cache data provider.
    /// </summary>
    public class LocalCacheObjectDataProvider : DataProviderImpl, ICacheObject {

        // Implementation

        public LocalCacheObjectDataProvider() : base(0) { }

        public Task AddAsync<TYPE>(string key, TYPE data) {
            if (data == null) {
#if MVC6
                YetaWFManager.MemoryCache.Set<object>(cacheKey, YetaWF.Core.IO.Caching.EmptyCachedObject);
#else
                System.Web.HttpRuntime.Cache[key] = YetaWF.Core.IO.Caching.EmptyCachedObject;
#endif
            } else {
                // we can't save the entire object, just the data that we actually marked as savable (Properties)
                // the main reason the object is not savable is because it may be derived from other classes with
                // volatile data which is expected to be cleared for every invocation.
                byte[] cacheData;
                if (typeof(TYPE) == typeof(byte[]))
                    cacheData = (byte[])(object) data;
                else
                    cacheData = new GeneralFormatter().Serialize(data);
#if MVC6
                    YetaWFManager.MemoryCache.Set<byte[]>(key, cacheData);
#else
                    System.Web.HttpRuntime.Cache[key] = cacheData;
#endif
            }
            return Task.CompletedTask;
        }
        public Task<GetObjectInfo<TYPE>> GetAsync<TYPE>(string key) {
            object data = null;
#if MVC6
            data = YetaWFManager.MemoryCache.Get(cacheKey);
#else
            data = System.Web.HttpRuntime.Cache[key];
#endif
            if (data != null) {
                if (data.GetType() == typeof(string) && (string)data == YetaWF.Core.IO.Caching.EmptyCachedObject) {
                    return Task.FromResult(new GetObjectInfo<TYPE> {
                        Data = default(TYPE),
                        Success = true,
                    });
                } else {
                    TYPE desData;
                    if (typeof(TYPE) == typeof(byte[]))
                        desData = (TYPE)data;
                    else
                        desData = (TYPE)new GeneralFormatter().Deserialize((byte[])data);
                    return Task.FromResult(new GetObjectInfo<TYPE> {
                        Data = (TYPE)desData,
                        Success = true,
                    });
                }
            } else
                return Task.FromResult(new GetObjectInfo<TYPE>());
        }

        public Task RemoveAsync<TYPE>(string key) {
#if MVC6
            YetaWFManager.MemoryCache.Remove(cacheKey);
#else
            System.Web.HttpRuntime.Cache.Remove(key);
#endif
            return Task.CompletedTask;
        }
    }
}