/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL;

namespace YetaWF.Modules.SyntaxHighlighter.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.ConfigDataProvider), typeof(ConfigDataProvider));
        }
        class ConfigDataProvider : SQLSimpleObject<int, ConfigData> {
            public ConfigDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
