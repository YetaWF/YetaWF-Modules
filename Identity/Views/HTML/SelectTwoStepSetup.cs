/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Identity.Modules;

namespace YetaWF.Modules.Identity.Views;

public class SelectTwoStepSetupView : YetaWFView, IYetaWFView<SelectTwoStepSetupModule, SelectTwoStepSetupModule.EditModel> {

    public const string ViewName = "SelectTwoStepSetup";

    public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
    public override string GetViewName() { return ViewName; }

    public async Task<string> RenderViewAsync(SelectTwoStepSetupModule module, SelectTwoStepSetupModule.EditModel model) {

        HtmlBuilder hb = new HtmlBuilder();

        hb.Append($@"
{await HtmlHelper.ForEditContainerAsync(model, "PropertyList")}");

        if (model.AuthMethods.Count == 0) {
            hb.Append($@"
<div class='yDivWarning'>
    {HE(this.__ResStr("noAuthInstalled", "There are no two-step authentication providers installed."))}
</div>");

        } else {
            foreach (SelectTwoStepSetupModule.EditModel.AuthMethod meth in model.AuthMethods) {

                hb.Append($@"
<div class='t_auth_info'>
    <div class='t_stat'>
        {await HtmlHelper.ForDisplayAsync(meth, nameof(meth.Enabled))}
    </div>
    <div class='t_auth'>
        {await HtmlHelper.ForDisplayAsync(meth, nameof(meth.Action))}
    </div>
</div>
<div class='t_auth_desc'>
    {HE(meth.Description)}
</div>
");

            }
        }
        return hb.ToString();
    }
}
