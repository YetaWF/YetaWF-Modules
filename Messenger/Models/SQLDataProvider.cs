/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL2;

namespace YetaWF.Modules.Messenger.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQL2Base.ExternalName, typeof(DataProvider.ConfigDataProvider), typeof(ConfigDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQL2Base.ExternalName, typeof(DataProvider.ConnectionDataProvider), typeof(ConnectionDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQL2Base.ExternalName, typeof(DataProvider.MessagingDataProvider), typeof(MessagingDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQL2Base.ExternalName, typeof(DataProvider.SiteAccouncementDataProvider), typeof(SiteAccouncementDataProvider));
        }
        class ConfigDataProvider : SQLSimpleObject<int, ConfigData> {
            public ConfigDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class ConnectionDataProvider : SQLSimpleObject<int, ConfigData> {
            public ConnectionDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class MessagingDataProvider : SQLSimple2Object<int, object, Message> {
            public MessagingDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class SiteAccouncementDataProvider : SQLSimple2Object<int, object, Message> {
            public SiteAccouncementDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
