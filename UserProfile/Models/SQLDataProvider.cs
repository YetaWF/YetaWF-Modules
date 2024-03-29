/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/UserProfile#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL;

namespace YetaWF.Modules.UserProfile.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.UserInfoDataProvider), typeof(UserInfoDataProvider));
        }
        class UserInfoDataProvider : SQLSimpleObject<int, UserInfo> {
            public UserInfoDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
