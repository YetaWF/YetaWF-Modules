/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Identity.Modules;

namespace YetaWF.Modules.Identity.Views;

public class Need2FADisplayView : YetaWFView, IYetaWFView<Need2FADisplayModule, Need2FADisplayModule.DisplayModel> {

    public const string ViewName = "Need2FADisplay";

    public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
    public override string GetViewName() { return ViewName; }

    public async Task<string> RenderViewAsync(Need2FADisplayModule module, Need2FADisplayModule.DisplayModel model) {

        HtmlBuilder hb = new HtmlBuilder();

        hb.Append($@"
<div class='t_container'>
    <div class='t_messagecontainer'>
        <div class='t_message'>
            {HE(this.__ResStr("need2FA", "Please set up Two-Step Authentication for full access to this site - "))}
            {await model.SetupAction.RenderAsLinkAsync()}
        </div>
    </div>
</div>");

        return hb.ToString();
    }
}
