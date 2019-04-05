/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Menus.Controllers;
using YetaWF.Modules.Menus.Modules;

namespace YetaWF.Modules.Logging.Views {

    public class MainMenuView : YetaWFView, IYetaWFView<MainMenuModule, MainMenuModuleController.MenuModel> {

        public const string ViewName = "MainMenu";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<string> RenderViewAsync(MainMenuModule module, MainMenuModuleController.MenuModel model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append($@"{await HtmlHelper.ForDisplayAsync(model, nameof(model.Menu))}");
            return hb.ToString();
        }
    }
}
