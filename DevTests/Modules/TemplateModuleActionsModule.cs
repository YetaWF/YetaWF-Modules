using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Endpoints.Support;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
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
            return new ModuleAction() {
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
            return new ModuleAction() {
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

        [Trim]
        public class Model {

            [Caption("NormalMenu"), Description("ModuleActions (NormalMenu)")]
            [UIHint("ModuleActions"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.NormalMenu), ReadOnly]
            public List<ModuleAction> PropNormalMenu { get; set; }

            [Caption("NormalLinks"), Description("ModuleActions (NormalLinks)")]
            [UIHint("ModuleActions"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.NormalLinks), ReadOnly]
            public List<ModuleAction> PropNormalLinks { get; set; }

            [Caption("IconsOnly"), Description("ModuleActions (IconsOnly)")]
            [UIHint("ModuleActions"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.IconsOnly), ReadOnly]
            public List<ModuleAction> PropIconsOnly { get; set; }

            [Caption("LinksOnly"), Description("ModuleActions (LinksOnly)")]
            [UIHint("ModuleActions"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.LinksOnly), ReadOnly]
            public List<ModuleAction> PropLinksOnly { get; set; }

            [Caption("Button"), Description("ModuleActions (Button)")]
            [UIHint("ModuleActions"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.Button), ReadOnly]
            public List<ModuleAction> PropButton { get; set; }

            [Caption("ButtonIcon"), Description("ModuleActions (ButtonIcon)")]
            [UIHint("ModuleActions"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.ButtonIcon), ReadOnly]
            public List<ModuleAction> PropButtonIcon { get; set; }

            [Caption("ButtonOnly"), Description("ModuleActions (ButtonOnly)")]
            [UIHint("ModuleActions"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.ButtonOnly), ReadOnly]
            public List<ModuleAction> PropButtonOnly { get; set; }

            [Caption("ButtonBar"), Description("ModuleActions (ButtonBar)")]
            [UIHint("ModuleActions"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.ButtonBar), ReadOnly]
            public List<ModuleAction> PropButtonBar { get; set; }

            [Caption("ButtonDropDown"), Description("ModuleActions (ButtonDropDown)")]
            [UIHint("ModuleActions"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.ButtonDropDown), ReadOnly]
            public List<ModuleAction> PropButtonDropDown { get; set; }
            public string PropButtonDropDown_Label { get { return "Select"; } }

            [Caption("ButtonMiniDropDown"), Description("ModuleActions (ButtonMiniDropDown)")]
            [UIHint("ModuleActions"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.ButtonMiniDropDown), ReadOnly]
            public List<ModuleAction> PropButtonMiniDropDown { get; set; }

            public Model() {
                TemplateModuleActionsModule mod = new TemplateModuleActionsModule();
                List<ModuleAction> actions = new List<ModuleAction>();
                ModuleAction action = mod.GetAction_Dashboard();
                action.CssClass = CssManager.CombineCss(action.CssClass, "y_success");
                actions.New(action);
                action = mod.GetAction_User();
                action.CssClass = CssManager.CombineCss(action.CssClass, "y_warning");
                actions.New(action);
                action = mod.GetAction_Dashboard();
                action.CssClass = CssManager.CombineCss(action.CssClass, "y_danger");
                actions.New(action);
                actions.New(mod.GetAction_User());

                PropNormalMenu = actions;
                PropNormalLinks = actions;
                PropIconsOnly = actions;
                PropLinksOnly = actions;
                PropButton = actions;
                PropButtonIcon = actions;
                PropButtonOnly = actions;
                PropButtonBar = actions;
                PropButtonDropDown = actions;
                PropButtonDropDown = actions;
                PropButtonMiniDropDown = actions;
            }
        }

        public Task<ActionInfo> RenderModuleAsync() {
            Model model = new Model();
            return RenderAsync(model);
        }

        public Task<IResult> UpdateModuleAsync(Model model) {
            if (!ModelState.IsValid)
                return PartialViewAsync(model);
            return FormProcessedAsync(model, this.__ResStr("ok", "OK"));
        }
    }
}
