/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/UserSettings#License */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Language;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Site;
using YetaWF.Core.Support;

namespace YetaWF.Modules.UserSettings.DataProvider;

public class UserData {

    public const int MaxTimeZone = 40;

    [Data_PrimaryKey]
    public int Key { get; set; }

    // user options
    public Formatting.DateFormatEnum DateFormat { get; set; }
    [StringLength(MaxTimeZone)]
    public string TimeZone { get; set; }
    public Formatting.TimeFormatEnum TimeFormat { get; set; }
    [StringLength(SiteDefinition.MaxTheme)]
    [Data_NewValue]
    public string Theme { get; set; }
    [Data_NewValue]
    public Grid.GridActionsEnum GridActions { get; set; }
    [StringLength(LanguageData.MaxId)]
    public string LanguageId { get; set; }
    public bool ShowGridSearchToolbar { get; set; }
    [Data_NewValue]
    public bool ShowModuleOwnership { get; set; }
    [Data_NewValue]
    public bool ShowPageOwnership { get; set; }
    public bool ShowEnumValue { get; set; }
    public bool ShowVariables { get; set; }
    public bool ShowInternals { get; set; }
    public bool ConfirmDelete { get; set; }
    public bool ConfirmActions { get; set; }

    public Grid.GridActionsEnum DefaultGridActions {
        get {
            if (_DefaultGridActions == null)
                _DefaultGridActions = WebConfigHelper.GetValue(YetaWF.Modules.UserSettings.AreaRegistration.CurrentPackage.AreaName, "DefaultGridActions", Grid.GridActionsEnum.DropdownMenu);
            return (Grid.GridActionsEnum)_DefaultGridActions;
        }
    }
    private static Grid.GridActionsEnum? _DefaultGridActions;

    public bool DefaultShowGridSearchToolbar {
        get {
            //_ShowGridSearchToolbar ??= WebConfigHelper.GetValue(YetaWF.Modules.UserSettings.AreaRegistration.CurrentPackage.AreaName, "DefaultGridFilter", true);
            if (_ShowGridSearchToolbar == null)
                _ShowGridSearchToolbar = WebConfigHelper.GetValue(YetaWF.Modules.UserSettings.AreaRegistration.CurrentPackage.AreaName, "DefaultGridFilter", true);
            return (bool)_ShowGridSearchToolbar;
        }
    }
    private static bool? _ShowGridSearchToolbar;

    public UserData() {
        string timeZone = TimeZoneInfo.Local.Id;
        if (YetaWFManager.HaveManager && YetaWFManager.Manager.CurrentSite != null && !string.IsNullOrWhiteSpace(YetaWFManager.Manager.CurrentSite.TimeZone))
            timeZone = YetaWFManager.Manager.CurrentSite.TimeZone;
        string? theme = YetaWF.Core.Localize.UserSettings.GetProperty<string?>("Theme");
        if (theme == null && YetaWFManager.HaveManager && YetaWFManager.Manager.CurrentSite != null)
            theme = YetaWFManager.Manager.CurrentSite.Theme;
        DateFormat = Formatting.DateFormatEnum.MMDDYYYY;
        TimeFormat = Formatting.TimeFormatEnum.HHMMAM;
        Theme =  theme ?? SiteDefinition.DefaultTheme;
        LanguageId = MultiString.DefaultLanguage;
        TimeZone = timeZone;
        GridActions = DefaultGridActions;
        ShowGridSearchToolbar = DefaultShowGridSearchToolbar;
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

    public Task InitializeApplicationStartupAsync() {
        YetaWF.Core.Localize.UserSettings.UserSettingsAccess = (IUserSettings)this;
        return Task.CompletedTask;
    }

    // IUSERSETTINGS
    // IUSERSETTINGS
    // IUSERSETTINGS

    public async Task<object> ResolveUserAsync() {
        object? userInfo = Manager.UserSettingsObject;
        if (userInfo == null) {
            using (UserDataProvider userDP = new UserDataProvider()) {
                Manager.UserSettingsObject = userInfo = await userDP.GetItemAsync();
            }
        }
        return userInfo;
    }

    public object? GetProperty(string name, Type type) {
        // get the user settings
        object? userInfo = Manager.UserSettingsObject;
        if (userInfo == null) return null;
        // get the requested property
        PropertyInfo? pi = ObjectSupport.TryGetProperty(userInfo.GetType(), name);
        if (pi == null) throw new InternalError("User setting {0} doesn't exist", name);
        if (pi.PropertyType != type) throw new InternalError("User setting {0} is not of the requested type {1}", name, type.FullName);
        return pi.GetValue(userInfo);
    }

    public async Task SetPropertyAsync(string name, System.Type type, object? value) {
        // set the user settings
        UserData userInfo = (UserData)await ResolveUserAsync();
        // get the requested property
        PropertyInfo pi = ObjectSupport.GetProperty(userInfo.GetType(), name);
        if (pi == null) throw new InternalError("User setting {0} doesn't exist", name);
        if (pi.PropertyType != type) throw new InternalError("User setting {0} is not of the requested type {1}", name, type.FullName);
        pi.SetValue(userInfo, value);
        using (UserDataProvider userDP = new UserDataProvider()) {
            await userDP.UpdateItemAsync(userInfo);
        }
    }
}

public class UserDataProvider : DataProviderImpl, IInstallableModel, IRemoveUser {

    public const string KEY = "YetaWF_UserSettings_Settings";

    // IMPLEMENTATION
    // IMPLEMENTATION
    // IMPLEMENTATION

    public UserDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
    public UserDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

    private IDataProvider<int, UserData> DataProvider { get { return GetDataProvider(); } }

    private IDataProvider<int, UserData>? CreateDataProvider() {
        Package package = YetaWF.Modules.UserSettings.AreaRegistration.CurrentPackage;
        return MakeDataProvider(package, package.AreaName, SiteIdentity: SiteIdentity, Cacheable: true);
    }

    // API
    // API
    // API

    public async Task<UserData> GetItemAsync() {
        UserData? userInfo;
        if (Manager.HaveUser)
            userInfo = await DataProvider.GetAsync(Manager.UserId);
        else
            userInfo = Manager.SessionSettings.SiteSettings.GetValue<UserData>(KEY);
        if (userInfo == null)
            userInfo = new UserData();
        return userInfo;
    }
    public async Task UpdateItemAsync(UserData data) {
        if (Manager.HaveUser) {
            data.Key = Manager.UserId;
            UpdateStatusEnum status = await DataProvider.UpdateAsync(data.Key, data.Key, data);
            if (status == UpdateStatusEnum.RecordDeleted) {
                if (!await DataProvider.AddAsync(data))
                    throw new InternalError("Unexpected failure saving user settings", status);
            } else if (status != UpdateStatusEnum.OK)
                throw new InternalError("Unexpected status {0} updating user settings", status);
        } else {
            Manager.SessionSettings.SiteSettings.SetValue<UserData>(KEY, data);
            Manager.SessionSettings.SiteSettings.Save();
            Manager.UserSettingsObject = data;
        }
    }
    public async Task<bool> RemoveItemAsync() {
        if (Manager.SessionSettings.SiteSettings.ContainsKey(KEY))
            Manager.SessionSettings.SiteSettings[KEY].Remove();
        await DataProvider.RemoveAsync(Manager.UserId);
        return true;
    }

    public async Task<DataProviderGetRecords<UserData>> GetItemsAsync(List<DataProviderFilterInfo> filter) {
        return await DataProvider.GetRecordsAsync(0, 0, null, filter);
    }
    public async Task<DataProviderGetRecords<UserData>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
        return await DataProvider.GetRecordsAsync(skip, take, sort, filters);
    }
    public async Task<int> RemoveItemsAsync(List<DataProviderFilterInfo> filters) {
        return await DataProvider.RemoveRecordsAsync(filters);
    }

    // IREMOVEUSER
    // IREMOVEUSER
    // IREMOVEUSER

    public async Task RemoveAsync(int userId) {
        await DataProvider.RemoveAsync(userId);
    }
}
