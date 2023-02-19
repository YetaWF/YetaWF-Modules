/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.Modules;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.Blog.Views;

public class SummaryView : YetaWFView, IYetaWFView<SummaryModule, SummaryModule.DisplayModel> {

    public const string ViewName = "Summary";

    public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
    public override string GetViewName() { return ViewName; }

    public async Task<string> RenderViewAsync(SummaryModule module, SummaryModule.DisplayModel model) {

        HtmlBuilder hb = new HtmlBuilder();

        int count = 0;

        foreach (SummaryModule.Entry blogEntry in model.BlogEntries) {
            ++count;

            hb.Append($@"
<div class='t_entry t_entry{count}'>
    {await blogEntry.ViewAction.RenderAsLinkAsync()}
</div>");

        }
        return hb.ToString();
    }
}