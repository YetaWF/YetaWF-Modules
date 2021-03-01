using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using YetaWF.Core.Modules;
using YetaWF.Modules.DevTests.Modules;
using YetaWF.Core.Support;
using YetaWF.Core.Pages;

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplateModuleActionsModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplateModuleActionsModule> {

        public TemplateModuleActionsModuleController() { }

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

            [Caption("ButtonMiniDropDown"), Description("ModuleActions (ButtonMiniDropDown)")]
            [UIHint("ModuleActions"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.ButtonMiniDropDown), ReadOnly]
            public List<ModuleAction> PropButtonMiniDropDown { get; set; }

            public Model() { }

            public void Update() {
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

        [AllowGet]
        public ActionResult TemplateModuleActions() {
            Model model = new Model();
            model.Update();
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplateModuleActions_Partial(Model model) {
            model.Update();
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }
    }
}
