/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SiteProperties#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.SiteProperties.Controllers;
using YetaWF.Modules.SiteProperties.Modules;

namespace YetaWF.Modules.SiteProperties.Views {

    public class LockedStatusView : YetaWFView, IYetaWFView<LockedStatusModule, LockedStatusModuleController.Model> {

        public const string ViewName = "LockedStatus";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public Task<string> RenderViewAsync(LockedStatusModule module, LockedStatusModuleController.Model model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div class='{Globals.CssDivAlert}'>
    {HE(this.__ResStr("locked", "Site is locked!"))}
</div>");

            return Task.FromResult(hb.ToString());
        }
    }
}
