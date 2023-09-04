/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Languages.Modules;

namespace YetaWF.Modules.Languages.Views;

public class TranslationWarningView : YetaWFView, IYetaWFView<TranslationWarningModule, TranslationWarningModule.DisplayModel> {

    public const string ViewName = "TranslationWarning";

    public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
    public override string GetViewName() { return ViewName; }

    public Task<string> RenderViewAsync(TranslationWarningModule module, TranslationWarningModule.DisplayModel model) {

        HtmlBuilder hb = new HtmlBuilder();
        hb.Append($@"
<div class='t_container'>
    <div class='t_messagecontainer'>
        <div class='t_message'>
            {HE(module.Warning)}
        </div>
    </div>
</div>");
        return Task.FromResult(hb.ToString());
    }
}
