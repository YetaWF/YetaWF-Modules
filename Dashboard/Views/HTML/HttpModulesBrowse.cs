/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

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

    public class HttpModulesBrowseView : YetaWFView, IYetaWFView2<HttpModulesBrowseModule, HttpModulesBrowseModuleController.BrowseModel> {

        public const string ViewName = "HttpModulesBrowse";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<string> RenderViewAsync(HttpModulesBrowseModule module, HttpModulesBrowseModuleController.BrowseModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            if (Utility.AspNetMvc == Utility.AspNetMvcVersion.MVC5) {

                hb.Append($@"
{await RenderBeginFormAsync()}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
{await RenderEndFormAsync()}");

            } else {
                hb.Append($@"
    <div class='{Globals.CssDivWarning}'>
        {Utility.HtmlEncode(this.__ResStr("notAvail", "Information not available on ASP.NET Core MVC"))}
    </div>");
            }

            return hb.ToString();
        }

        public async Task<string> RenderPartialViewAsync(HttpModulesBrowseModule module, HttpModulesBrowseModuleController.BrowseModel model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append($@"
{await HtmlHelper.ForDisplayAsync(model, nameof(model.GridDef))}");

            return hb.ToString();

        }
    }
}
