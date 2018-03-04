/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Feedback#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL2;

namespace YetaWF.Modules.Feedback.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.FeedbackConfigDataProvider), typeof(FeedbackConfigDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.FeedbackDataProvider), typeof(FeedbackDataProvider));
        }
        class FeedbackConfigDataProvider : SQLSimpleObject<int, FeedbackConfigData> {
            public FeedbackConfigDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class FeedbackDataProvider : SQLSimpleObject<int, FeedbackData> {
            public FeedbackDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
