/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.PageEdit.Controllers;
using YetaWF.Modules.PageEdit.Modules;

namespace YetaWF.Modules.PageEdit.Views {

    public class LoginSiteSelectionView : YetaWFView, IYetaWFView2<PageControlModule, PageControlModuleController.LoginSiteSelectionModel> {

        public const string ViewName = "LoginSiteSelection";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(PageControlModule module, PageControlModuleController.LoginSiteSelectionModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
        {await RenderBeginFormAsync()}
            {await PartialForm(async () => await RenderPartialViewAsync(module, model), UsePartialFormCss: false)}
        {await RenderEndFormAsync()}");

            return hb.ToYHtmlString();
        }
        public async Task<YHtmlString> RenderPartialViewAsync(PageControlModule module, PageControlModuleController.LoginSiteSelectionModel model) {
            HtmlBuilder hb = new HtmlBuilder();
            hb.Append(await HtmlHelper.ForEditContainerAsync(model, "PropertyList"));
            return hb.ToYHtmlString();
        }
    }
}
