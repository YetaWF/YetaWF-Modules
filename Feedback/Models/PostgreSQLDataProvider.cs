/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Feedback#License */

using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.PostgreSQL;

namespace YetaWF.Modules.Feedback.DataProvider.PostgreSQL {

    public class SQLDataProvider : IExternalDataProvider {

        public void Register() {
            //$$ DataProviderImpl.RegisterExternalDataProvider(PostgreSQLBase.ExternalName, typeof(DataProvider.FeedbackConfigDataProvider), typeof(PostgreSQLSimpleObject<int, FeedbackConfigData>));
            DataProviderImpl.RegisterExternalDataProvider(PostgreSQLBase.ExternalName, typeof(DataProvider.FeedbackDataProvider), typeof(PostgreSQLSimpleObject<int, FeedbackData>));
        }
    }
}
