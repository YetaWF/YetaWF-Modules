/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/UserSettings#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.Endpoints.Support;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Site;
using YetaWF.DataProvider;
using YetaWF.Modules.UserSettings.DataProvider;

namespace YetaWF.Modules.UserSettings.Modules;

public class MinimalSettingsEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, MinimalSettingsEditModule>, IInstallableModel { }

[ModuleGuid("{0513D232-F4B1-4a17-A71E-01F7C1ED674C}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class MinimalSettingsEditModule : ModuleDefinition {

    public MinimalSettingsEditModule() : base() {
        Title = this.__ResStr("modTitle", "User Settings");
        Name = this.__ResStr("modName", "User Settings (Minimal)");
        Description = this.__ResStr("modSummary", "Edits the logged on user's settings, like desired date/time formats, time zone.");
        DefaultViewName = StandardViews.EditApply;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new MinimalSettingsEditModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Edit(string url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Edit",
            LinkText = this.__ResStr("editLink", "Edit Settings"),
            MenuText = this.__ResStr("editText", "Edit Settings"),
            Tooltip = this.__ResStr("editTooltip", "Edit your user settings (date/time formats)"),
            Legend = this.__ResStr("editLegend", "Edits your user settings (date/time formats)"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
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

        [Caption("Theme"), Description("The theme used for all pages of the site")]
        [UIHint("Theme"), StringLength(SiteDefinition.MaxTheme), SelectionRequired]
        public string? Theme { get; set; }

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
            UserData data = await dataProvider.GetItemAsync();
            model.SetData(data);
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(EditModel model) {
        using (UserDataProvider dataProvider = new UserDataProvider()) {
            UserData data = await dataProvider.GetItemAsync();
            if (!ModelState.IsValid)
                return await PartialViewAsync(model);
            data = model.GetData(data); // merge new data into original
            model.SetData(data); // and all the data back into model for final display
            await dataProvider.UpdateItemAsync(data);
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "Your settings have been successfully saved"), ForceReload: true);
        }
    }
}