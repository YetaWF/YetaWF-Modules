/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.Controllers;
using YetaWF.Modules.Blog.DataProvider;
using YetaWF.Modules.Blog.Modules;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.Blog.Views {

    public class BlogView : YetaWFView, IYetaWFView<BlogModule, BlogModuleController.DisplayModel> {

        public const string ViewName = "Blog";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<string> RenderViewAsync(BlogModule module, BlogModuleController.DisplayModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            int count = 0;

            if (model.StartDate != null) {
                hb.Append($@"
<div class='t_startdate'>
    {this.__ResStr("startDate", "Showing blog entries published on or before {0}.", Formatting.FormatDate(model.StartDate))}
</div>");
            }

            foreach (BlogModuleController.Entry blogEntry in model.BlogEntries) {
                ++count;

                hb.Append($@"
<h2>
    <a href='{Utility.HAE(await BlogConfigData.GetEntryCanonicalNameAsync(blogEntry.Identity))}' class='yaction-link'>
        {Utility.HE(blogEntry.Title.ToString())}
    </a>
</h2>
<div class='t_entry t_entry{count}'>
    {await HtmlHelper.ForDisplayContainerAsync(blogEntry, "PropertyList")}
</div>");
            }

            return hb.ToString();
        }
    }
}
