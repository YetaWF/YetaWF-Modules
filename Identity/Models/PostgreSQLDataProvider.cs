/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.PostgreSQL;

namespace YetaWF.Modules.Identity.DataProvider.PostgreSQL {

    public class PostgreSQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.AuthorizationDataProvider), typeof(AuthorizationDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.LoginConfigDataProvider), typeof(LoginConfigDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.RoleDefinitionDataProvider), typeof(RoleDefinitionDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.SuperuserDefinitionDataProvider), typeof(SuperuserDefinitionDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.UserDefinitionDataProvider), typeof(UserDefinitionDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.UserLoginInfoDataProvider), typeof(UserLoginInfoDataProvider));
        }
        class AuthorizationDataProvider : SQLSimpleObject<string, Authorization> {
            public AuthorizationDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class LoginConfigDataProvider : SQLSimpleObject<int, LoginConfigData> {
            public LoginConfigDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class RoleDefinitionDataProvider : SQLSimpleObject<string, RoleDefinition> {
            public RoleDefinitionDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class SuperuserDefinitionDataProvider : SQLSimpleObject<string, UserDefinition> {
            public SuperuserDefinitionDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class UserDefinitionDataProvider : SQLSimpleObject<string, UserDefinition> {
            public UserDefinitionDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class UserLoginInfoDataProvider : SQLSimpleObject<string, LoginInfo> {
            public UserLoginInfoDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
