/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateScrollerModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateScrollerModule>, IInstallableModel { }

    [ModuleGuid("{4f4d9110-369c-441d-bf55-adc210fa4bc0}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateScrollerModule : ModuleDefinition {

        public TemplateScrollerModule() {
            Title = this.__ResStr("modTitle", "Scroller Test Component");
            Name = this.__ResStr("modName", "Component Test - Scroller");
            Description = this.__ResStr("modSummary", "Scroller test component");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateScrollerModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Scroller"),
                MenuText = this.__ResStr("displayText", "Scroller"),
                Tooltip = this.__ResStr("displayTooltip", "Display the Scroller test"),
                Legend = this.__ResStr("displayLegend", "Displays the Scroller test"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}
