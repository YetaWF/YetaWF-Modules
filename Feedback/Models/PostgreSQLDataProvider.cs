/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Feedback#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.PostgreSQL;

namespace YetaWF.Modules.Feedback.DataProvider.PostgreSQL {

    public class PostgreSQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(PostgreSQLBase.ExternalName, typeof(DataProvider.FeedbackConfigDataProvider), typeof(FeedbackConfigDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(PostgreSQLBase.ExternalName, typeof(DataProvider.FeedbackDataProvider), typeof(FeedbackDataProvider));
        }
        class FeedbackConfigDataProvider : PostgreSQLSimpleObject<int, FeedbackConfigData> {
            public FeedbackConfigDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class FeedbackDataProvider : PostgreSQLSimpleObject<int, FeedbackData> {
            public FeedbackDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
