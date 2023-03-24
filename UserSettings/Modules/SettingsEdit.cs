/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/UserSettings#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Language;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.UserSettings.DataProvider;

namespace YetaWF.Modules.UserSettings.Modules;

public class SettingsEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, SettingsEditModule>, IInstallableModel { }

[ModuleGuid("{4034971e-82c3-49de-9467-11219a8f61e3}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class SettingsEditModule : ModuleDefinition {

    public SettingsEditModule() : base() {
        Title = this.__ResStr("modTitle", "User Settings");
        Name = this.__ResStr("modName", "User Settings");
        Description = this.__ResStr("modSummary", "Edits the logged on user's settings, like desired date/time formats, time zone, language used and other options. The User Settings Module is accessible using User > Settings (standard YetaWF site).");
        DefaultViewName = StandardViews.EditApply;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new SettingsEditModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles {
        get {
            return new SerializableList<AllowedRole>() {
                new AllowedRole(Resource.ResourceAccess.GetUserRoleId(), AllowedEnum.Yes),
                new AllowedRole(Resource.ResourceAccess.GetEditorRoleId(), AllowedEnum.Yes, AllowedEnum.Yes, extra1: AllowedEnum.Yes),
                new AllowedRole(Resource.ResourceAccess.GetAdministratorRoleId(), AllowedEnum.Yes, AllowedEnum.Yes, AllowedEnum.Yes, AllowedEnum.Yes, AllowedEnum.Yes, AllowedEnum.Yes, AllowedEnum.Yes, AllowedEnum.Yes),
            };
        }
    }
    public override List<RoleDefinition> ExtraRoles {
        get {
            return new List<RoleDefinition>() {
                new RoleDefinition("Development Info",
                    this.__ResStr("roleUseDevC", "Use Development Settings"), this.__ResStr("roleUseDev", "The role has permission to use development settings"),
                    this.__ResStr("userUseDevC", "Use Development Settings"), this.__ResStr("userUseDev", "The user has permission to use development settings")),
            };
        }
    }

    public ModuleAction GetAction_Edit(string url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Edit",
            LinkText = this.__ResStr("editLink", "Edit Settings"),
            MenuText = this.__ResStr("editText", "Edit Settings"),
            Tooltip = this.__ResStr("editTooltip", "Edit your user settings, like date/time formats, language used and other options"),
            Legend = this.__ResStr("editLegend", "Edits your user settings, like date/time formats, language used and other options"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class EditModel {

        [UIHint("Hidden")]
        public int Key { get; set; }

        [Caption("Time Zone"), Description("Your time zone - all dates/times within this web site will be adjusted for the specified time zone")]
        [UIHint("TimeZone"), StringLength(UserData.MaxTimeZone), Required]
        public string? TimeZone { get; set; }

        [Caption("Date Format"), Description("The desired date format when dates are displayed on this website")]
        [UIHint("Enum"), Required]
        public Formatting.DateFormatEnum DateFormat { get; set; }

        [Caption("Time Format"), Description("The desired time format when times are displayed on this website")]
        [UIHint("Enum"), Required]
        public Formatting.TimeFormatEnum TimeFormat { get; set; }

        [Caption("Grid Actions"), Description("The desired display method for available actions in grids")]
        [UIHint("Enum")]
        public Grid.GridActionsUserEnum GridActionsUser { get; set; }

        public Grid.GridActionsEnum GridActions {
            // we're only showing a subset to the user
            get {
                return (Grid.GridActionsEnum)GridActionsUser;
            }
            set {
                try {
                    GridActionsUser = (Grid.GridActionsUserEnum)value;
                } catch (Exception) {
                    GridActionsUser = Grid.GridActionsUserEnum.DropdownMenu;
                }
            }
        }

        [Caption("Language"), Description("The default language used for the entire site (only used when localization is enabled)")]
        [UIHint("LanguageId"), StringLength(LanguageData.MaxId)]
        public string? LanguageId { get; set; }

        [Caption("Show Filter Toolbar"), Description("Defines whether the filter toolbar is always shown on grids - If not shown, it can still be accessed using the search button in each grid, at the bottom of the grid, next to the refresh button")]
        [UIHint("Boolean")]
        public bool ShowGridSearchToolbar { get; set; }

        [Caption("Show Page Ownership"), Description("Defines whether pages that can't be seen by anonymous users or regular users are shown with special background colors - Requires a skin that supports ownership display")]
        [UIHint("Boolean"), SuppressIf(nameof(ShowDevInfo), false)]
        public bool ShowPageOwnership { get; set; }

        [Caption("Show Module Ownership"), Description("Defines whether modules that can't be seen by anonymous users or regular users are shown with special background colors - Requires a skin that supports ownership display")]
        [UIHint("Boolean"), SuppressIf(nameof(ShowDevInfo), false)]
        public bool ShowModuleOwnership { get; set; }

        [Caption("Show Enum Values"), Description("Defines whether enumerated values (in dropdown lists) show their numeric value. Numeric values are typically only useful for programming purposes")]
        [UIHint("Boolean"), SuppressIf(nameof(ShowDevInfo), false)]
        public bool ShowEnumValue { get; set; }

        [Caption("Show Variables"), Description("Defines whether variable names are shown for properties and all available variables are listed on property pages. Variables are used for variable substitution in modules and pages and of course for programming purposes")]
        [UIHint("Boolean"), SuppressIf(nameof(ShowDevInfo), false)]
        public bool ShowVariables { get; set; }

        [Caption("Show Internal Data"), Description("Defines whether internal information is shown (e.g., ids)")]
        [UIHint("Boolean"), SuppressIf(nameof(ShowDevInfo), false)]
        public bool ShowInternals { get; set; }

        [Caption("Confirm Delete"), Description("Defines whether delete actions must be confirmed before items are deleted")]
        [UIHint("Boolean")]
        public bool ConfirmDelete { get; set; }

        [Caption("Confirm Actions"), Description("Defines whether actions must be confirmed before they are executed. This is normally used for actions that need a prompt but are not destructive (delete) in nature")]
        [UIHint("Boolean")]
        public bool ConfirmActions { get; set; }

        public bool ShowDevInfo { get; set; }

        public UserData GetData(UserData data) {
            ObjectSupport.CopyData(this, data);
            return data;
        }

        public void SetData(UserData data) {
            ObjectSupport.CopyData(data, this);
        }
        public EditModel() { }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        using (UserDataProvider dataProvider = new UserDataProvider()) {
            EditModel model = new EditModel { };
            model.ShowDevInfo = IsAuthorized("Development Info");
            UserData data = await dataProvider.GetItemAsync();
            model.SetData(data);
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(EditModel model) {
        using (UserDataProvider dataProvider = new UserDataProvider()) {
            UserData data = await dataProvider.GetItemAsync();
            model.ShowDevInfo = IsAuthorized("Development Info");
            if (!ModelState.IsValid)
                return await PartialViewAsync(model);
            data = model.GetData(data); // merge new data into original
            model.SetData(data); // and all the data back into model for final display
            await dataProvider.UpdateItemAsync(data);
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "Your settings have been successfully saved"), ForceRedirect: true);
        }
    }
}