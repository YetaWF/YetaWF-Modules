/* Copyright � 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

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

        private IDataProviderIdentity<int, object, Message> DataProvider { get { return GetDataProvider(); } }

        private IDataProviderIdentity<int, object, Message> CreateDataProvider() {
            Package package = YetaWF.Modules.Messenger.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Messaging", SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // API
        // API
        // API

        public async Task<Message> GetItemAsync(int key) {
            return await DataProvider.GetByIdentityAsync(key);
        }
        public async Task<bool> AddItemAsync(Message data) {
            return await DataProvider.AddAsync(data);
        }
        public async Task<UpdateStatusEnum> UpdateItemAsync(Message data) {
            return await DataProvider.UpdateByIdentityAsync(data.Key, data);
        }
        public async Task<bool> RemoveItemAsync(int key) {
            return await DataProvider.RemoveByIdentityAsync(key);
        }
        public async Task<DataProviderGetRecords<Message>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            return await DataProvider.GetRecordsAsync(skip, take, sort, filters);
        }
        public async Task<int> RemoveItemsAsync(List<DataProviderFilterInfo> filters) {
            return await DataProvider.RemoveRecordsAsync(filters);
        }
    }
}
