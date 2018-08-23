/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Identity.Controllers;
using YetaWF.Modules.Identity.Modules;

namespace YetaWF.Modules.Identity.Views {

    public class RegisterView : YetaWFView, IYetaWFView2<RegisterModule, RegisterModuleController.RegisterModel> {

        public const string ViewName = "Register";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(RegisterModule module, RegisterModuleController.RegisterModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
{await RenderBeginFormAsync()}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
    {await FormButtonsAsync(new FormButton[] {
        new FormButton() { ButtonType= ButtonTypeEnum.Submit, Text=this.__ResStr("btnSave", "Register") },
        new FormButton() { ButtonType= ButtonTypeEnum.Cancel, },
    })}
{await RenderEndFormAsync()}");

            if (model.ExternalProviders.Count > 0) {

                hb.Append($@"
<div class='t_external'>
    <h2>{HE(this.__ResStr("extProviders", "External Login Providers"))}</h2>");

                if (Manager.IsLocalHost) {
                    hb.Append($@"
    <div class='{HAE(@YetaWF.Core.Globals.CssDivWarning)}'>
        {HE(this.__ResStr("localOnly", "External login providers may not work when your site uses Localhost."))}
    </div>");
                }

                if (model.ExternalProviders.Count > 1) {
                    hb.Append($@"<p>{HE(this.__ResStr("accts", "Log in using one of these accounts:"))}</p>");
                } else {
                    hb.Append($@"<p>{HE(this.__ResStr("acct1", "Log in using this account:"))}</p>");
                }

                hb.Append($@"
    {await RenderBeginFormAsync(Pure: true, ActionName: nameof(LoginExternalController.ExternalLogin_Partial), ControllerName: "LoginExternal", HtmlAttributes: new { ReturnUrl = Manager.ReturnToUrl })}");
                hb.Append(HtmlHelper.AntiForgeryToken());

                int index = 0;
                foreach (FormButton formButton in model.ExternalProviders) {
                    hb.Append($@"
        <div class='t_ext1'>
            <img src='{HAE(model.Images[index])}' alt='{HAE(formButton.Title)}' title='{HAE(formButton.Title)}' />
            {await formButton.RenderAsync()}
        </div>");
                    ++index;
                }
                hb.Append($@"
    {await RenderEndFormAsync()}
</div>");
            }

            return hb.ToYHtmlString();
        }

        public async Task<YHtmlString> RenderPartialViewAsync(RegisterModule module, RegisterModuleController.RegisterModel model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append(await HtmlHelper.ForEditContainerAsync(model, "PropertyList"));
            return hb.ToYHtmlString();

        }
    }
}
