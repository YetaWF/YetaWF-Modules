/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/UserProfile#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
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

        public string CityCombined { get { return CountryISO3166Helper.CombineCityStateZip(Country, City, State, Zip); } }

        [Data_DontSave]
        public string Email { get; set; }
    }

    public class UserInfoDataProvider : DataProviderImpl, IInstallableModelAsync, IRemoveUser {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public UserInfoDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public UserInfoDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProviderAsync<int, UserInfo> DataProvider { get { return GetDataProvider(); } }

        private IDataProviderAsync<int, UserInfo> CreateDataProvider() {
            Package package = YetaWF.Modules.UserProfile.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName, SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // API
        // API
        // API

        public async Task<UserInfo> GetItemAsync(int key) {
            UserInfo userInfo = await DataProvider.GetAsync(key);
            if (userInfo == null) return null;
            if (string.IsNullOrWhiteSpace(userInfo.Country)) userInfo.Country = Globals.DefaultCountry;
            return userInfo;
        }
        public async Task<bool> AddItemAsync(UserInfo userInfo) {
            userInfo.Created = DateTime.UtcNow;
            if (string.IsNullOrWhiteSpace(userInfo.Country)) userInfo.Country = Globals.DefaultCountry;
            return await DataProvider.AddAsync(userInfo);
        }
        public async Task<UpdateStatusEnum> UpdateItemAsync(UserInfo userInfo) {
            userInfo.Updated = DateTime.UtcNow;
            if (string.IsNullOrWhiteSpace(userInfo.Country)) userInfo.Country = Globals.DefaultCountry;
            return await DataProvider.UpdateAsync(userInfo.UserId, userInfo.UserId, userInfo);
        }
        public async Task<bool> RemoveItemAsync(int key) {
            return await DataProvider.RemoveAsync(key);
        }
        public async Task<DataProviderGetRecords<UserInfo>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            DataProviderGetRecords<UserInfo> list = await DataProvider.GetRecordsAsync(skip, take, sort, filters);
            foreach (UserInfo l in list.Data) {
                if (string.IsNullOrWhiteSpace(l.Country)) l.Country = Globals.DefaultCountry;
            }
            return list;
        }
        public async Task<int> RemoveItemsAsync(List<DataProviderFilterInfo> filters) {
            return await DataProvider.RemoveRecordsAsync(filters);
        }

        // IREMOVEUSER
        // IREMOVEUSER
        // IREMOVEUSER

        public async Task RemoveAsync(int userId) {
            await RemoveItemAsync(userId);
        }
    }
}
