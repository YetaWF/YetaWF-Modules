/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/AddThis#License */

using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.PostgreSQL;

namespace YetaWF.Modules.AddThis.DataProvider.PostgreSQL {

    public class PostgreSQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(PostgreSQLBase.ExternalName, typeof(DataProvider.ConfigDataProvider), typeof(PostgreSQLSimpleObject<int, ConfigData>));
        }
    }
}
