/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Menus.Modules;

namespace YetaWF.Modules.Menus.Views;

public class MenuView : YetaWFView, IYetaWFView<MenuModule, MenuModule.MenuModel> {

    public const string ViewName = "Menu";

    public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
    public override string GetViewName() { return ViewName; }

    public async Task<string> RenderViewAsync(MenuModule module, MenuModule.MenuModel model) {
        return await HtmlHelper.ForDisplayAsync(model, nameof(model.Menu));
    }
}
