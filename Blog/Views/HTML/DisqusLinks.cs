/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.Modules;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.Blog.Views {

    public class DisqusLinksView : YetaWFView, IYetaWFView<DisqusLinksModule, DisqusLinksModule.DisplayModel> {

        public const string ViewName = "DisqusLinks";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public Task<string> RenderViewAsync(DisqusLinksModule module, DisqusLinksModule.DisplayModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            return Task.FromResult($@"<script id='dsq-count-scr' src='//{Utility.JserEncode(model.ShortName.ToLower())}.disqus.com/count.js' async></script>");
        }
    }
}
