/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Dashboard.Controllers;
using YetaWF.Modules.Dashboard.Modules;

namespace YetaWF.Modules.Dashboard.Views {

    public class DisposableTrackerBrowseView : YetaWFView, IYetaWFView2<DisposableTrackerBrowseModule, DisposableTrackerBrowseModuleController.BrowseModel> {

        public const string ViewName = "DisposableTrackerBrowse";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<string> RenderViewAsync(DisposableTrackerBrowseModule module, DisposableTrackerBrowseModuleController.BrowseModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
{await RenderBeginFormAsync()}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
{await RenderEndFormAsync()}");

            return hb.ToString();
        }

        public async Task<string> RenderPartialViewAsync(DisposableTrackerBrowseModule module, DisposableTrackerBrowseModuleController.BrowseModel model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append($@"
{this.__ResStr("explain", "<p>Disposable objects implement the IDisposable interface. Their creation/destruction is tracked using the DisposableTracker class. If objects are shown they should be checked as they are possible leaks.Some classes deliberately create near permanent disposable object. These are SiteDefinition, LanguageData and LogRecordDataProvider.More than one object may be created per class. Only objects with explicit calls to DisposableTracker.AddObject/RemoveObject are tracked.</p><p>Tracking is enabled/disabled in the site's Appsettings.json file using \"P:YetaWF_Core:DisposableTracker\" set to true or false.</p>")}
{await HtmlHelper.ForDisplayAsync(model, nameof(model.GridDef))}");

            return hb.ToString();

        }
    }
}
