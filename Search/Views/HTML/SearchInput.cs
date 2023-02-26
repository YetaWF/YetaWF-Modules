/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Search.Modules;

namespace YetaWF.Modules.Search.Views;

public class SearchInputView : YetaWFView, IYetaWFView2<SearchInputModule, SearchInputModule.Model> {

    public const string ViewName = "SearchInput";

    public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
    public override string GetViewName() { return ViewName; }

    public async Task<string> RenderViewAsync(SearchInputModule module, SearchInputModule.Model model) {

        HtmlBuilder hb = new HtmlBuilder();

        hb.Append($@"
{await RenderBeginFormAsync()}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
    {await FormButtonsAsync(new FormButton[] {
    new FormButton() { ButtonType= ButtonTypeEnum.Submit, Text=module.SearchButtonText, Title=module.SearchButtonTT },
})}
{await RenderEndFormAsync()}");
        return hb.ToString();
    }

    public async Task<string> RenderPartialViewAsync(SearchInputModule module, SearchInputModule.Model model) {

        HtmlBuilder hb = new HtmlBuilder();
        hb.Append(await HtmlHelper.ForEditContainerAsync(model, "PropertyList"));
        return hb.ToString();

    }
}
