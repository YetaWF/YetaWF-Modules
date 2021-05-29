using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateModuleActionsModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateModuleActionsModule>, IInstallableModel { }

    [ModuleGuid("{6dc99bd1-73b2-4780-9455-f040bf48bde6}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateModuleActionsModule : ModuleDefinition {

        public TemplateModuleActionsModule() {
            Title = this.__ResStr("modTitle", "ModuleActions Test Template");
            Name = this.__ResStr("modName", "Template Test - ModuleActions");
            Description = this.__ResStr("modSummary", "ModuleActions test template");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateModuleActionsModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_User() {
            return new ModuleAction(this) {
                Url = "/User",
                Image = "#Display",
                LinkText = this.__ResStr("userLink", "User"),
                MenuText = this.__ResStr("userText", "User"),
                Tooltip = this.__ResStr("userTooltip", "Display user account page"),
                Legend = this.__ResStr("userLegend", "Displays the user account page"),
                Style = ModuleAction.ActionStyleEnum.NewWindow,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = false,
            };
        }
        public ModuleAction GetAction_Dashboard() {
            return new ModuleAction(this) {
                Url = "/Admin/Bar/Dashboard",
                Image = "#Display",
                LinkText = this.__ResStr("dashboardLink", "Dashboard"),
                MenuText = this.__ResStr("dashboardText", "Dashboard"),
                Tooltip = this.__ResStr("dashboardTooltip", "Display administrator dashboard page"),
                Legend = this.__ResStr("dashboardLegend", "Displays the administrator dashboard page"),
                Style = ModuleAction.ActionStyleEnum.NewWindow,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = false,
            };
        }
    }
}
