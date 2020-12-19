/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.PostgreSQL;

namespace YetaWF.Modules.PageEdit.DataProvider.PostgreSQL {

    public class PostgreSQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.ControlPanelConfigDataProvider), typeof(ControlPanelConfigDataProvider));
        }
        class ControlPanelConfigDataProvider : SQLSimpleObject<int, ControlPanelConfigData> {
            public ControlPanelConfigDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
