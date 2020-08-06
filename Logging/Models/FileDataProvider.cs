/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Logging#License */

using System.Collections.Generic;
using System.IO;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Logging.DataProvider.File {

    public class FileDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.LoggingConfigDataProvider), typeof(LoggingConfigDataProvider));
        }
        class LoggingConfigDataProvider : FileDataProvider<int, LoggingConfigData> {
            public LoggingConfigDataProvider(Dictionary<string, object> options) : base(options) { }
            public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Dataset, SiteIdentity.ToString()); }
        }
    }
}
