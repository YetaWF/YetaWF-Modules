/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/UserSettings#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.PostgreSQL;

namespace YetaWF.Modules.UserSettings.DataProvider.PostgreSQL {

    public class PostgreSQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.UserDataProvider), typeof(UserDataProvider));
        }
        class UserDataProvider : SQLSimpleObject<int, UserData> {
            public UserDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
