/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Feedback#License */

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Feedback.DataProvider {
    public class FeedbackData {

        public const int MaxSubject = 100;
        public const int MaxMessage = 5000;

        [Data_Identity, Data_PrimaryKey]
        public int Key { get; set; }

        [StringLength(MaxSubject)]
        public string Subject { get; set; }
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
            Package package = YetaWF.Modules.Feedback.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package.AreaName,
                () => { // File
                    return new FileDataProvider<int, FeedbackData>(
                        Path.Combine(YetaWFManager.DataFolder, AreaName, SiteIdentity.ToString()),
                        CurrentSiteIdentity: SiteIdentity,
                        Cacheable: true);
                },
                (dbo, conn) => {  // SQL
                    return new SQLSimpleObjectDataProvider<int, FeedbackData>(AreaName, dbo, conn,
                        CurrentSiteIdentity: SiteIdentity,
                        Cacheable: true);
                },
                () => { // External
                    return MakeExternalDataProvider(new { AreaName = AreaName, CurrentSiteIdentity = SiteIdentity, Cacheable = true });
                }
            );
        }

        // API
        // API
        // API

        public FeedbackData GetItem(int key) {
            return DataProvider.Get(key);
        }
        public bool AddItem(FeedbackData data) {
            data.Created = DateTime.UtcNow;
            data.IPAddress = Manager.UserHostAddress;
            return DataProvider.Add(data);
        }
        public UpdateStatusEnum UpdateItem(FeedbackData data) {
            return DataProvider.Update(data.Key, data.Key, data);
        }
        public bool RemoveItem(int key) {
            return DataProvider.Remove(key);
        }

        public List<FeedbackData> GetItems(List<DataProviderFilterInfo> filters) {
            int total;
            return DataProvider.GetRecords(0, 0, null, filters, out total);
        }
        public List<FeedbackData> GetItems(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, out int total) {
            return DataProvider.GetRecords(skip, take, sort, filters, out total);
        }
        public int RemoveItems(List<DataProviderFilterInfo> filters) {
            return DataProvider.RemoveRecords(filters);
        }
    }
}
