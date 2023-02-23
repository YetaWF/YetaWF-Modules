/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.PageEdit.Modules;

namespace YetaWF.Modules.PageEdit.Views;

public class ImportModuleView : YetaWFView, IYetaWFView2<PageControlModule, PageControlModule.ImportModuleModel> {

    public const string ViewName = "ImportModule";

    public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
    public override string GetViewName() { return ViewName; }

    public async Task<string> RenderViewAsync(PageControlModule module, PageControlModule.ImportModuleModel model) {

        HtmlBuilder hb = new HtmlBuilder();

        hb.Append($@"
        {await RenderBeginFormAsync(ActionName: GetViewName())}
            {await PartialForm(async () => await RenderPartialViewAsync(module, model), UsePartialFormCss: false)}
        {await RenderEndFormAsync()}");

        return hb.ToString();
    }
    public async Task<string> RenderPartialViewAsync(PageControlModule module, PageControlModule.ImportModuleModel model) {
        HtmlBuilder hb = new HtmlBuilder();
        hb.Append(await HtmlHelper.ForEditContainerAsync(model, "PropertyList"));
        return hb.ToString();
    }
}
