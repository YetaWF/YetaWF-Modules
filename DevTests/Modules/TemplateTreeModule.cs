/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateTreeModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateTreeModule>, IInstallableModel { }

    [ModuleGuid("{A1952569-E77D-40fb-8C4A-2A1412EB03E2}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateTreeModule : ModuleDefinition {

        public TemplateTreeModule() {
            Title = this.__ResStr("modTitle", "Tree (Static) Test Component");
            Name = this.__ResStr("modName", "Component Test - Tree");
            Description = this.__ResStr("modSummary", "Test module for the Tree component. A test page for this module can be found at Tests > Templates > Tree (standard YetaWF site).");
            DefaultViewName = StandardViews.Display;
            UsePartialFormCss = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateTreeModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Tree"),
                MenuText = this.__ResStr("displayText", "Tree"),
                Tooltip = this.__ResStr("displayTooltip", "Display a sample tree"),
                Legend = this.__ResStr("displayLegend", "Displays a sample tree"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}
