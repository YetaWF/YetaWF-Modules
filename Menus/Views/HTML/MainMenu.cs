/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Menus;
using YetaWF.Modules.Menus.Modules;

namespace YetaWF.Modules.Menu.Views;

public class MainMenuView : YetaWFView, IYetaWFView<MainMenuModule, MainMenuModule.MenuModel> {

    public const string ViewName = "MainMenu";

    public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
    public override string GetViewName() { return ViewName; }

    public async Task<string> RenderViewAsync(MainMenuModule module, MainMenuModule.MenuModel model) {
        return await HtmlHelper.ForDisplayAsync(model, nameof(model.Menu));
    }
}
