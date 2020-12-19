/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

using System;
using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL;

namespace YetaWF.Modules.Menus.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.MenuInfoDataProvider), typeof(MenuInfoDataProvider));
        }
        class MenuInfoDataProvider : SQLSimpleObject<Guid, MenuInfo> {
            public MenuInfoDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
