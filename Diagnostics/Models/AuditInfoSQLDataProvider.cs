using System;
using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL;

namespace YetaWF.Modules.Diagnostics.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.AuditInfoDataProvider), typeof(AuditInfoDataProvider));
        }
        class AuditInfoDataProvider : SQLSimpleIdentityObject<int, AuditInfo> {
            public AuditInfoDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
