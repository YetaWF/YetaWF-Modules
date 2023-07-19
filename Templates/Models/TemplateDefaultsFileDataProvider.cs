using System.Collections.Generic;
using System.IO;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Templates.DataProvider.File;

public class TemplateDefaultsFileDataProvider : IExternalDataProvider {

    public void Register() {
        DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.TemplateDefaultsDataProvider), typeof(TemplateDefaultsDataProvider));
    }
    class TemplateDefaultsDataProvider : FileDataProvider<int, TemplateDefaults> {
        public TemplateDefaultsDataProvider(Dictionary<string, object> options) : base(options) { }
        public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Dataset, SiteIdentity.ToString()); }
    }
}
