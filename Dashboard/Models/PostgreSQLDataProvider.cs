/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.PostgreSQL;

namespace YetaWF.Modules.Dashboard.DataProvider.PostgreSQL {

    public class PostgreSQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(PostgreSQLBase.ExternalName, typeof(DataProvider.AuditInfoDataProvider), typeof(AuditInfoDataProvider));
            //$$$ DataProviderImpl.RegisterExternalDataProvider(PostgreSQLBase.ExternalName, typeof(DataProvider.AuditConfigDataProvider), typeof(AuditConfigDataProvider));
        }
        class AuditInfoDataProvider : PostgreSQLSimpleIdentityObject<int, AuditInfo> {
            public AuditInfoDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class AuditConfigDataProvider : PostgreSQLSimpleObject<int, AuditConfigData> {
            public AuditConfigDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
