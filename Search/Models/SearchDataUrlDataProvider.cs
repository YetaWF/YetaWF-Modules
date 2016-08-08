/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Search#License */

using System;
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

    public class SearchDataUrl {

        public const int MaxDescription = 100;

        [Data_Identity]
        public int SearchDataUrlId { get; set; }

        [Data_PrimaryKey, StringLength(Globals.MaxUrl)]
        public string PageUrl { get; set; }

        [StringLength(MaxDescription)]
        public string PageDescription { get; set; }

        public DateTime DatePageCreated { get; set; }
        public DateTime? DatePageUpdated { get; set; }

        public SearchDataUrl() { }
    }

    public class SearchDataUrlDataProvider : DataProviderImpl, IInstallableModel {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public SearchDataUrlDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(DataProvider); }
        public SearchDataUrlDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(DataProvider); }

        private IDataProviderIdentity<string, object, int, SearchDataUrl> DataProvider {
            get {
                if (_dataProvider == null) {
                    Package package = Package.GetPackageFromAssembly(GetType().Assembly);
                    switch (GetIOMode(package.AreaName + "_Urls")) {
                        default:
                        case WebConfigHelper.IOModeEnum.File:
                            if (SearchDataProvider.IsUsable)
                                throw new InternalError("File I/O is not supported");
                            break;
                        case WebConfigHelper.IOModeEnum.Sql:
                            using (SearchDataProvider searchDP = new SearchDataProvider()) {
                                if (SearchDataProvider.IsUsable)
                                    _dataProvider = new YetaWF.DataProvider.SQLIdentityObjectDataProvider<string, object, int, SearchDataUrl>(AreaName, SQLDbo, SQLConn,
                                        CurrentSiteIdentity: SiteIdentity,
                                        Cacheable: true);
                            }
                            break;
                    }
                }
                return _dataProvider;
            }
        }
        private IDataProviderIdentity<string, object, int, SearchDataUrl> _dataProvider { get; set; }

        // LOAD/SAVE
        // LOAD/SAVE
        // LOAD/SAVE

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

        public bool IsInstalled() {
            return DataProvider.IsInstalled();
        }
        public bool InstallModel(List<string> errorList) {
            if (!SearchDataProvider.IsUsable) return true;
            return DataProvider.InstallModel(errorList);
        }
        public bool UninstallModel(List<string> errorList) {
            if (!SearchDataProvider.IsUsable) return true;
            return DataProvider.UninstallModel(errorList);
        }
        public void AddSiteData() {
            if (!SearchDataProvider.IsUsable) return;
            DataProvider.AddSiteData();
        }
        public void RemoveSiteData() {
            if (!SearchDataProvider.IsUsable) return;
            DataProvider.RemoveSiteData();
        }
        public bool ExportChunk(int chunk, SerializableList<SerializableFile> fileList, out object obj) {
            // we don't export search data
            obj = null;
            return false;
            //if (!SearchDataProvider.IsUsable) return true;
            //return DataProvider.ExportChunk(chunk, fileList, out obj);
        }
        public void ImportChunk(int chunk, SerializableList<SerializableFile> fileList, object obj) {
            // we don't export search data
            return;
            //if (!SearchDataProvider.IsUsable) return;
            //DataProvider.ImportChunk(chunk, fileList, obj);
        }
    }
}
