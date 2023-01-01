/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TwilioProcessor#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL;

namespace Softelvdm.Modules.TwilioProcessor.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.TwilioConfigDataProvider), typeof(TwilioConfigDataProvider));
        }
        class TwilioConfigDataProvider : SQLSimpleObject<int, TwilioData> {
            public TwilioConfigDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
