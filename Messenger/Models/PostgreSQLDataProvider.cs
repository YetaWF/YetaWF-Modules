/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.PostgreSQL;

namespace YetaWF.Modules.Messenger.DataProvider.PostgreSQL {

    public class PostgreSQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.SiteAnnouncementDataProvider), typeof(SiteAnnouncementDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.ActiveUsersDataProvider), typeof(ActiveUsersDataProvider));
        }
        class SiteAnnouncementDataProvider : SQLSimpleIdentityObject<int, SiteAnnouncement> {
            public SiteAnnouncementDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class ActiveUsersDataProvider : SQLSimpleIdentityObject<int, ActiveUser> {
            public ActiveUsersDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
