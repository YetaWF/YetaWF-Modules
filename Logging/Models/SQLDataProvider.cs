/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Logging#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL;

namespace YetaWF.Modules.Logging.DataProvider.SQL;

public class SQLDataProvider : IExternalDataProvider {

    public void Register() {
        DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.LoggingConfigDataProvider), typeof(LoggingConfigDataProvider));
    }
    class LoggingConfigDataProvider : SQLSimpleObject<int, LoggingConfigData> {
        public LoggingConfigDataProvider(Dictionary<string, object> options) : base(options) { }
    }
}
