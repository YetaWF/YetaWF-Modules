/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/UserProfile#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.PostgreSQL;

namespace YetaWF.Modules.UserProfile.DataProvider.PostgreSQL {

    public class PostgreSQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.UserInfoDataProvider), typeof(UserInfoDataProvider));
        }
        class UserInfoDataProvider : SQLSimpleObject<int, UserInfo> {
            public UserInfoDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
