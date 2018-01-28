﻿/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL2;

namespace YetaWF.Modules.PageEdit.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQL2Base.ExternalName, typeof(DataProvider.ControlPanelConfigDataProvider), typeof(ControlPanelConfigDataProvider));
        }
        class ControlPanelConfigDataProvider : SQLSimpleObject<int, ControlPanelConfigData> {
            public ControlPanelConfigDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
