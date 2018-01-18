/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System.Collections.Generic;
using System.IO;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Search.DataProvider {

    public class SearchConfigData {

        [Data_PrimaryKey]
        public int Id { get; set; }

        [Range(2,10)]
        public int SmallestMixedToken { get; set; }
        [Range(2, 10)]
        public int SmallestUpperCaseToken { get; set; }
        [StringLength(Globals.MaxUrl)]
        public string ResultsUrl { get; set; }
        [Range(1, 1000)]
        public int MaxResults { get; set; }
        [Data_NewValue("(0)")]
        public bool ShowUrl { get; set; }
        [Data_NewValue("(0)")]
        public bool ShowSummary { get; set; }

        public SearchConfigData() {
            SmallestMixedToken = 3;
            SmallestUpperCaseToken = 2;
            MaxResults = 20;
            ShowUrl = true;
            ShowSummary = true;
        }
    }

    public class SearchConfigDataProvider : DataProviderImpl, IInstallableModel {

        private const int KEY = 1;

        private static object _lockObject = new object();

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public SearchConfigDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public SearchConfigDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, SearchConfigData> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, SearchConfigData> CreateDataProvider() {
            Package package = YetaWF.Modules.Search.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Config",
                () => { // File
                    return new FileDataProvider<int, SearchConfigData>(
                        Path.Combine(YetaWFManager.DataFolder, Dataset, SiteIdentity.ToString()),
                        CurrentSiteIdentity: SiteIdentity,
                        Cacheable: true);
                },
                (dbo, conn) => {  // SQL
                    return new SQLSimpleObjectDataProvider<int, SearchConfigData>(Dataset, dbo, conn,
                        CurrentSiteIdentity: SiteIdentity,
                        Cacheable: true);
                },
                () => { // External
                    return MakeExternalDataProvider(new { Package = Package, Dataset = Dataset, CurrentSiteIdentity = SiteIdentity, Cacheable = true });
                }
            );
        }

        // API
        // API
        // API

        public static SearchConfigData GetConfig() {
            using (SearchConfigDataProvider configDP = new SearchConfigDataProvider()) {
                return configDP.GetItem();
            }
        }
        public SearchConfigData GetItem() {
            SearchConfigData config = DataProvider.Get(KEY);
            if (config == null) {
                lock (_lockObject) {
                    config = DataProvider.Get(KEY);
                    if (config == null) {
                        config = new SearchConfigData();
                        AddConfig(config);
                    }
                }
            }
            return config;
        }
        private void AddConfig(SearchConfigData data) {
            data.Id = KEY;
            if (!DataProvider.Add(data))
                throw new InternalError("Unexpected error adding settings");
        }
        public void UpdateConfig(SearchConfigData data) {
            data.Id = KEY;
            UpdateStatusEnum status = DataProvider.Update(data.Id, data.Id, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Unexpected error saving settings {0}", status);
        }
    }
}
