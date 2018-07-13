/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.Controllers;
using YetaWF.Modules.Blog.Modules;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.Blog.Views {

    public class DisqusLinksView : YetaWFView, IYetaWFView<DisqusLinksModule, DisqusLinksModuleController.DisplayModel> {

        public const string ViewName = "DisqusLinks";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public Task<YHtmlString> RenderViewAsync(DisqusLinksModule module, DisqusLinksModuleController.DisplayModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<script id='dsq-count-scr' src='//{YetaWFManager.JserEncode(model.ShortName)}.disqus.com/count.js' async></script>");

            hb.Append(@"
<script>
    var _YetaWF_Blog_Disqus = {};
    _YetaWF_Blog_Disqus.on = true;

    // Handles events turning the addon on/off (used for dynamic content)
    $(document).on('YetaWF_Basics_Addon', function (event, addonGuid, on) {
        if (addonGuid == '776adfcd-da5f-4926-b29d-4c06353266c0') {
            _YetaWF_Blog_Disqus.on = on;
        }
    });
    YetaWF_Basics.addWhenReady(function (tag) {
        if (_YetaWF_Blog_Disqus.on);
            DISQUSWIDGETS.getCount({ reset: true });
    });
</script>
");

            return Task.FromResult(hb.ToYHtmlString());
        }
    }
}
