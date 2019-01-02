/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Packages.Controllers;
using YetaWF.Modules.Packages.Modules;

namespace YetaWF.Modules.Modules.Views {

    public class SiteTemplateProcessView : YetaWFView, IYetaWFView2<SiteTemplateProcessModule, SiteTemplateProcessModuleController.EditModel> {

        public const string ViewName = "SiteTemplateProcess";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(SiteTemplateProcessModule module, SiteTemplateProcessModuleController.EditModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
{await RenderBeginFormAsync()}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
    {await FormButtonsAsync(new FormButton[] {
        new FormButton() { ButtonType= ButtonTypeEnum.Apply, Text=this.__ResStr("btnSave", "Process") },
    })}
{await RenderEndFormAsync()}");
            return hb.ToYHtmlString();
        }

        public async Task<YHtmlString> RenderPartialViewAsync(SiteTemplateProcessModule module, SiteTemplateProcessModuleController.EditModel model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append(await HtmlHelper.ForEditContainerAsync(model, "PropertyList"));
            return hb.ToYHtmlString();

        }
    }
}
