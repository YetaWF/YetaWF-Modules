/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Text#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Text.Modules;

namespace YetaWF.Modules.Text.Views;

public class TextDisplayView : YetaWFView, IYetaWFView<TextModule, TextModule.TextModelDisplay> {

    public const string ViewName = "TextDisplay";

    public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
    public override string GetViewName() { return ViewName; }

    public async Task<string> RenderViewAsync(TextModule module, TextModule.TextModelDisplay model) {
        return await HtmlHelper.ForDisplayAsync(model, nameof(model.Contents));
    }
}
