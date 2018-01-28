﻿/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL2;

namespace YetaWF.Modules.Identity.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQL2Base.ExternalName, typeof(DataProvider.AuthorizationDataProvider), typeof(AuthorizationDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQL2Base.ExternalName, typeof(DataProvider.LoginConfigDataProvider), typeof(LoginConfigDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQL2Base.ExternalName, typeof(DataProvider.RoleDefinitionDataProvider), typeof(RoleDefinitionDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQL2Base.ExternalName, typeof(DataProvider.SuperuserDefinitionDataProvider), typeof(SuperuserDefinitionDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQL2Base.ExternalName, typeof(DataProvider.UserDefinitionDataProvider), typeof(UserDefinitionDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQL2Base.ExternalName, typeof(DataProvider.UserLoginInfoDataProvider), typeof(UserLoginInfoDataProvider));
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
