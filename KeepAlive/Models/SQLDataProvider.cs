/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/KeepAlive#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL2;

namespace YetaWF.Modules.KeepAlive.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.KeepAliveConfigDataProvider), typeof(KeepAliveConfigDataProvider));
        }
        class KeepAliveConfigDataProvider : SQLSimpleObject<int, KeepAliveConfigData> {
            public KeepAliveConfigDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
