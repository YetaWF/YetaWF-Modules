/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.Panels.Models;

namespace YetaWF.Modules.Panels.Modules {

    public class ModulePanelModuleDataProvider : ModuleDefinitionDataProvider<Guid, ModulePanelModule>, IInstallableModel { }

    [ModuleGuid("{c6129cf4-223d-4c12-82a7-beba2d2bbc22}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class ModulePanelModule : ModuleDefinition {

        public ModulePanelModule() {
            Title = this.__ResStr("modTitle", "Module Panel");
            Name = this.__ResStr("modName", "Module Panel");
            Description = this.__ResStr("modSummary", "Module Panel - used to display multiple modules using tabs or an accordion widget");
            PanelInfo = new Models.PanelInfo();
        }

        public override IModuleDefinitionIO GetDataProvider() { return new ModulePanelModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        [Caption("Panels"), Description("The panels displayed")] // no category so the property is not shown in property page
        [UIHint("YetaWF_Panels_PanelInfo")]
        public PanelInfo PanelInfo { get; set; }

        public override void ModuleSaving() {
            PanelInfo.Saving("PanelInfo", ModuleGuid); // update internal information
        }

        public ModuleAction GetAction_Display(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Module Panel"),
                MenuText = this.__ResStr("displayText", "Module Panel"),
                Tooltip = this.__ResStr("displayTooltip", "Display the Module Panel"),
                Legend = this.__ResStr("displayLegend", "Displays the Module Panel"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}
