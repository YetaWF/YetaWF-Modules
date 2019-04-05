/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Dashboard.Controllers;
using YetaWF.Modules.Dashboard.Modules;

namespace YetaWF.Modules.Dashboard.Views {

    public class StaticPagesBrowseView : YetaWFView, IYetaWFView2<StaticPagesBrowseModule, StaticPagesBrowseModuleController.BrowseModel> {

        public const string ViewName = "StaticPagesBrowse";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<string> RenderViewAsync(StaticPagesBrowseModule module, StaticPagesBrowseModuleController.BrowseModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            if (!Manager.CurrentSite.StaticPages) {
                hb.Append($@"
<div class='{Globals.CssDivWarning}'>
    {YetaWFManager.HtmlEncode(this.__ResStr("disabled", "Static pages are disabled for this site."))}
</div>");
            }

                hb.Append($@"
{await RenderBeginFormAsync()}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
{await RenderEndFormAsync()}");

            return hb.ToString();
        }

        public async Task<string> RenderPartialViewAsync(StaticPagesBrowseModule module, StaticPagesBrowseModuleController.BrowseModel model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append($@"
{this.__ResStr("explain", "<p>Static pages are fast loading pages whose content typically doesn't change. The list below shows all currently loaded static pages. There may be additional pages which were defined as static pages (in their Page Settings) which haven't been loaded yet.</p><p>Static pages are enabled per page (Page Settings) and site wide using Site Settings.</p>")}
{await HtmlHelper.ForDisplayAsync(model, nameof(model.GridDef))}");

            return hb.ToString();

        }
    }
}
