/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL;

namespace YetaWF.Modules.Caching.DataProvider.SQL;

/// <summary>
/// An instance of this class is instantiated during application startup and registers low-level data providers
/// implementing shared and static caching using SQL.
/// </summary>
/// <remarks>These low-level data providers are only used with distributed caching (multi-instance site).
///
/// Applications do not access these low-level data providers directly.
/// Caching services provided by YetaWF.Core.IO.Caching should be used instead.
/// </remarks>
public class CachingSQLDataProvider : IExternalDataProvider {

    /// <summary>
    /// Called by the framework to register external data providers that expose the YetaWF.Core.DataProvider.IExternalDataProvider interface.
    /// </summary>
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