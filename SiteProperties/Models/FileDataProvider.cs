/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SiteProperties#License */

using System.Collections.Generic;
using System.IO;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Site;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.SiteProperties.DataProvider.File {

    public class FileDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(YetaWF.Modules.SiteProperties.Models.SiteDefinitionDataProvider), typeof(SiteDefinitionDataProvider));
        }
        class SiteDefinitionDataProvider : FileDataProvider<string, SiteDefinition> {
            public SiteDefinitionDataProvider(Dictionary<string, object> options) : base(options) { }
            public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Dataset); }
        }
    }
}
