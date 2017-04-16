/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/UserProfile#License */

using System;
using System.Collections.Generic;
using System.IO;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;

namespace YetaWF.Modules.UserProfile.DataProvider {

    public enum AddressTypeEnum {
        [EnumDescription("Domestic, US Address")]
        US = 0,
        [EnumDescription("International Address (includes Canada)")]
        International = 1,
    }

    public class UserInfo {

        public const int MaxName = 50;
        public const int MaxCompanyName = 50;
        public const int MaxAddress = 50;
        public const int MaxCity = 50;
        public const int MaxState = 2;
        public const int MaxZip = 10;
        public const int MaxCountry = 50;
        public const int MaxTelephone = 20;

        [Data_PrimaryKey, Data_Index]
        public int UserId { get; set; } // This is the site's userId

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        [StringLength(MaxName)]
        public string Name { get; set; }
        [StringLength(MaxCompanyName)]
        public string CompanyName { get; set; }
        [StringLength(MaxAddress)]
        public string Address1 { get; set; }
        [StringLength(MaxAddress)]
        public string Address2 { get; set; }
        [StringLength(MaxCity)]
        public string City { get; set; }
        [StringLength(MaxState)]
        public string State { get; set; }
        [StringLength(MaxZip)]
        public string Zip { get; set; }
        [StringLength(MaxCountry)]
        public string Country { get; set; }
        [StringLength(MaxTelephone)]
        public string Telephone { get; set; }

        public string CityCombined {
            get {
                if (string.IsNullOrWhiteSpace(Country))
                    return City + ", " + State + " " + Zip;
                else
                    return City;
            }
        }
        [Data_DontSave]
        public string Email { get; set; }
    }

    public class UserInfoDataProvider : DataProviderImpl, IInstallableModel {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public UserInfoDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(DataProvider); }
        public UserInfoDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(DataProvider); }

        private IDataProvider<int, UserInfo> DataProvider {
            get {
                if (_dataProvider == null) {
                    Package package = Package.GetPackageFromAssembly(GetType().Assembly);
                    switch (GetIOMode(package.AreaName)) {
                        default:
                        case WebConfigHelper.IOModeEnum.File:
                            _dataProvider = new YetaWF.DataProvider.FileDataProvider<int, UserInfo>(
                                Path.Combine(YetaWFManager.DataFolder, AreaName, SiteIdentity.ToString()),
                                Cacheable: true);
                            break;
                        case WebConfigHelper.IOModeEnum.Sql:
                            _dataProvider = new YetaWF.DataProvider.SQLSimpleObjectDataProvider<int, UserInfo>(AreaName, SQLDbo, SQLConn,
                                CurrentSiteIdentity: SiteIdentity,
                                Cacheable: true);
                            break;
                    }
                }
                return _dataProvider;
            }
        }
        private IDataProvider<int, UserInfo> _dataProvider { get; set; }

        // LOAD/SAVE
        // LOAD/SAVE
        // LOAD/SAVE

        public void DoAction(int key, Action action) {
            StringLocks.DoAction(LockKey(key), () => {
                action();
            });
        }
        private string LockKey(int key) {
            return string.Format("{0}_{1}", this.AreaName, key);
        }
        public UserInfo GetItem(int key) {
            return DataProvider.Get(key);
        }
        public bool AddItem(UserInfo data) {
            data.Created = DateTime.UtcNow;
            return DataProvider.Add(data);
        }
        public UpdateStatusEnum UpdateItem(UserInfo data) {
            data.Updated = DateTime.UtcNow;
            return DataProvider.Update(data.UserId, data.UserId, data);
        }
        public bool RemoveItem(int key) {
            return DataProvider.Remove(key);
        }
        public List<UserInfo> GetItems(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, out int total) {
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
