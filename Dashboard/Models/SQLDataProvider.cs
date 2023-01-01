/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL;

namespace YetaWF.Modules.Dashboard.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.AuditInfoDataProvider), typeof(AuditInfoDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.AuditConfigDataProvider), typeof(AuditConfigDataProvider));
        }
        class AuditInfoDataProvider : SQLSimpleIdentityObject<int, AuditInfo> {
            public AuditInfoDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class AuditConfigDataProvider : SQLSimpleObject<int, AuditConfigData> {
            public AuditConfigDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
