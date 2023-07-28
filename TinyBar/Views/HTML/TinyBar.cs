/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TinyBar#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.TinyBar.Modules;

namespace YetaWF.Modules.TinyBar.Views;

public class TinyBarView : YetaWFView, IYetaWFView<TinyBarModule, TinyBarModule.TinyBarModel> {

    public const string ViewName = "TinyBar";

    public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
    public override string GetViewName() { return ViewName; }

    public Task<string> RenderViewAsync(TinyBarModule module, TinyBarModule.TinyBarModel model) {
        return HtmlHelper.ForDisplayAsync(model, nameof(model.MenuData), UIHint: "Menu");
    }
}
