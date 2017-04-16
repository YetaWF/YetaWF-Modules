/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/UserSettings#License */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Language;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;

namespace YetaWF.Modules.UserSettings.DataProvider {
    public class UserData {

        public const int MaxTimeZone = 40;

        [Data_PrimaryKey]
        public int Key { get; set; }

        // user options
        public Formatting.DateFormatEnum DateFormat { get; set; }
        [StringLength(MaxTimeZone)]
        public string TimeZone { get; set; }
        public Formatting.TimeFormatEnum TimeFormat { get; set; }
        [StringLength(LanguageData.MaxId)]
        public string LanguageId { get; set; }
        public bool ShowGridSearchToolbar { get; set; }
        [Data_NewValue("(0)")]
        public bool ShowModuleOwnership { get; set; }
        [Data_NewValue("(0)")]
        public bool ShowPageOwnership { get; set; }
        public bool ShowEnumValue { get; set; }
        public bool ShowVariables { get; set; }
        public bool ShowInternals { get; set; }
        public bool ConfirmDelete { get; set; }
        public bool ConfirmActions { get; set; }

        public UserData() {
            DateFormat = Formatting.DateFormatEnum.MMDDYYYY;
            TimeFormat = Formatting.TimeFormatEnum.HHMMAM;
            LanguageId = MultiString.DefaultLanguage;
            TimeZone = TimeZoneInfo.Local.Id;
            ShowGridSearchToolbar = false;
            ShowModuleOwnership = false;
            ShowPageOwnership = false;
            ShowEnumValue = false;
            ShowVariables = false;
            ShowInternals = false;
            ConfirmDelete = true;
            ConfirmActions = true;
        }
    }

    public class UserSettingsAccess : IInitializeApplicationStartup, IUserSettings {

        protected YetaWFManager Manager { get { return YetaWFManager.Manager; } }

        // STARTUP
        // STARTUP
        // STARTUP

        public void InitializeApplicationStartup() {
            YetaWF.Core.Localize.UserSettings.UserSettingsAccess = (IUserSettings)this;
        }

        // IUSERSETTINGS
        // IUSERSETTINGS
        // IUSERSETTINGS

        public object GetProperty(string name, System.Type type) {
            // get the user settings (for anonymous users create one)
            object userInfo = Manager.UserSettingsObject;
            if (userInfo == null) {
                using (UserDataProvider userDP = new UserDataProvider()) {
                    Manager.UserSettingsObject = userInfo = userDP.GetItem();
                }
            }
            // get the requested property
            PropertyInfo pi = ObjectSupport.TryGetProperty(userInfo.GetType(), name);
            if (pi == null) throw new InternalError("User setting {0} doesn't exist", name);
            if (pi.PropertyType != type) throw new InternalError("User setting {0} is not of the requested type {1}", name, type.FullName);
            return pi.GetValue(userInfo);
        }

        public void SetProperty(string name, System.Type type, object value) {
            // set the user settings (for anonymous users create one)
            UserData userInfo = (UserData)Manager.UserSettingsObject;
            if (userInfo == null) {
                using (UserDataProvider userDP = new UserDataProvider()) {
                    Manager.UserSettingsObject = userInfo = userDP.GetItem();
                }
            }
            // get the requested property
            PropertyInfo pi = ObjectSupport.GetProperty(userInfo.GetType(), name);
            if (pi == null) throw new InternalError("User setting {0} doesn't exist", name);
            if (pi.PropertyType != type) throw new InternalError("User setting {0} is not of the requested type {1}", name, type.FullName);
            pi.SetValue(userInfo, value);
            using (UserDataProvider userDP = new UserDataProvider()) {
                userDP.UpdateItem(userInfo);
            }
        }
    }

    public class UserDataProvider : DataProviderImpl, IInstallableModel {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public UserDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(DataProvider); }
        public UserDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(DataProvider); }

        public const string KEY = "YetaWF_UserSettings_Settings";

        private IDataProvider<int, UserData> DataProvider {
            get {
                if (_dataProvider == null) {
                    Package package = Package.GetPackageFromAssembly(GetType().Assembly);
                    switch (GetIOMode(package.AreaName)) {
                        default:
                        case WebConfigHelper.IOModeEnum.File:
                            _dataProvider = new YetaWF.DataProvider.FileDataProvider<int, UserData>(
                                Path.Combine(YetaWFManager.DataFolder, AreaName, SiteIdentity.ToString()),
                                CurrentSiteIdentity: SiteIdentity,
                                Cacheable: true);
                            break;
                        case WebConfigHelper.IOModeEnum.Sql:
                            _dataProvider = new YetaWF.DataProvider.SQLSimpleObjectDataProvider<int, UserData>(AreaName, SQLDbo, SQLConn,
                                CurrentSiteIdentity: SiteIdentity,
                                Cacheable: true);
                            break;
                    }
                }
                return _dataProvider;
            }
        }
        private IDataProvider<int, UserData> _dataProvider { get; set; }

        // LOAD/SAVE
        // LOAD/SAVE
        // LOAD/SAVE

        public UserData GetItem() {
            UserData userInfo = null;
            if (Manager.HaveUser)
                 userInfo = DataProvider.Get(Manager.UserId);
            else
                userInfo = Manager.SessionSettings.SiteSettings.GetValue<UserData>(KEY);
            if (userInfo == null)
                userInfo = new UserData();
            return userInfo;
        }
        public void UpdateItem(UserData data) {
            if (Manager.HaveUser) {
                data.Key = Manager.UserId;
                UpdateStatusEnum status = DataProvider.Update(data.Key, data.Key, data);
                if (status == UpdateStatusEnum.RecordDeleted) {
                    if (!DataProvider.Add(data))
                        throw new InternalError("Unexpected failure saving user settings", status);
                } else if (status != UpdateStatusEnum.OK)
                    throw new InternalError("Unexpected status {0} updating user settings", status);
            } else {
                Manager.SessionSettings.SiteSettings.SetValue<UserData>(KEY, data);
                Manager.SessionSettings.SiteSettings.Save();
            }
        }
        public bool RemoveItem() {
            if (Manager.SessionSettings.SiteSettings.ContainsKey(KEY))
                Manager.SessionSettings.SiteSettings[KEY].Remove();
            DataProvider.Remove(Manager.UserId);
            return true;
        }

        public List<UserData> GetItems(List<DataProviderFilterInfo> filter) {
            int total;
            return DataProvider.GetRecords(0, 0, null, filter, out total);
        }
        public List<UserData> GetItems(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, out int total) {
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
