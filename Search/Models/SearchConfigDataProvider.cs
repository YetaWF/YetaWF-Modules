/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System.Collections.Generic;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;

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
            return MakeDataProvider(package, package.AreaName + "_Config", SiteIdentity: SiteIdentity, Cacheable: true);
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


        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL

        public new bool IsInstalled() {
            if (DataProvider == null) return false;
            return base.IsInstalled();
        }
        public new bool InstallModel(List<string> errorList) {
            if (DataProvider == null) return true;
            return base.InstallModel(errorList);
        }
        public new bool UninstallModel(List<string> errorList) {
            if (DataProvider == null) return true;
            return base.UninstallModel(errorList);
        }
        public new void AddSiteData() {
            if (DataProvider == null) return;
            base.AddSiteData();
        }
        public new void RemoveSiteData() {
            if (DataProvider == null) return;
            base.RemoveSiteData();
        }
        public new bool ExportChunk(int chunk, SerializableList<SerializableFile> fileList, out object obj) {
            if (DataProvider == null) {
                obj = null;
                return false;
            }
            return base.ExportChunk(chunk, fileList, out obj);
        }
        public new void ImportChunk(int chunk, SerializableList<SerializableFile> fileList, object obj) {
            if (DataProvider == null) return;
            base.ImportChunk(chunk, fileList, obj);
        }
    }
}
