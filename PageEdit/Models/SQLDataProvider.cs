/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL;

namespace YetaWF.Modules.PageEdit.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.ControlPanelConfigDataProvider), typeof(ControlPanelConfigDataProvider));
        }
        class ControlPanelConfigDataProvider : SQLSimpleObject<int, ControlPanelConfigData> {
            public ControlPanelConfigDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
