/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Logging#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Logging.Modules;

namespace YetaWF.Modules.Logging.Views;

public class BrowseLogView : YetaWFView, IYetaWFView2<BrowseLogModule, BrowseLogModule.BrowseModel> {

    public const string ViewName = "BrowseLog";

    public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
    public override string GetViewName() { return ViewName; }

    public async Task<string> RenderViewAsync(BrowseLogModule module, BrowseLogModule.BrowseModel model) {

        HtmlBuilder hb = new HtmlBuilder();

        if (!model.LogAvailable) {

            hb.Append($@"
<div class='{Globals.CssDivAlert} t_browselog t_file'>
    {HE(this.__ResStr("fileNA", "The site log is not available. Logging has not been started."))}
    {HE(this.__ResStr("fileNA2", "This may be caused by a missing log folder (\\Data\\DataFolder\\YetaWF_Logging), database or external dataprovider."))}
</div>");
        } else if (model.BrowsingSupported) {
            hb.Append($@"
{await RenderBeginFormAsync()}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
{await RenderEndFormAsync()}");
        } else {
            hb.Append($@"
<div class='{Globals.CssDivWarning} t_browselog t_file'>
    {HE(this.__ResStr("file", "Log browsing is not available. The following log provider is used: {0}", model.LoggerName))}
</div>");
        }
        return hb.ToString();
    }

    public async Task<string> RenderPartialViewAsync(BrowseLogModule module, BrowseLogModule.BrowseModel model) {

        HtmlBuilder hb = new HtmlBuilder();
        hb.Append(await HtmlHelper.ForDisplayAsync(model, nameof(model.GridDef)));
        return hb.ToString();

    }
}
