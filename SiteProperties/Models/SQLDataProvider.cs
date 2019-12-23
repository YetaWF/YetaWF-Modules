/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SiteProperties#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Site;
using YetaWF.DataProvider.SQL;

namespace YetaWF.Modules.SiteProperties.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(YetaWF.Modules.SiteProperties.Models.SiteDefinitionDataProvider), typeof(SiteDefinitionDataProvider));
        }
        class SiteDefinitionDataProvider : SQLSimpleObject<string, SiteDefinition> {
            public SiteDefinitionDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
