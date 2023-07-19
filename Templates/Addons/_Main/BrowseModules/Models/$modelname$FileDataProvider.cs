using System.Collections.Generic;
using System.IO;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace $companynamespace$.Modules.$projectnamespace$.DataProvider.File;

public class $modelname$FileDataProvider : IExternalDataProvider {

    public void Register() {
        DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.$modelname$DataProvider), typeof($modelname$DataProvider));
    }
    class $modelname$DataProvider : FileDataProvider<int, $modelname$> {
        public $modelname$DataProvider(Dictionary<string, object> options) : base(options) { }
        public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Dataset, SiteIdentity.ToString()); }
    }
}
