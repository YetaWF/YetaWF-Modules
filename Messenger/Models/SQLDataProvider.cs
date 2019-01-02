/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL;

namespace YetaWF.Modules.Messenger.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {

        public void Register() {
            //DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.ConfigDataProvider), typeof(ConfigDataProvider));
            //DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.ConnectionDataProvider), typeof(ConnectionDataProvider));
            //DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.MessagingDataProvider), typeof(MessagingDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.SiteAnnouncementDataProvider), typeof(SiteAnnouncementDataProvider));
        }
        //class ConfigDataProvider : SQLSimpleObject<int, ConfigData> {
        //    public ConfigDataProvider(Dictionary<string, object> options) : base(options) { }
        //}
        //class ConnectionDataProvider : SQLSimpleObject<string, Connection> {
        //    public ConnectionDataProvider(Dictionary<string, object> options) : base(options) { }
        //}
        //class MessagingDataProvider : SQLSimpleIdentityObject<int, Message> {
        //    public MessagingDataProvider(Dictionary<string, object> options) : base(options) { }
        //}
        class SiteAnnouncementDataProvider : SQLSimpleIdentityObject<int, SiteAnnouncement> {
            public SiteAnnouncementDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
