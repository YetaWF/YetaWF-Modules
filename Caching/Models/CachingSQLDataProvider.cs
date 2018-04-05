/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL;

namespace YetaWF.Modules.Caching.DataProvider.SQL {

    public class CachingSQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.SharedCacheObjectSQLDataProvider), typeof(SharedCacheObjectSQLDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.SharedCacheVersionSQLDataProvider), typeof(SharedCacheVersionSQLDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.StaticObjectMultiSQLDataProvider), typeof(StaticObjectMultiSQLDataProvider));
        }
        class SharedCacheObjectSQLDataProvider : SQLSimpleObject<string, SharedCacheObject> {
            public SharedCacheObjectSQLDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class SharedCacheVersionSQLDataProvider : SQLSimpleObject<string, SharedCacheVersion> {
            public SharedCacheVersionSQLDataProvider(Dictionary<string, object> options) : base(options) { }
        }       
        class StaticObjectMultiSQLDataProvider : SQLSimpleObject<string, SharedCacheObject> {
            public StaticObjectMultiSQLDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
