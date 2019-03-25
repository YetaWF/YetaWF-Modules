/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/AddThis#License */

using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL;

namespace YetaWF.Modules.AddThis.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.ConfigDataProvider), typeof(SQLSimpleObject<int, ConfigData>));
        }
    }
}
