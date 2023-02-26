/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Text#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Text.Modules;

namespace YetaWF.Modules.Text.Views;

public class MarkdownDisplayView : YetaWFView, IYetaWFView<MarkdownModule, MarkdownModule.ModelDisplay> {

    public const string ViewName = "MarkdownDisplay";

    public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
    public override string GetViewName() { return ViewName; }

    public Task<string> RenderViewAsync(MarkdownModule module, MarkdownModule.ModelDisplay model) {
        return Task.FromResult(model.Contents.HTML ?? string.Empty);
    }
}
