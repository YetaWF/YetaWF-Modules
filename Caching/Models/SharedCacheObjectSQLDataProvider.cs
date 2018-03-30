/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL;

namespace YetaWF.Modules.Caching.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.SharedCacheObjectDataProvider), typeof(SharedCacheDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.SharedCacheVersionDataProvider), typeof(SharedCacheVersionProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.StaticSharedCacheObjectDataProvider), typeof(StaticSharedCacheObjectDataProvider));
        }
        class SharedCacheDataProvider : SQLSimpleObject<string, SharedCacheObject> {
            public SharedCacheDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class SharedCacheVersionProvider : SQLSimpleObject<string, SharedCacheVersion> {
            public SharedCacheVersionProvider(Dictionary<string, object> options) : base(options) { }
        }       
        class StaticSharedCacheObjectDataProvider : SQLSimpleObject<string, SharedCacheVersion> {
            public StaticSharedCacheObjectDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
