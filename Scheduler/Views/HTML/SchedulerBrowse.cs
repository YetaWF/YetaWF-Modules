/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Scheduler.Controllers;
using YetaWF.Modules.Scheduler.Modules;

namespace YetaWF.Modules.Scheduler.Views {

    public class SchedulerBrowseView : YetaWFView, IYetaWFView2<SchedulerBrowseModule, SchedulerBrowseModuleController.SchedulerBrowseModel> {

        public const string ViewName = "SchedulerBrowse";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<string> RenderViewAsync(SchedulerBrowseModule module, SchedulerBrowseModuleController.SchedulerBrowseModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            if (YetaWF.Core.Scheduler.SchedulerSupport.Enabled) {

//                hb.Append($@"
//<div class='{Globals.CssDivWarning}'>
//    {HE(this.__ResStr("enabled", "The Scheduler is enabled."))}
//</div>");

            } else {

                hb.Append($@"
<div class='{Globals.CssDivWarning}'>
    {HE(this.__ResStr("disabled", "The Scheduler is disabled."))}
</div>");

            }

            hb.Append($@"
{await RenderBeginFormAsync()}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
{await RenderEndFormAsync()}");

            return hb.ToString();
        }

        public async Task<string> RenderPartialViewAsync(SchedulerBrowseModule module, SchedulerBrowseModuleController.SchedulerBrowseModel model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append(await HtmlHelper.ForDisplayAsync(model, nameof(model.GridDef)));
            return hb.ToString();

        }
    }
}
