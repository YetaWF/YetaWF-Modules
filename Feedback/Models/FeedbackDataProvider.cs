/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Feedback#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Feedback.DataProvider {
    public class FeedbackData {

        public const int MaxSubject = 100;
        public const int MaxName = 80;
        public const int MaxMessage = 5000;

        [Data_Identity, Data_PrimaryKey]
        public int Key { get; set; }

        [StringLength(MaxSubject)]
        public string Subject { get; set; }
        [StringLength(MaxName)]
        [Data_NewValue]
        public string Name { get; set; }
        [StringLength(Globals.MaxEmail)]
        public string Email { get; set; }

        [StringLength(MaxMessage)]
        public string Message { get; set; }

        public DateTime Created { get; set; }
        [StringLength(Globals.MaxIP)]
        public string IPAddress { get; set; }

        public FeedbackData() { }
    }

    // SQL - no limit
    // File - keep only a very small number of feedback messages (~20)

    public class FeedbackDataProvider : DataProviderImpl, IInstallableModel {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public FeedbackDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public FeedbackDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, FeedbackData> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, FeedbackData> CreateDataProvider() {
            Package package = YetaWF.Modules.Feedback.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName, SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // API
        // API
        // API

        public Task<FeedbackData> GetItemAsync(int key) {
            return DataProvider.GetAsync(key);
        }
        public Task<bool> AddItemAsync(FeedbackData data) {
            data.Created = DateTime.UtcNow;
            data.IPAddress = Manager.UserHostAddress;
            return DataProvider.AddAsync(data);
        }
        public Task<UpdateStatusEnum> UpdateItemAsync(FeedbackData data) {
            return DataProvider.UpdateAsync(data.Key, data.Key, data);
        }
        public Task<bool> RemoveItemAsync(int key) {
            return DataProvider.RemoveAsync(key);
        }
        public Task<DataProviderGetRecords<FeedbackData>> GetItemsAsync(List<DataProviderFilterInfo> filters) {
            return DataProvider.GetRecordsAsync(0, 0, null, filters);
        }
        public Task<DataProviderGetRecords<FeedbackData>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            return DataProvider.GetRecordsAsync(skip, take, sort, filters);
        }
        public Task<int> RemoveItemsAsync(List<DataProviderFilterInfo> filters) {
            return DataProvider.RemoveRecordsAsync(filters);
        }
    }
}
