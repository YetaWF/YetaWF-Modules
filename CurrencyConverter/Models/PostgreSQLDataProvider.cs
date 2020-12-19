/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/CurrencyConverter#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.PostgreSQL;

namespace YetaWF.Modules.CurrencyConverter.DataProvider.PostgreSQL {

    public class PostgreSQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.ConfigDataProvider), typeof(ConfigDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.ExchangeRateDataProvider), typeof(ExchangeRateDataProvider));
        }
        class ConfigDataProvider : SQLSimpleObject<int, ConfigData> {
            public ConfigDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class ExchangeRateDataProvider : SQLSimpleObject<int, ExchangeRateData> {
            public ExchangeRateDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
