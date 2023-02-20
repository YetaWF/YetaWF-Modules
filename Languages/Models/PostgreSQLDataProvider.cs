/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.PostgreSQL;

namespace YetaWF.Modules.Languages.DataProvider.PostgreSQL;

public class PostgreSQLDataProvider : IExternalDataProvider {

    public void Register() {
        DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.LocalizeConfigDataProvider), typeof(LocalizeConfigDataProvider));
    }
    class LocalizeConfigDataProvider : SQLSimpleObject<int, LocalizeConfigData> {
        public LocalizeConfigDataProvider(Dictionary<string, object> options) : base(options) { }
    }
}
