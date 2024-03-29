/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.Modules;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.Blog.Views {

    public class NotFoundView : YetaWFView, IYetaWFView<EntryDisplayModule, object> {

        public const string ViewName = "NotFound";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public Task<string> RenderViewAsync(EntryDisplayModule module, object model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<p>{Utility.HE(this.__ResStr("blog404_1", "It looks like something went wrong. There is no such page on this site."))}</p>
<p>{Utility.HE(this.__ResStr("blog404_2", "Maybe we haven't created this page yet. Or maybe there is a typo in the page Url."))}</p>");

            return Task.FromResult(hb.ToString());
        }
    }
}
