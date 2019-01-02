/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TawkTo#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.TawkTo.Controllers;
using YetaWF.Modules.TawkTo.DataProvider;
using YetaWF.Modules.TawkTo.Modules;

namespace YetaWF.Modules.TawkTo.Views {

    public class SkinTawkToView : YetaWFView, IYetaWFView<SkinTawkToModule, SkinTawkToModuleController.DisplayModel> {

        public const string ViewName = "SkinTawkTo";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public Task<YHtmlString> RenderViewAsync(SkinTawkToModule module, SkinTawkToModuleController.DisplayModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            ConfigData config = ConfigDataProvider.GetConfigAsync().Result;

            hb.Append($@"
<script>
    // https://www.tawk.to/javascript-api/
    var Tawk_API = Tawk_API || {{}}, Tawk_LoadStart = new Date();");

            if (Manager.HaveUser) {
                hb.Append($@"
    Tawk_API.visitor = {{
        name: '{JE(Manager.UserName)}',
        email: '{JE(Manager.UserEmail)}',
        hash: '{JE(model.Hash)}'
    }};");
            }

            hb.Append($@"
    (function() {{
        var s1=document.createElement('script'),s0=document.getElementsByTagName('script')[0];
        s1.async=true;
        s1.src = '{JE(string.Format("https://embed.tawk.to/{0}/default", YetaWFManager.UrlEncodePath(config.Account)))}';
        s1.charset='UTF-8';
        s1.setAttribute('crossorigin','*');
        s0.parentNode.insertBefore(s1,s0);
    }})();
</script>");

            return Task.FromResult(hb.ToYHtmlString());
        }
    }
}
