/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Visitors.Controllers;
using YetaWF.Modules.Visitors.Modules;

namespace YetaWF.Modules.Visitors.Views {

    public class TinyVisitorsView : YetaWFView, IYetaWFView<TinyVisitorsModule, TinyVisitorsModuleController.DisplayModel> {

        public const string ViewName = "TinyVisitors";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public Task<YHtmlString> RenderViewAsync(TinyVisitorsModule module, TinyVisitorsModuleController.DisplayModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div class='t_image'>");

            if (string.IsNullOrWhiteSpace(model.VisitorsUrl)) {

                hb.Append($@"
    <img src='{HAE(Manager.GetCDNUrl(model.ImageUrl))}' data-tooltip='{HAE(model.Tooltip)}' />");

            } else {

                hb.Append($@"
    <a href='{HAE(model.VisitorsUrl)}' class='{HAE(Basics.CssActionLink)}'>
        <img src='{HAE(Manager.GetCDNUrl(model.ImageUrl))}' data-tooltip='{HAE(model.Tooltip)}' alt='{HAE(this.__ResStr("visitors", "Visitors"))}' />
    </a>");

            }

            hb.Append($@"
</div>");

            return Task.FromResult(hb.ToYHtmlString());
        }
    }
}
