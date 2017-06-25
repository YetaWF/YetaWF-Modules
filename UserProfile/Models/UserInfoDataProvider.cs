/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/UserProfile#License */

using System;
using System.Collections.Generic;
using System.IO;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;

namespace YetaWF.Modules.UserProfile.DataProvider {

    // The user's Country was incorrectly set to null when the user was in the US
    // When retrieving/updating records we're replacing null with United States so the country is always available

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
        public const int MaxState = 50;
        public const int MaxZip = 20;
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
                string addressType = CountryISO3166Helper.CountryToAddressType(Country);
                if (addressType == CountryISO3166Helper.Country.US)
                    return City + ", " + State + " " + Zip;
                else if (addressType == CountryISO3166Helper.Country.Zip1)
                    return Zip + " " + City;
                else if (addressType == CountryISO3166Helper.Country.ZipLast)
                    return City + " " + Zip;
#if DEBUG
                else if (addressType == "DE")
                    return Zip + " " + City;
#endif
                //else if (addressType == CountryISO3166Helper.Country.Generic)
                return City + " " + Zip;
            }
        }
        [Data_DontSave]
        public string Email { get; set; }
    }

    public class UserInfoDataProvider : DataProviderImpl, IInstallableModel, IRemoveUser {

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
            UserInfo userInfo = DataProvider.Get(key);
            if (userInfo == null) return null;
            if (string.IsNullOrWhiteSpace(userInfo.Country)) userInfo.Country = Globals.DefaultCountry;
            return userInfo;
        }
        public bool AddItem(UserInfo userInfo) {
            userInfo.Created = DateTime.UtcNow;
            if (string.IsNullOrWhiteSpace(userInfo.Country)) userInfo.Country = Globals.DefaultCountry;
            return DataProvider.Add(userInfo);
        }
        public UpdateStatusEnum UpdateItem(UserInfo userInfo) {
            userInfo.Updated = DateTime.UtcNow;
            if (string.IsNullOrWhiteSpace(userInfo.Country)) userInfo.Country = Globals.DefaultCountry;
            return DataProvider.Update(userInfo.UserId, userInfo.UserId, userInfo);
        }
        public bool RemoveItem(int key) {
            return DataProvider.Remove(key);
        }
        public List<UserInfo> GetItems(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, out int total) {
            List<UserInfo> list = DataProvider.GetRecords(skip, take, sort, filters, out total);
            foreach (UserInfo l in list) {
                if (string.IsNullOrWhiteSpace(l.Country)) l.Country = Globals.DefaultCountry;
            }
            return list;
        }
        public int RemoveItems(List<DataProviderFilterInfo> filters) {
            return DataProvider.RemoveRecords(filters);
        }

        // IREMOVEUSER
        // IREMOVEUSER
        // IREMOVEUSER

        public void Remove(int userId) {
            RemoveItem(userId);
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
