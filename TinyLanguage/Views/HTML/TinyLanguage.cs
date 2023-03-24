/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TinyLanguage#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.TinyLanguage.Modules;

namespace YetaWF.Modules.TinyLanguage.Views;

public class TinyLanguageView : YetaWFView, IYetaWFView2<TinyLanguageModule, TinyLanguageModule.EditModel> {

    public const string ViewName = "TinyLanguage";

    public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
    public override string GetViewName() { return ViewName; }

    public async Task<string> RenderViewAsync(TinyLanguageModule module, TinyLanguageModule.EditModel model) {

        HtmlBuilder hb = new HtmlBuilder();

        hb.Append($@"
{await RenderBeginFormAsync(HtmlAttributes: new { Id = DivId })}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model), UsePartialFormCss: false)}
{await RenderEndFormAsync()}");
        return hb.ToString();
    }

    public async Task<string> RenderPartialViewAsync(TinyLanguageModule module, TinyLanguageModule.EditModel model) {

        HtmlBuilder hb = new HtmlBuilder();

        hb.Append($@"
{await HtmlHelper.ForEditContainerAsync(model, "PropertyList")}");

        return hb.ToString();

    }
}
