/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

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

    public class LogBrowseView : YetaWFView, IYetaWFView2<LogBrowseModule, LogBrowseModuleController.BrowseModel> {

        public const string ViewName = "LogBrowse";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(LogBrowseModule module, LogBrowseModuleController.BrowseModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            if (YetaWF.Core.Scheduler.SchedulerSupport.Enabled) {

                hb.Append($@"
<div class='{Globals.CssDivWarning}'>
    {HE(this.__ResStr("enabled", "The Scheduler is enabled."))}
</div>");

            } else {

                hb.Append($@"
<div class='{Globals.CssDivWarning}'>
    {HE(this.__ResStr("disabled", "The Scheduler is disabled."))}
</div>");

            }

            if (!model.LogAvailable) {

                hb.Append($@"
<div class='{Globals.CssDivAlert} t_browselog t_file'>
    {HE(this.__ResStr("fileNA", "The scheduler log is not available. Scheduler logging has not been started."))}");

                if (YetaWF.Core.Scheduler.SchedulerSupport.Enabled) {
                    hb.Append($@"
    {HE(this.__ResStr("fileNA2", "This may be caused by a missing log folder (\\Data\\DataFolder\\YetaWF_Scheduler) or due to permissions issues writing to the folder."))}");
                }
                hb.Append($@"
</div>");

            } else if (model.BrowsingSupported) {

                hb.Append($@"
{await RenderBeginFormAsync()}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
{await RenderEndFormAsync()}");

            } else {

                hb.Append($@"
<div class='{Globals.CssDivWarning} t_browselog t_file'>
    {HE(this.__ResStr("file", "The scheduler log is a flat file. Online browsing is not available."))}
</div>");
            }

            return hb.ToYHtmlString();
        }

        public async Task<YHtmlString> RenderPartialViewAsync(LogBrowseModule module, LogBrowseModuleController.BrowseModel model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append(await HtmlHelper.ForDisplayAsync(model, nameof(model.GridDef)));
            return hb.ToYHtmlString();

        }
    }
}
