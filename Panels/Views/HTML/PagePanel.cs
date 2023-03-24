/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Panels.Modules;

namespace YetaWF.Modules.Panels.Views;

public class PagePanelView : YetaWFView, IYetaWFView2<PagePanelModule, PagePanelModule.Model> {

    public const string ViewName = "PagePanel";

    public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
    public override string GetViewName() { return ViewName; }

    public async Task<string> RenderViewAsync(PagePanelModule module, PagePanelModule.Model model) {

        HtmlBuilder hb = new HtmlBuilder();

        if (Manager.EditMode) {

            hb.Append($@"
{await RenderBeginFormAsync()}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
    {await FormButtonsAsync(new FormButton[] {
    new FormButton() { ButtonType= ButtonTypeEnum.Apply, },
    new FormButton() { ButtonType= ButtonTypeEnum.Submit, },
    new FormButton() { ButtonType= ButtonTypeEnum.Cancel, },
})}
{await RenderEndFormAsync()}");

        } else {

            hb.Append(await HtmlHelper.ForDisplayAsync(model, nameof(model.PanelInfo)));

        }
        return hb.ToString();
    }

    public async Task<string> RenderPartialViewAsync(PagePanelModule module, PagePanelModule.Model model) {

        HtmlBuilder hb = new HtmlBuilder();
        hb.Append(await HtmlHelper.ForEditContainerAsync(model, "PropertyList"));
        return hb.ToString();

    }
}
