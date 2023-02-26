/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License */

using System.Collections.Generic;
using System.IO;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.SyntaxHighlighter.DataProvider.File;

public class FileDataProvider : IExternalDataProvider {

    public void Register() {
        DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.ConfigDataProvider), typeof(ConfigDataProvider));
    }
    class ConfigDataProvider : FileDataProvider<int, ConfigData> {
        public ConfigDataProvider(Dictionary<string, object> options) : base(options) { }
        public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Dataset, SiteIdentity.ToString()); }
    }
}
