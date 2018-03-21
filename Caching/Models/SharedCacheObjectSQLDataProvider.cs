using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL;

namespace YetaWF.Modules.Caching.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.SharedCacheObjectDataProvider), typeof(SharedCacheDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.SharedCacheVersionDataProvider), typeof(SharedCacheVersionProvider));
        }
        class SharedCacheDataProvider : SQLSimpleIdentityObject<string, SharedCacheObject> {
            public SharedCacheDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class SharedCacheVersionProvider : SQLSimpleIdentityObject<string, SharedCacheVersion> {
            public SharedCacheVersionProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
