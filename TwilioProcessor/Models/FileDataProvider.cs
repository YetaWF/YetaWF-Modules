/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TwilioProcessor#License */

using System.Collections.Generic;
using System.IO;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace Softelvdm.Modules.TwilioProcessor.DataProvider.File {

    public class FileDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.TwilioConfigDataProvider), typeof(TwilioConfigDataProvider));
        }
        class TwilioConfigDataProvider : FileDataProvider<int, TwilioData> {
            public TwilioConfigDataProvider(Dictionary<string, object> options) : base(options) { }
            public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Dataset, SiteIdentity.ToString()); }
        }
    }
}
