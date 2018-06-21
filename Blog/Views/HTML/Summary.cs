/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.Controllers;
using YetaWF.Modules.Blog.Modules;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.Blog.Views {

    public class SummaryView : YetaWFView, IYetaWFView<SummaryModule, SummaryModuleController.DisplayModel> {

        public const string ViewName = "Summary";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(SummaryModule module, SummaryModuleController.DisplayModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            int count = 0;

            foreach (SummaryModuleController.Entry blogEntry in model.BlogEntries) {
                ++count;

                hb.Append($@"
<div class='t_entry t_entry{count}'>
    {await blogEntry.ViewAction.RenderAsLinkAsync()}
</div>");

            }
            return hb.ToYHtmlString();
        }
    }
}
