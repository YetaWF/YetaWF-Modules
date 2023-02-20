/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ModuleEdit#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.ModuleEdit.Modules;

namespace YetaWF.Modules.ModuleEdit.Views;

public class EditView : YetaWFView, IYetaWFView2<ModuleEditModule, ModuleEditModule.ModuleEditModel> {

    public const string ViewName = "ModuleEdit";

    public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
    public override string GetViewName() { return ViewName; }

    public async Task<string> RenderViewAsync(ModuleEditModule module, ModuleEditModule.ModuleEditModel model) {

        HtmlBuilder hb = new HtmlBuilder();

        hb.Append($@"
{await RenderBeginFormAsync()}
    {await HtmlHelper.ForDisplayAsync(model, nameof(model.ModuleGuid))}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
    {await FormButtonsAsync(new FormButton[] {
    new FormButton() { ButtonType= ButtonTypeEnum.Submit, },
    new FormButton() { ButtonType= ButtonTypeEnum.Cancel, },
})}
{await RenderEndFormAsync()}");
        return hb.ToString();
    }

    public async Task<string> RenderPartialViewAsync(ModuleEditModule module, ModuleEditModule.ModuleEditModel model) {

        HtmlBuilder hb = new HtmlBuilder();
        hb.Append(await HtmlHelper.ForEditAsync(model, nameof(model.Dynamic)));
        return hb.ToString();

    }
}
