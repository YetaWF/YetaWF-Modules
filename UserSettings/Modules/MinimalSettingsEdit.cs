/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/UserSettings#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.UserSettings.Modules {

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
            return new ModuleAction(this) {
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
                SaveReturnUrl = true,
            };
        }
    }
}