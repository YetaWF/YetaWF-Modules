/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IFrame#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.IFrame.Controllers;
using YetaWF.Modules.IFrame.Modules;

namespace YetaWF.Modules.IFrame.Views {

    public class IFrameDisplayView : YetaWFView, IYetaWFView<IFrameDisplayModule, IFrameDisplayModuleController.DisplayModel> {

        public const string ViewName = "IFrameDisplay";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public Task<string> RenderViewAsync(IFrameDisplayModule module, IFrameDisplayModuleController.DisplayModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            if (Manager.EditMode) {

                hb.Append($@"
<p class='yDivWarning'>
    {HE(this.__ResStr("editMode", "Url contents({0}) not shown in Site Edit Mode", module.Url))}
</p>");

            } else {
                hb.Append($@"
<iframe src='{HAE(module.Url)}' style='border:none;{HAE(model.Style)}'></iframe>");
            }

            return Task.FromResult(hb.ToString());
        }
    }
}
