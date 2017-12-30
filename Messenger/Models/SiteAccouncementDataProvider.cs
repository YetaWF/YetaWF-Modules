/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System;
using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
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

        public SiteAccouncementDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(DataProvider); }
        public SiteAccouncementDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(DataProvider); }

        private IDataProviderIdentity<int, object, int, SiteAccouncement> DataProvider {
            get {
                if (_dataProvider == null) {
                    Package package = Package.GetPackageFromAssembly(GetType().Assembly);
                    switch (GetIOMode(package.AreaName)) {
                        default:
                        case WebConfigHelper.IOModeEnum.File:
                            throw new InternalError("File I/O is not supported");
                        case WebConfigHelper.IOModeEnum.Sql:
                            _dataProvider = new YetaWF.DataProvider.SQLIdentityObjectDataProvider<int, object, int, SiteAccouncement>(AreaName + "_Announcements", SQLDbo, SQLConn,
                                CurrentSiteIdentity: SiteIdentity,
                                Cacheable: true);
                            break;
                    }
                }
                return _dataProvider;
            }
        }
        private IDataProviderIdentity<int, object, int, SiteAccouncement> _dataProvider { get; set; }

        // LOAD/SAVE
        // LOAD/SAVE
        // LOAD/SAVE
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
