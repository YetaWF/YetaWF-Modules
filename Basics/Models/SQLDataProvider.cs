/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

using System.Collections.Generic;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL;

namespace YetaWF.Modules.Basics.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.AlertConfigDataProvider), typeof(AlertConfigDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.RecaptchaV2ConfigDataProvider), typeof(RecaptchaV2ConfigDataProvider));
        }
        class AlertConfigDataProvider : SQLSimpleObject<int, AlertConfig> {
            public AlertConfigDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class RecaptchaV2ConfigDataProvider : SQLSimpleObject<int, RecaptchaV2Config> {
            public RecaptchaV2ConfigDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
