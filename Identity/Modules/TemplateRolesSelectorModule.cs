/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Identity.Modules {

    public class TemplateRolesSelectorModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateRolesSelectorModule>, IInstallableModel { }

    [ModuleGuid("{3dc50ef9-ca0f-4a57-9f56-7bbec59f303b}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateRolesSelectorModule : ModuleDefinition {

        public TemplateRolesSelectorModule() {
            Title = this.__ResStr("modTitle", "RolesSelector Test Template");
            Name = this.__ResStr("modName", "Template Test - RolesSelector");
            Description = this.__ResStr("modSummary", "RolesSelector test template");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateRolesSelectorModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "RolesSelector"),
                MenuText = this.__ResStr("displayText", "RolesSelector"),
                Tooltip = this.__ResStr("displayTooltip", "Display the RolesSelector test template"),
                Legend = this.__ResStr("displayLegend", "Displays the RolesSelector test template"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}
