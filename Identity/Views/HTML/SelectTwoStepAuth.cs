/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Identity.Controllers;
using YetaWF.Modules.Identity.Modules;

namespace YetaWF.Modules.Identity.Views {

    public class SelectTwoStepAuthView : YetaWFView, IYetaWFView<SelectTwoStepAuthModule, SelectTwoStepAuthModuleController.EditModel> {

        public const string ViewName = "SelectTwoStepAuth";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(SelectTwoStepAuthModule module, SelectTwoStepAuthModuleController.EditModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
{await HtmlHelper.ForEditContainerAsync(model, "PropertyList")}");

            foreach (ModuleAction action in model.Actions) {
                hb.Append($@"
<div class='t_auth'>
    {await action.RenderAsLinkAsync()}
</div>");
            }

            return hb.ToYHtmlString();
        }
    }
}
