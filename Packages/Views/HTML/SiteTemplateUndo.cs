/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Packages.Modules;

namespace YetaWF.Modules.Modules.Views;

public class SiteTemplateUndoView : YetaWFView, IYetaWFView2<SiteTemplateUndoModule, SiteTemplateUndoModule.EditModel> {

    public const string ViewName = "SiteTemplateUndo";

    public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
    public override string GetViewName() { return ViewName; }

    public async Task<string> RenderViewAsync(SiteTemplateUndoModule module, SiteTemplateUndoModule.EditModel model) {

        HtmlBuilder hb = new HtmlBuilder();

        hb.Append($@"
{await RenderBeginFormAsync()}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
    {await FormButtonsAsync(new FormButton[] {
    new FormButton() { ButtonType= ButtonTypeEnum.Apply, Text=this.__ResStr("btnSave", "Undo") },
})}
{await RenderEndFormAsync()}");
        return hb.ToString();
    }

    public async Task<string> RenderPartialViewAsync(SiteTemplateUndoModule module, SiteTemplateUndoModule.EditModel model) {

        HtmlBuilder hb = new HtmlBuilder();
        hb.Append(await HtmlHelper.ForEditContainerAsync(model, "PropertyList"));
        return hb.ToString();

    }
}
