/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Identity.DataProvider;

namespace YetaWF.Modules.Identity.Modules {

    public class LoginConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, LoginConfigModule>, IInstallableModel { }

    [ModuleGuid("{dbc60a3f-eb01-4fe0-a0aa-4059200b1092}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    [ModuleCategory("Configuration")]
    public class LoginConfigModule : ModuleDefinition {

        public LoginConfigModule() {
            Title = this.__ResStr("modTitle", "User Login Settings");
            Name = this.__ResStr("modName", "User Login Settings");
            Description = this.__ResStr("modSummary", "Manages the site's user login settings and is used to define a site's settings, such as whether new users are accepted, new users, require verification, etc. The User Login Settings Module can be accessed using Admin > Settings > User Login Settings (standard YetaWF site).");
            ShowHelp = true;
            DefaultViewName = StandardViews.Config;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new LoginConfigModuleDataProvider(); }
        public override DataProviderImpl GetConfigDataProvider() { return new LoginConfigDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Config",
                LinkText = this.__ResStr("editLink", "User Login Settings"),
                MenuText = this.__ResStr("editText", "User Login Settings"),
                Tooltip = this.__ResStr("editTooltip", "Edit the user login settings"),
                Legend = this.__ResStr("editLegend", "Edits the user login settings"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}
