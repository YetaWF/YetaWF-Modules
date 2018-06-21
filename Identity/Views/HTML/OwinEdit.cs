/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Identity.Controllers;
using YetaWF.Modules.Identity.Modules;

namespace YetaWF.Modules.Identity.Views {

    public class OwinEditView : YetaWFView, IYetaWFView2<OwinEditModule, OwinEditModuleController.EditModel> {

        public const string ViewName = "OwinEdit";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(OwinEditModule module, OwinEditModuleController.EditModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
{await RenderBeginFormAsync()}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
    <div class='{Globals.CssDivAlert}'>
        {HE(this.__ResStr("restartOnSubmit", "When saving these settings, the site (and all instances) must be restarted for the new settings to take effect."))}
    </div>
    {await FormButtonsAsync(new FormButton[] {
        new FormButton() { ButtonType= ButtonTypeEnum.Submit, },
        new FormButton() { ButtonType= ButtonTypeEnum.Cancel, },
    })}
{await RenderEndFormAsync()}");

            return hb.ToYHtmlString();
        }

        public async Task<YHtmlString> RenderPartialViewAsync(OwinEditModule module, OwinEditModuleController.EditModel model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append(await HtmlHelper.ForEditContainerAsync(model, "PropertyList"));
            return hb.ToYHtmlString();

        }
    }
}
