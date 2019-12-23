/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Visitors.Controllers;
using YetaWF.Modules.Visitors.Modules;

namespace YetaWF.Modules.Visitors.Views {

    public class SkinVisitorView : YetaWFView, IYetaWFView2<SkinVisitorModule, SkinVisitorModuleController.DisplayModel> {

        public const string ViewName = "SkinVisitor";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<string> RenderViewAsync(SkinVisitorModule module, SkinVisitorModuleController.DisplayModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
{await RenderBeginFormAsync(HtmlAttributes: new { data_track = model.TrackClickUrl })}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model), UsePartialFormCss: false)}
{await RenderEndFormAsync()}");
            return hb.ToString();
        }

        public Task<string> RenderPartialViewAsync(SkinVisitorModule module, SkinVisitorModuleController.DisplayModel model) {

            HtmlBuilder hb = new HtmlBuilder();
            // This is used so we get antiforgery fields
            return Task.FromResult(hb.ToString());

        }
    }
}
