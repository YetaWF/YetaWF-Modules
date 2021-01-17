/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Menus.Controllers;
using YetaWF.Modules.Menus.Modules;

namespace YetaWF.Modules.Menus.Views {

    public class MenuToggleView : YetaWFView, IYetaWFView<MenuToggleModule, MenuToggleModuleController.Model> {

        public const string ViewName = "MenuToggle";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public class Setup {
            public string ButtonId { get; set; }
            public int MaxWidth { get; set; }
            public string Target { get; set; } = null!;
        }

        public async Task<string> RenderViewAsync(MenuToggleModule module, MenuToggleModuleController.Model model) {

            await Manager.AddOnManager.AddAddOnNamedAsync(module.AreaName, ViewName);

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<button class='y_buttonlite' id='{ControlId}' style='display:none'>
    {SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-bars")}
</button>");

            string tags = hb.ToString();

            Setup setup = new Setup {
                ButtonId = ControlId,
                Target = module.Target,
            };
            Manager.ScriptManager.AddLast($@"new YetaWF_Menus.MenuToggleModule('{module.ModuleHtmlId}', {Utility.JsonSerialize(setup)});");

            return tags;
        }
    }
}
