/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Site;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.Controllers;

namespace YetaWF.Modules.Identity.DataProvider {

    public class LoginInfo {

        public const int MaxLoginProvider = 100;
        public const int MaxProviderKey = 100;

        [Data_Index, StringLength(Globals.MaxUser)]
        public int UserId { get; set; }

        [Data_PrimaryKey, StringLength(MaxLoginProvider + 1 + MaxProviderKey)]
        public string Key { get; set; }

        [Data_DontSave]
        public string LoginProvider {
            get {
                string[] s = Key.Split(new char[] { '\x1' }, 2);
                if (s.Length == 0) return "";
                return s[0];
            }
            set {
                Key = MakeKey(value, ProviderKey);
            }
        }
        [Data_DontSave]
        public string ProviderKey {
            get {
                string[] s = Key.Split(new char[] { '\x1' }, 2);
                if (s.Length < 2) return "";
                return s[1];
            }
            set {
                Key = MakeKey(LoginProvider, value);
            }
        }

        [Obsolete("Discontinued as of package version 1.1.1")]
        [Data_DontSave]
        public string OldLoginProvider { get { return Key.Substring(0, MaxLoginProvider).Trim(); } }
        [Data_DontSave]
        [Obsolete("Discontinued as of package version 1.1.1")]
        public string OldProviderKey {
            get {
                return Key.Length <= MaxLoginProvider ? "" : Key.Substring(MaxLoginProvider).Trim();
            }
        }

        public static string MakeKey(string loginProvider, string providerKey) {
            return loginProvider.Trim() + "\x1" + providerKey.Trim();
        }
        public LoginInfo() {
            Key = "";
        }
    }

    /// <summary>
    /// UserDefinitionDataProvider
    /// Users are separated by site
    /// File - A small set of users is expected - all users are preloaded so less than 20 is recommended
    /// SQL - No limit
    /// </summary>
    public class UserLoginInfoDataProvider : DataProviderImpl, IInstallableModel, IInstallableModel2 {

        static UserLoginInfoDataProvider() { }

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public UserLoginInfoDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public UserLoginInfoDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<string, LoginInfo> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<string, LoginInfo> CreateDataProvider() {
            Package package = YetaWF.Modules.Identity.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_LoginInfoList", SiteIdentity: SiteIdentity, Cacheable: true, Parms: new { NoLanguages = true });
        }

        // API
        // API
        // API

        /// <summary>
        /// Returns the user id given a login provider and key.
        /// </summary>
        /// <param name="loginProvider"></param>
        /// <param name="providerKey"></param>
        /// <returns>The UserDefinition object or null if no such user exists.</returns>
        public async Task<UserDefinition> GetItemAsync(string loginProvider, string providerKey) {
            LoginInfo info = await DataProvider.GetAsync(LoginInfo.MakeKey(loginProvider, providerKey));
            if (info == null)
                return null;
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                return await userDP.GetItemAsync(info.UserId);
            }
        }
        /// <summary>
        /// Adds login provider and key for a specific user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loginProvider"></param>
        /// <param name="providerKey"></param>
        /// <returns></returns>
        public async Task<bool> AddItemAsync(int userId, string loginProvider, string providerKey) {
            return await DataProvider.AddAsync(new LoginInfo {
                UserId = userId,
                LoginProvider = loginProvider,
                ProviderKey = providerKey,
            });
        }
        /// <summary>
        /// Removes a known login provider and key.
        /// </summary>
        /// <param name="loginProvider"></param>
        /// <param name="providerKey"></param>
        /// <returns></returns>
        public async Task<bool> RemoveItemAsync(string loginProvider, string providerKey) {
            return await DataProvider.RemoveAsync(LoginInfo.MakeKey(loginProvider, providerKey));
        }
        /// <summary>
        /// Removes all login provider and key info for a specific user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> RemoveItemAsync(int userId) {
            List<DataProviderFilterInfo> filters = null;
            filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "UserId", Operator = "==", Value = userId });
            return await RemoveItemsAsync(filters) > 0;
        }
        /// <summary>
        /// Retrieves a list of login providers and keys
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="sort"></param>
        /// <param name="filters"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public async Task<DataProviderGetRecords<LoginInfo>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            return await DataProvider.GetRecordsAsync(skip, take, sort, filters);
        }
        /// <summary>
        /// Return whether the specified user id is an external user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> IsExternalUserAsync(int userId) {
            List<DataProviderFilterInfo> filters = null;
            filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "UserId", Operator = "==", Value = userId });
            DataProviderGetRecords<LoginInfo> logInfo = await GetItemsAsync(0, 0, null, filters);
            return logInfo.Data.Count > 0;
        }
        /// <summary>
        /// Removes login provider and key info.
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public async Task<int> RemoveItemsAsync(List<DataProviderFilterInfo> filters) {
            return await DataProvider.RemoveRecordsAsync(filters);
        }

        // IINSTALLABLEMODEL2
        // IINSTALLABLEMODEL2
        // IINSTALLABLEMODEL2

        public async Task<bool> UpgradeModelAsync(List<string> errorList, string lastSeenVersion) {

            // Convert pre 1.1.1 data to new format (for all sites)
            if (Package.CompareVersion(lastSeenVersion, AreaRegistration.CurrentPackage.Version) < 0 &&
                    Package.CompareVersion(lastSeenVersion, "1.1.1") < 0) {

                DataProviderGetRecords<SiteDefinition> info = await SiteDefinition.GetSitesAsync(0, 0, null, null);
                foreach (SiteDefinition site in info.Data) {
                    using (UserLoginInfoDataProvider userLoginInfoDP = new UserLoginInfoDataProvider(site.Identity)) {
                        const int chunk = 100;
                        int skip = 0;
                        for (; ; skip += chunk) {
                            DataProviderGetRecords<LoginInfo> list = await userLoginInfoDP.GetItemsAsync(skip, chunk, null, null);
                            if (list.Data.Count <= 0)
                                break;
                            foreach (LoginInfo l in list.Data) {
                                await RemoveItemAsync(l.UserId);
#pragma warning disable 0618 // Type or member is obsolete
                                await AddItemAsync(l.UserId, l.OldLoginProvider, l.OldProviderKey);
#pragma warning restore 0618 // Type or member is obsolete
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}
