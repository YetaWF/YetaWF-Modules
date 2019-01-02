/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Identity.Controllers;

namespace YetaWF.Modules.Identity.Views {

    public class TwoStepDoneView : YetaWFView, IYetaWFView<ModuleDefinition, object> {

        public const string ViewName = "TwoStepDone";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public Task<YHtmlString> RenderViewAsync(ModuleDefinition module, object model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"<script>window.close();</script>");

            return Task.FromResult(hb.ToYHtmlString());
        }
    }
}
