/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System;
using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Messenger.DataProvider {

    public class Message {

        public const int MaxMessageText = 2000;

        [Data_PrimaryKey, Data_Identity]
        public int Key { get; set; }

        [Data_Index]
        public int FromUser { get; set; }
        [Data_Index]
        public int ToUser { get; set; }

        public DateTime Sent { get; set; }
        public bool Seen { get; set; }

        [StringLength(MaxMessageText)]
        public string MessageText { get; set; }

        public Message() {
            Sent = DateTime.UtcNow;
        }
    }

    public class MessagingDataProvider : DataProviderImpl, IInstallableModel {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public MessagingDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public MessagingDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProviderIdentity<int, object, int, Message> DataProvider { get { return GetDataProvider(); } }

        private IDataProviderIdentity<int, object, int, Message> CreateDataProvider() {
            Package package = YetaWF.Modules.Messenger.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package.AreaName,
                () => { // File
                    throw new InternalError("File I/O is not supported");
                },
                (dbo, conn) => {  // SQL
                    return new SQLIdentityObjectDataProvider<int, object, int, Message>(AreaName + "_Messaging", dbo, conn,
                        CurrentSiteIdentity: SiteIdentity,
                        Cacheable: true);
                },
                () => { // External
                    return MakeExternalDataProvider(new { AreaName = AreaName + "_Messaging", CurrentSiteIdentity = SiteIdentity, Cacheable = true });
                }
            );
        }

        // API
        // API
        // API

        public Message GetItem(int key) {
            return DataProvider.GetByIdentity(key);
        }
        public bool AddItem(Message data) {
            return DataProvider.Add(data);
        }
        public UpdateStatusEnum UpdateItem(Message data) {
            return DataProvider.UpdateByIdentity(data.Key, data);
        }
        public bool RemoveItem(int key) {
            return DataProvider.RemoveByIdentity(key);
        }
        public List<Message> GetItems(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, out int total) {
            return DataProvider.GetRecords(skip, take, sort, filters, out total);
        }
        public int RemoveItems(List<DataProviderFilterInfo> filters) {
            return DataProvider.RemoveRecords(filters);
        }
    }
}
