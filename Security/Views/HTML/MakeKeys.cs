/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Security#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Security.Modules;

namespace YetaWF.Modules.Security.Views;

public class EditView : YetaWFView, IYetaWFView<MakeKeysModule, MakeKeysModule.Model> {

    public const string ViewName = "MakeKeys";

    public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
    public override string GetViewName() { return ViewName; }

    public async Task<string> RenderViewAsync(MakeKeysModule module, MakeKeysModule.Model model) {

        HtmlBuilder hb = new HtmlBuilder();

        hb.Append($@"
{await RenderBeginFormAsync()}
    {await HtmlHelper.ForEditContainerAsync(model, "PropertyList")}
    {await FormButtonsAsync(new FormButton[] {
    new FormButton() { ButtonType= ButtonTypeEnum.Cancel, Text=this.__ResStr("btnCancel", "Return") },
})}
{await RenderEndFormAsync()}");
        return hb.ToString();
    }
}
