/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.Modules;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.Blog.Views;

public class DisqusView : YetaWFView, IYetaWFView<DisqusModule, DisqusModule.DisplayModel> {

    public const string ViewName = "Disqus";

    public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
    public override string GetViewName() { return ViewName; }

    public Task<string> RenderViewAsync(DisqusModule module, DisqusModule.DisplayModel model) {

        HtmlBuilder hb = new HtmlBuilder();

        hb.Append($@"
<div id='disqus_thread'></div>");

        ScriptBuilder sb = new ScriptBuilder();
        sb.Append($@"
    var disqus_config = function () {{
        this.page.url = '{Utility.JserEncode(Manager.CurrentPage.EvaluatedCanonicalUrl)}';
        this.page.identifier = '{Utility.JserEncode(Manager.CurrentPage.PageGuid.ToString())}';
        this.page.title = '{Utility.JserEncode(Manager.CurrentPage.Title)}';
        this.page.language = '{MultiString.ActiveLanguage.Substring(0, 2)}';");

        sb.Append($@"
    }};
    (function () {{
        var d = document, s = d.createElement('script');
        s.src = '//{Utility.JserEncode(model.ShortName)}.disqus.com/embed.js';
        s.setAttribute('data-timestamp', +new Date());
        (d.head || d.body).appendChild(s);
    }})();");

        Manager.ScriptManager.AddLast(sb.ToString());

        return Task.FromResult(hb.ToString());
    }
}
