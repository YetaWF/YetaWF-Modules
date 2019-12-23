/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

using System.Collections.Generic;
using System.IO;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Basics.DataProvider.File {

    public class FileDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.AlertConfigDataProvider), typeof(AlertConfigDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.RecaptchaV2ConfigDataProvider), typeof(RecaptchaV2ConfigDataProvider));
        }
        class AlertConfigDataProvider : FileDataProvider<int, AlertConfig> {
            public AlertConfigDataProvider(Dictionary<string, object> options) : base(options) { }
            public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Dataset, SiteIdentity.ToString()); }
        }
        class RecaptchaConfigDataProvider : FileDataProvider<int, RecaptchaV2Config> {
            public RecaptchaConfigDataProvider(Dictionary<string, object> options) : base(options) { }
            public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Dataset); }
        }
        class RecaptchaV2ConfigDataProvider : FileDataProvider<int, RecaptchaV2Config> {
            public RecaptchaV2ConfigDataProvider(Dictionary<string, object> options) : base(options) { }
            public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Dataset, SiteIdentity.ToString()); }
        }
    }
}
