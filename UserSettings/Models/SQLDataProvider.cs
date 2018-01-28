/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/UserSettings#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL2;

namespace YetaWF.Modules.UserSettings.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQL2Base.ExternalName, typeof(DataProvider.UserDataProvider), typeof(UserDataProvider));
        }
        class UserDataProvider : SQLSimpleObject<int, UserData> {
            public UserDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
