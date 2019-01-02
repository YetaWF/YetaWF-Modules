/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/UserProfile#License */

using System.Collections.Generic;
using System.IO;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.UserProfile.DataProvider.File {

    public class FileDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.UserInfoDataProvider), typeof(UserInfoDataProvider));
        }
        class UserInfoDataProvider : FileDataProvider<int, UserInfo> {
            public UserInfoDataProvider(Dictionary<string, object> options) : base(options) { }
            public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Dataset, SiteIdentity.ToString()); }
        }
    }
}
