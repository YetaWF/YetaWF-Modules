/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
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

            // icon used: fas-bars
            hb.Append($@"
<button class='y_button y_button_outline' id='{ControlId}' style='display:none'>
    <svg aria-hidden='true' focusable='false' role='img' viewBox='0 0 448 512'>
        <path fill='currentColor' d='M16 132h416c8.837 0 16-7.163 16-16V76c0-8.837-7.163-16-16-16H16C7.163 60 0 67.163 0 76v40c0 8.837 7.163 16 16 16zm0 160h416c8.837 0 16-7.163 16-16v-40c0-8.837-7.163-16-16-16H16c-8.837 0-16 7.163-16 16v40c0 8.837 7.163 16 16 16zm0 160h416c8.837 0 16-7.163 16-16v-40c0-8.837-7.163-16-16-16H16c-8.837 0-16 7.163-16 16v40c0 8.837 7.163 16 16 16z' class=''></path>
    </svg>
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
