/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System;
using System.Collections.Generic;
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

        public SiteAccouncement GetItem(int key) {
            return DataProvider.GetByIdentity(key);
        }
        public bool AddItem(SiteAccouncement data) {
            return DataProvider.Add(data);
        }
        public UpdateStatusEnum UpdateItem(SiteAccouncement data) {
            return DataProvider.UpdateByIdentity(data.Key, data);
        }
        public bool RemoveItem(int key) {
            return DataProvider.RemoveByIdentity(key);
        }
        public List<SiteAccouncement> GetItems(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, out int total) {
            return DataProvider.GetRecords(skip, take, sort, filters, out total);
        }
        public int RemoveItems(List<DataProviderFilterInfo> filters) {
            return DataProvider.RemoveRecords(filters);
        }
    }
}
