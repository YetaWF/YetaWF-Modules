/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System.Collections.Generic;
using System.IO;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Search.DataProvider.File {

    public class FileDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.SearchConfigDataProvider), typeof(SearchConfigDataProvider));
        }
        class SearchConfigDataProvider : FileDataProvider<int, SearchConfigData> {
            public SearchConfigDataProvider(Dictionary<string, object> options) : base(options) { }
            public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Dataset, SiteIdentity.ToString()); }
        }
    }
}
