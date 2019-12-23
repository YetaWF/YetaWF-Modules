/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SitePropertiesService#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Site;
using YetaWF.DataProvider.SQL;

namespace YetaWF.Modules.SitePropertiesService.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(YetaWF.Modules.SitePropertiesService.Models.SiteDefinitionDataProvider), typeof(SiteDefinitionDataProvider));
        }
        class SiteDefinitionDataProvider : SQLSimpleObject<string, SiteDefinition> {
            public SiteDefinitionDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
