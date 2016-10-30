/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.IO;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Extensions;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.Controllers;

namespace YetaWF.Modules.Identity.DataProvider {

    public class LoginInfo {

        public const int MaxLoginProvider = 100;
        public const int MaxProviderKey = 100;

        [Data_Index, StringLength(Globals.MaxUser)]
        public int UserId { get; set; }

        [Data_PrimaryKey, StringLength(MaxLoginProvider + MaxProviderKey)]
        public string Key { get; set; }

        [Data_DontSave]
        public string LoginProvider { get { return Key.Substring(0, MaxLoginProvider).Trim(); } set { Key = MakeKey(value, ProviderKey); } }
        [Data_DontSave]
        public string ProviderKey {
            get {
                return Key.Length <= MaxLoginProvider ? "" : Key.Substring(MaxLoginProvider).Trim();
            }
            set {
                Key = MakeKey(LoginProvider, value);
            }
        }

        public static string MakeKey(string loginProvider, string providerKey) {
            return loginProvider.Truncate(MaxLoginProvider).PadRight(MaxLoginProvider) + providerKey.Truncate(MaxProviderKey).PadRight(MaxProviderKey);
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
    public class UserLoginInfoDataProvider : DataProviderImpl, IInstallableModel {

        static UserLoginInfoDataProvider() { }

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        private static object _lockObject = new object();

        public UserLoginInfoDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(DataProvider); }
        public UserLoginInfoDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(DataProvider); }

        protected IDataProvider<string, LoginInfo> DataProvider {
            get {
                if (_dataProvider == null) {
                    switch (GetIOMode(AreaRegistration.CurrentPackage.AreaName + "_LoginInfoList")) {
                        default:
                        case WebConfigHelper.IOModeEnum.File:
                            _dataProvider = new YetaWF.DataProvider.FileDataProvider<string, LoginInfo>(
                                Path.Combine(YetaWFManager.DataFolder, AreaName, SiteIdentity.ToString()),
                                CurrentSiteIdentity: SiteIdentity,
                                Cacheable: true);
                            break;
                        case WebConfigHelper.IOModeEnum.Sql:
                            _dataProvider = new YetaWF.DataProvider.SQLSimpleObjectDataProvider<string, LoginInfo>(AreaName, SQLDbo, SQLConn,
                                CurrentSiteIdentity: SiteIdentity,
                                NoLanguages: true,
                                Cacheable: true);
                            break;
                    }
                }
                return _dataProvider;
            }
        }
        private IDataProvider<string, LoginInfo> _dataProvider { get; set; }

        // LOAD/SAVE
        // LOAD/SAVE
        // LOAD/SAVE

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
            filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "UserId", Logic = "==", Value = userId });
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
