/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Search.Modules;

namespace YetaWF.Modules.Search.Views;

public class SearchControlView : YetaWFView, IYetaWFView<SearchControlModule, SearchControlModule.Model> {

    public const string ViewName = "SearchControl";

    public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
    public override string GetViewName() { return ViewName; }

    public async Task<string> RenderViewAsync(SearchControlModule module, SearchControlModule.Model model) {

        HtmlBuilder hb = new HtmlBuilder();

        hb.Append($@"
{await HtmlHelper.ForDisplayAsync(model, nameof(model.On))}
{await HtmlHelper.ForDisplayAsync(model, nameof(model.Off))}");

        return hb.ToString();
    }
}
