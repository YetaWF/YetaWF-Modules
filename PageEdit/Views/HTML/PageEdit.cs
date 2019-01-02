/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.PageEdit.Controllers;
using YetaWF.Modules.PageEdit.Modules;

namespace YetaWF.Modules.PageEdit.Views {

    public class PageEditView : YetaWFView, IYetaWFView2<PageEditModule, PageEditModuleController.EditModel> {

        public const string ViewName = "PageEdit";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(PageEditModule module, PageEditModuleController.EditModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
{await RenderBeginFormAsync()}
    {await HtmlHelper.ForDisplayAsync(model, nameof(model.PageGuid))}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
    {await FormButtonsAsync(new FormButton[] {
        new FormButton() { ButtonType= ButtonTypeEnum.Submit, },
        new FormButton() { ButtonType= ButtonTypeEnum.Cancel, },
    })}
{await RenderEndFormAsync()}");
            return hb.ToYHtmlString();
        }

        public async Task<YHtmlString> RenderPartialViewAsync(PageEditModule module, PageEditModuleController.EditModel model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append(await HtmlHelper.ForEditAsync(model, nameof(model.Page)));
            return hb.ToYHtmlString();

        }
    }
}
