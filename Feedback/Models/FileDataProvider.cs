/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Feedback#License */

using System.Collections.Generic;
using System.IO;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Feedback.DataProvider.File;

public class FileDataProvider : IExternalDataProvider {

    public void Register() {
        DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.FeedbackConfigDataProvider), typeof(FeedbackConfigDataProvider));
        DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.FeedbackDataProvider), typeof(FeedbackDataProvider));
    }
    class FeedbackConfigDataProvider : FileDataProvider<int, FeedbackConfigData> {
        public FeedbackConfigDataProvider(Dictionary<string, object> options) : base(options) { }
        public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Dataset, SiteIdentity.ToString()); }
    }
    class FeedbackDataProvider : FileDataProvider<int, FeedbackData> {
        public FeedbackDataProvider(Dictionary<string, object> options) : base(options) { }
        public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Dataset, SiteIdentity.ToString()); }
    }
}
