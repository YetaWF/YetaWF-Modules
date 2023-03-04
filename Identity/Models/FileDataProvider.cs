/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.IO;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Identity.DataProvider.File;

public class FileDataProvider : IExternalDataProvider {

    public void Register() {
        DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.AuthorizationDataProvider), typeof(AuthorizationDataProvider));
        DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.LoginConfigDataProvider), typeof(LoginConfigDataProvider));
        DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.RoleDefinitionDataProvider), typeof(RoleDefinitionDataProvider));
        DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.SuperuserDefinitionDataProvider), typeof(SuperuserDefinitionDataProvider));
        DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.UserDefinitionDataProvider), typeof(UserDefinitionDataProvider));
        DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.UserLoginInfoDataProvider), typeof(UserLoginInfoDataProvider));
    }
    class AuthorizationDataProvider : FileDataProvider<string, Authorization> {
        public AuthorizationDataProvider(Dictionary<string, object> options) : base(options) { }
        public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Package.AreaName, "Authorization", SiteIdentity.ToString()); }
    }
    class LoginConfigDataProvider : FileDataProvider<int, LoginConfigData> {
        public LoginConfigDataProvider(Dictionary<string, object> options) : base(options) { }
        public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Dataset, SiteIdentity.ToString()); }
    }
    class RoleDefinitionDataProvider : FileDataProvider<string, RoleDefinition> {
        public RoleDefinitionDataProvider(Dictionary<string, object> options) : base(options) { }
        public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Package.AreaName, "Roles", SiteIdentity.ToString()); }
    }
    class SuperuserDefinitionDataProvider : FileDataProvider<string, UserDefinition> {
        public SuperuserDefinitionDataProvider(Dictionary<string, object> options) : base(options) { }
        public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Dataset); }
    }
    class UserDefinitionDataProvider : FileDataProvider<string, UserDefinition> {
        public UserDefinitionDataProvider(Dictionary<string, object> options) : base(options) { }
        public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Dataset, SiteIdentity.ToString()); }
    }
    class UserLoginInfoDataProvider : FileDataProvider<string, LoginInfo> {
        public UserLoginInfoDataProvider(Dictionary<string, object> options) : base(options) { }
        public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Dataset, SiteIdentity.ToString()); }
    }
}
