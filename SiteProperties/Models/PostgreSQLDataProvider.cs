/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SiteProperties#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Site;
using YetaWF.DataProvider.PostgreSQL;

namespace YetaWF.Modules.SiteProperties.DataProvider.PostgreSQL {

    public class PostgreSQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(YetaWF.Modules.SiteProperties.Models.SiteDefinitionDataProvider), typeof(SiteDefinitionDataProvider));
        }
        class SiteDefinitionDataProvider : SQLSimpleObject<string, SiteDefinition> {
            public SiteDefinitionDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
