using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.Controllers;
using YetaWF.Modules.Blog.Modules;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.Blog.Views {

    public class DisqusView : YetaWFView, IYetaWFView<DisqusModule, DisqusModuleController.DisplayModel> {

        public const string ViewName = "Disqus";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public Task<YHtmlString> RenderViewAsync(DisqusModule module, DisqusModuleController.DisplayModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div id='disqus_thread'></div>
<script>
    var disqus_config = function () {{
        this.page.url = '{YetaWFManager.JserEncode(Manager.CurrentPage.EvaluatedCanonicalUrl)}';
        this.page.identifier = '{YetaWFManager.JserEncode(Manager.CurrentPage.PageGuid.ToString())}';
        this.page.title = '{YetaWFManager.JserEncode(Manager.CurrentPage.Title)}';
        this.page.language = '{MultiString.ActiveLanguage.Substring(0, 2)}';");

            if (model.UseSSO && !string.IsNullOrWhiteSpace(model.AuthPayload)) {
                hb.Append($@"
        this.page.remote_auth_s3 = '{YetaWFManager.JserEncode(model.AuthPayload)}';
        this.page.api_key = '{YetaWFManager.JserEncode(model.PublicKey)}';");
            }
            if (model.UseSSO) {
                hb.Append($@"
        this.sso = {{
            name: '{YetaWFManager.JserEncode(Manager.CurrentSite.SiteDomain)}',
            //button: 'https://yetawf.com/images/samplenews.gif',
            //icon: 'https://yetawf.com/favicon.png',
            url: '{YetaWFManager.JserEncode(model.LoginUrl)}',
            logout: '{YetaWFManager.JserEncode(model.LogoffUrl)}',
            width: {model.Width},
            height: {model.Height}
        }};");
           }

            hb.Append($@"
    }};
    (function () {{
        var d = document, s = d.createElement('script');
        s.src = '//{YetaWFManager.JserEncode(model.ShortName)}.disqus.com/embed.js';
        s.setAttribute('data-timestamp', +new Date());
        (d.head || d.body).appendChild(s);
    }})();
</script>");

            return Task.FromResult(hb.ToYHtmlString());
        }
    }
}
