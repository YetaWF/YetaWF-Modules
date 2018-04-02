﻿/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Support;
using YetaWF.Modules.Caching.Controllers;
using YetaWF.Modules.Caching.DataProvider;

namespace YetaWF.Modules.Caching.Startup {

    public class Application : IInitializeApplicationStartup {

        public const string Distributed = "Distributed";

        public Task InitializeApplicationStartupAsync() {

            // permanently created dataproviders (never disposed)
            bool distributed = WebConfigHelper.GetValue<bool>(AreaRegistration.CurrentPackage.AreaName, Distributed);

            if (distributed) {
                // distributed caching uses local and shared cache
                YetaWF.Core.IO.Caching.GetLocalCacheProvider = LocalCacheObjectDataProvider.GetLocalCacheProvider;
                YetaWF.Core.IO.Caching.GetSharedCacheProvider = SharedCacheObjectDataProvider.GetLocalCacheProvider;
                YetaWF.Core.IO.Caching.GetStaticCacheProvider = StaticObjectMultiDataProvider.GetLocalCacheProvider;
                YetaWF.Core.Support.Startup.MultiInstance = true;
            } else {
                // non-distributed caching uses local cache only
                YetaWF.Core.IO.Caching.GetLocalCacheProvider = LocalCacheObjectDataProvider.GetLocalCacheProvider;
                YetaWF.Core.IO.Caching.GetSharedCacheProvider = LocalCacheObjectDataProvider.GetLocalCacheProvider;
                YetaWF.Core.IO.Caching.GetStaticCacheProvider = StaticObjectSingleDataProvider.GetLocalCacheProvider;
                YetaWF.Core.Support.Startup.MultiInstance = false;
            }
            return Task.CompletedTask;
        }
    }
}
