/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateGridModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateGridModule>, IInstallableModel { }

    [ModuleGuid("{8AA52B9A-7C5B-475d-8353-9D875CD75678}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateGridModule : ModuleDefinition {

        public TemplateGridModule() {
            Title = this.__ResStr("modTitle", "Grid (Static) Test Component");
            Name = this.__ResStr("modName", "Component Test - Grid (Static)");
            Description = this.__ResStr("modSummary", "Test module for the Grid (Static) component. A test page for this module can be found at Tests > Templates > Grid (Static) (standard YetaWF site).");
            DefaultViewName = StandardViews.Display;
            UsePartialFormCss = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateGridModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Grid (Static Data)"),
                MenuText = this.__ResStr("displayText", "Grid (Static Data)"),
                Tooltip = this.__ResStr("displayTooltip", "Display a sample grid"),
                Legend = this.__ResStr("displayLegend", "Displays a sample grid"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}
