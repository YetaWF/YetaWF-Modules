/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL2;

namespace YetaWF.Modules.Languages.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.LocalizeConfigDataProvider), typeof(LocalizeConfigDataProvider));
        }
        class LocalizeConfigDataProvider : SQLSimpleObject<int, LocalizeConfigData> {
            public LocalizeConfigDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
