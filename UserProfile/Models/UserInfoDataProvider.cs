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
using YetaWF.DataProvider;

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

        public string CityCombined { get { return CountryISO3166Helper.CombineCityStateZip(Country, City, State, Zip); } }

        [Data_DontSave]
        public string Email { get; set; }
    }

    public class UserInfoDataProvider : DataProviderImpl, IInstallableModel, IRemoveUser {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public UserInfoDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public UserInfoDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, UserInfo> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, UserInfo> CreateDataProvider() {
            Package package = YetaWF.Modules.UserProfile.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package.AreaName,
                () => { // File
                    return new FileDataProvider<int, UserInfo>(
                        Path.Combine(YetaWFManager.DataFolder, AreaName, SiteIdentity.ToString()),
                        Cacheable: true);
                },
                (dbo, conn) => {  // SQL
                    return new SQLSimpleObjectDataProvider<int, UserInfo>(AreaName, dbo, conn,
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
    }
}
