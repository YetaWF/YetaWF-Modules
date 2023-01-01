/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/UserSettings#License */

using System;
using System.Collections.Generic;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.UserSettings.Modules {

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
            return new ModuleAction(this) {
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
    }
}