using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Visitors.Controllers;
using YetaWF.Modules.Visitors.Modules;

namespace YetaWF.Modules.Visitors.Views {

    public class VisitorsUnavailableView : YetaWFView, IYetaWFView<VisitorsModule, object> {

        public const string ViewName = "VisitorsUnavailable";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public Task<YHtmlString> RenderViewAsync(VisitorsModule module, object model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div class='{Globals.CssDivWarning}'>
    {this.__ResStr("unavailable", "Visitor information is not available - See <a href='https://yetawf.com/Documentation/YetaWF/Visitors' target='_blank' rel='noopener noreferrer'>https://yetawf.com/Documentation/YetaWF/Visitors</a> for additional information.")}
</div>");

            return Task.FromResult(hb.ToYHtmlString());
        }
    }
}
