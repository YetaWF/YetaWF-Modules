/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/UserProfile#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.UserProfile.Modules {

    public class ProfileEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, ProfileEditModule>, IInstallableModel { }

    [ModuleGuid("{9ba8e8dc-7e04-492c-850d-27f0ca6fa2d3}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class ProfileEditModule : ModuleDefinition {

        public ProfileEditModule() {
            Title = this.__ResStr("modTitle", "User Profile");
            Name = this.__ResStr("modName", "Edit User Profile");
            Description = this.__ResStr("modSummary", "Edits an existing user profile");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new ProfileEditModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Edit",
                LinkText = this.__ResStr("editLink", "Profile"),
                MenuText = this.__ResStr("editText", "Profile"),
                Tooltip = this.__ResStr("editTooltip", "Edit your user profile"),
                Legend = this.__ResStr("editLegend", "Edits your user profile"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}
