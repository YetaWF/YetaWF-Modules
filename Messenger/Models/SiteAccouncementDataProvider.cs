/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

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

    public class SiteAccouncement {

        public const int MaxMessage = 2000;
        public const int MaxTitle = 80;

        [Data_PrimaryKey, Data_Identity]
        public int Key { get; set; }

        public DateTime Sent { get; set; }

        [StringLength(MaxMessage)]
        public string Message { get; set; }
        [StringLength(MaxTitle)]
        public string Title { get; set; }

        public SiteAccouncement() {
            Sent = DateTime.UtcNow;
        }
    }

    public class SiteAccouncementDataProvider : DataProviderImpl, IInstallableModel {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public SiteAccouncementDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public SiteAccouncementDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProviderIdentity<int, object, SiteAccouncement> DataProvider { get { return GetDataProvider(); } }

        private IDataProviderIdentity<int, object, SiteAccouncement> CreateDataProvider() {
            Package package = YetaWF.Modules.Messenger.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Announcements", SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // API
        // API
        // API

        public async Task<SiteAccouncement> GetItemAsync(int key) {
            return await DataProvider.GetByIdentityAsync(key);
        }
        public async Task<bool> AddItemAsync(SiteAccouncement data) {
            return await DataProvider.AddAsync(data);
        }
        public async Task<UpdateStatusEnum> UpdateItemAsync(SiteAccouncement data) {
            return await DataProvider.UpdateByIdentityAsync(data.Key, data);
        }
        public async Task<bool> RemoveItemAsync(int key) {
            return await DataProvider.RemoveByIdentityAsync(key);
        }
        public async Task<DataProviderGetRecords<SiteAccouncement>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            return await DataProvider.GetRecordsAsync(skip, take, sort, filters);
        }
        public async Task<int> RemoveItemsAsync(List<DataProviderFilterInfo> filters) {
            return await DataProvider.RemoveRecordsAsync(filters);
        }
    }
}
