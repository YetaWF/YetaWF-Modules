/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System;
using System.Collections.Generic;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Search.DataProvider {

    public class SearchDataUrl {

        public const int MaxTitle = 100;
        public const int MaxSummary = 500;

        [Data_Identity]
        public int SearchDataUrlId { get; set; }

        [Data_PrimaryKey, StringLength(Globals.MaxUrl)]
        public string PageUrl { get; set; }
        [Data_NewValue("(0)")]
        public PageDefinition.PageSecurityType PageSecurity { get; set; }

        [StringLength(MaxTitle)]
        public string PageTitle { get; set; }
        [StringLength(MaxSummary)]
        public string PageSummary { get; set; }

        public DateTime DatePageCreated { get; set; }
        public DateTime? DatePageUpdated { get; set; }

        public SearchDataUrl() { }
    }

    public class SearchDataUrlDataProvider : DataProviderImpl, IInstallableModel {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public SearchDataUrlDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public SearchDataUrlDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProviderIdentity<string, object, int, SearchDataUrl> DataProvider { get { return GetDataProvider(); } }

        private IDataProviderIdentity<string, object, int, SearchDataUrl> CreateDataProvider() {
            if (SearchDataProvider.IsUsable) {
                Package package = YetaWF.Modules.Search.Controllers.AreaRegistration.CurrentPackage;
                return MakeDataProvider(package.AreaName + "_Urls",
                    () => { // File
                        throw new InternalError("File I/O is not supported");
                    },
                    (dbo, conn) => {  // SQL
                        return new SQLIdentityObjectDataProvider<string, object, int, SearchDataUrl>(AreaName, dbo, conn,
                            CurrentSiteIdentity: SiteIdentity,
                            Cacheable: true);
                    },
                    () => { // External
                        return MakeExternalDataProvider(new { AreaName = AreaName, CurrentSiteIdentity = SiteIdentity, Cacheable = true });
                    }
                );
            } else {
                return null;
            }
        }

        // API
        // API
        // API

        public SearchDataUrl GetItem(int id) {
            if (!SearchDataProvider.IsUsable) return null;
            return DataProvider.GetByIdentity(id);
        }
        internal SearchDataUrl GetItemByUrl(string pageUrl) {
            if (!SearchDataProvider.IsUsable) return null;
            List<DataProviderFilterInfo> filters = null;
            filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "PageUrl", Operator = "==", Value = pageUrl });
            SearchDataUrl searchUrl = DataProvider.GetOneRecord(filters);
            return searchUrl;
        }
        public bool AddItem(SearchDataUrl data) {
            if (!SearchDataProvider.IsUsable) return false;
            return DataProvider.Add(data);
        }
        public UpdateStatusEnum UpdateItem(SearchDataUrl data) {
            if (!SearchDataProvider.IsUsable) return UpdateStatusEnum.RecordDeleted;
            return DataProvider.UpdateByIdentity(data.SearchDataUrlId, data);
        }
        public int RemoveItems(List<DataProviderFilterInfo> filters) {
            if (!SearchDataProvider.IsUsable) return 0;
            return DataProvider.RemoveRecords(filters);
        }

        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL

        public new bool IsInstalled() {
            if (DataProvider == null) return false;
            return DataProvider.IsInstalled();
        }
        public new bool InstallModel(List<string> errorList) {
            if (!SearchDataProvider.IsUsable) return true;
            return DataProvider.InstallModel(errorList);
        }
        public new bool UninstallModel(List<string> errorList) {
            if (!SearchDataProvider.IsUsable) return true;
            return DataProvider.UninstallModel(errorList);
        }
        public new void AddSiteData() {
            if (!SearchDataProvider.IsUsable) return;
            DataProvider.AddSiteData();
        }
        public new void RemoveSiteData() {
            if (!SearchDataProvider.IsUsable) return;
            DataProvider.RemoveSiteData();
        }
        public new bool ExportChunk(int chunk, SerializableList<SerializableFile> fileList, out object obj) {
            // we don't export search data
            obj = null;
            return false;
            //if (!SearchDataProvider.IsUsable) return true;
            //return DataProvider.ExportChunk(chunk, fileList, out obj);
        }
        public new void ImportChunk(int chunk, SerializableList<SerializableFile> fileList, object obj) {
            // we don't export search data
            return;
            //if (!SearchDataProvider.IsUsable) return;
            //DataProvider.ImportChunk(chunk, fileList, obj);
        }
    }
}
