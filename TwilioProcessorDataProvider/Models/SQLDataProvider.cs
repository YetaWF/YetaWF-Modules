/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TwilioProcessorDataProvider#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL;

namespace Softelvdm.Modules.TwilioProcessorDataProvider.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.TwilioConfigDataProvider), typeof(TwilioConfigDataProvider));
        }
        class TwilioConfigDataProvider : SQLSimpleObject<int, TwilioData> {
            public TwilioConfigDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
