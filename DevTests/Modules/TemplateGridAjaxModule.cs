/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

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
            Title = this.__ResStr("modTitle", "Grid (Ajax) Test Component");
            Name = this.__ResStr("modName", "Component Test - Grid (Ajax)");
            Description = this.__ResStr("modSummary", "Test module for the Grid (Ajax) component. A test page for this module can be found at Tests > Templates > Grid (Ajax) (standard YetaWF site).");
            DefaultViewName = StandardViews.Display;
            UsePartialFormCss = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateGridAjaxModuleDataProvider(); }

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
