/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Feedback#License */

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

    public class FeedbackDataDataProvider : DataProviderImpl, IInstallableModel {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public FeedbackDataDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(DataProvider); }
        public FeedbackDataDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(DataProvider); }

        private IDataProvider<int, FeedbackData> DataProvider {
            get {
                if (_dataProvider == null) {
                    Package package = Package.GetPackageFromAssembly(GetType().Assembly);
                    switch (GetIOMode(package.AreaName)) {
                        default:
                        case WebConfigHelper.IOModeEnum.File:
                            _dataProvider = new YetaWF.DataProvider.FileDataProvider<int, FeedbackData>(
                                Path.Combine(YetaWFManager.DataFolder, AreaName, SiteIdentity.ToString()),
                                CurrentSiteIdentity: SiteIdentity,
                                Cacheable: true);
                            break;
                        case WebConfigHelper.IOModeEnum.Sql:
                            _dataProvider = new YetaWF.DataProvider.SQLSimpleObjectDataProvider<int, FeedbackData>(AreaName, SQLDbo, SQLConn,
                                CurrentSiteIdentity: SiteIdentity,
                                Cacheable: true);
                            break;
                    }
                }
                return _dataProvider;
            }
        }
        private IDataProvider<int, FeedbackData> _dataProvider { get; set; }

        // LOAD/SAVE
        // LOAD/SAVE
        // LOAD/SAVE

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

        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL

        public bool IsInstalled() {
            return DataProvider.IsInstalled();
        }
        public bool InstallModel(List<string> errorList) {
            return DataProvider.InstallModel(errorList);
        }
        public bool UninstallModel(List<string> errorList) {
            return DataProvider.UninstallModel(errorList);
        }
        public void AddSiteData() {
            DataProvider.AddSiteData();
        }
        public void RemoveSiteData() {
            DataProvider.RemoveSiteData();
        }
        public bool ExportChunk(int chunk, SerializableList<SerializableFile> fileList, out object obj) {
            return DataProvider.ExportChunk(chunk, fileList, out obj);
        }
        public void ImportChunk(int chunk, SerializableList<SerializableFile> fileList, object obj) {
            DataProvider.ImportChunk(chunk, fileList, obj);
        }
    }
}
