using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateGridAjaxModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateGridAjaxModule>, IInstallableModel { }

    [ModuleGuid("{1316f4c8-a594-4831-b44d-1c7e925ac8a6}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateGridAjaxModule : ModuleDefinition {

        public TemplateGridAjaxModule() {
            Title = this.__ResStr("modTitle", "Grid (Ajax) Test Template");
            Name = this.__ResStr("modName", "Template Test - Grid (Ajax)");
            Description = this.__ResStr("modSummary", "Grid (Ajax) test template");
            DefaultViewName = StandardViews.Display;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateGridAjaxModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Grid (Ajax)"),
                MenuText = this.__ResStr("displayText", "Grid (Ajax)"),
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
