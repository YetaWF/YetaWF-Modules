/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Search.Modules;

namespace YetaWF.Modules.Search.Views {

    public class SearchUnavailable_BrowseView : YetaWFView, IYetaWFView<SearchBrowseModule, object> {

        public const string ViewName = "SearchUnavailable_Browse";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public Task<string> RenderViewAsync(SearchBrowseModule module, object model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div class='{Globals.CssDivWarning}'>
    {this.__ResStr("unavailable", "Search data is not available - See <a href='https://yetawf.com/Documentation/YetaWF/Search' target='_blank' rel='noopener noreferrer'>https://yetawf.com/Documentation/YetaWF/Search</a> for additional information.")}
</div>");
            return Task.FromResult(hb.ToString());
        }
    }
}
