/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Collections.Generic;
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

        private static object _lockObject = new object();

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public UserLoginInfoDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public UserLoginInfoDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<string, LoginInfo> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<string, LoginInfo> CreateDataProvider() {
            Package package = YetaWF.Modules.Identity.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider2(package, package.AreaName + "_LoginInfoList", SiteIdentity: SiteIdentity, Cacheable: true, Parms: new { NoLanguages = true });
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
        public UserDefinition GetItem(string loginProvider, string providerKey) {
            LoginInfo info = DataProvider.Get(LoginInfo.MakeKey(loginProvider, providerKey));
            if (info == null)
                return null;
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                return userDP.GetItem(info.UserId);
            }
        }
        /// <summary>
        /// Adds login provider and key for a specific user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loginProvider"></param>
        /// <param name="providerKey"></param>
        /// <returns></returns>
        public bool AddItem(int userId, string loginProvider, string providerKey) {
            return DataProvider.Add(new LoginInfo {
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
        public bool RemoveItem(string loginProvider, string providerKey) {
            return DataProvider.Remove(LoginInfo.MakeKey(loginProvider, providerKey));
        }
        /// <summary>
        /// Removes all login provider and key info for a specific user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool RemoveItem(int userId) {
            List<DataProviderFilterInfo> filters = null;
            filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "UserId", Operator = "==", Value = userId });
            return RemoveItems(filters) > 0;
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
        public List<LoginInfo> GetItems(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, out int total) {
            return DataProvider.GetRecords(skip, take, sort, filters, out total);
        }
        /// <summary>
        /// Return whether the specified user id is an external user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsExternalUser(int userId) {
            List<DataProviderFilterInfo> filters = null;
            filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "UserId", Operator = "==", Value = userId });
            int total;
            List<LoginInfo> logInfo = GetItems(0, 0, null, filters, out total);
            return logInfo.Count > 0;
        }
        /// <summary>
        /// Removes login provider and key info.
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public int RemoveItems(List<DataProviderFilterInfo> filters) {
            return DataProvider.RemoveRecords(filters);
        }

        // IINSTALLABLEMODEL2
        // IINSTALLABLEMODEL2
        // IINSTALLABLEMODEL2

        public bool UpgradeModel(List<string> errorList, string lastSeenVersion) {

            // Convert pre 1.1.1 data to new format (for all sites)
            if (Package.CompareVersion(lastSeenVersion, AreaRegistration.CurrentPackage.Version) < 0 &&
                    Package.CompareVersion(lastSeenVersion, "1.1.1") < 0) {

                SiteDefinition.SitesInfo info = SiteDefinition.GetSites(0,0,null,null);
                foreach (SiteDefinition site in info.Sites) {
                    using (UserLoginInfoDataProvider userLoginInfoDP = new UserLoginInfoDataProvider(site.Identity)) {
                        const int chunk = 100;
                        int skip = 0;
                        for ( ; ; skip += chunk) {
                            int total;
                            List<LoginInfo> list = userLoginInfoDP.GetItems(skip, chunk, null, null, out total);
                            if (list.Count <= 0)
                                break;
                            foreach (LoginInfo l in list) {
                                RemoveItem(l.UserId);
#pragma warning disable 0618 // Type or member is obsolete
                                AddItem(l.UserId, l.OldLoginProvider, l.OldProviderKey);
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
