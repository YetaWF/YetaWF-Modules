/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Views {

    public class ShowMessageView : YetaWFView, IYetaWFView<ModuleDefinition, string> {

        public const string ViewName = "ShowMessage";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public Task<YHtmlString> RenderViewAsync(ModuleDefinition module, string model) {

            HtmlBuilder hb = new HtmlBuilder();

            if (!string.IsNullOrWhiteSpace(model)) {
                 hb.Append($@"
<div class='{YetaWFManager.HtmlEncode(Globals.CssDivAlert)}'>
    {YetaWFManager.HtmlEncode(model)}
</div>");
            }
            return Task.FromResult(hb.ToYHtmlString());
        }
    }
}
