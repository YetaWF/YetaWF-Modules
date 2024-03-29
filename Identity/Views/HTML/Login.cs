/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

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

    public class LoginView : YetaWFView, IYetaWFView2<LoginModule, LoginModuleController.LoginModel> {

        public const string ViewName = "Login";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<string> RenderViewAsync(LoginModule module, LoginModuleController.LoginModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
{await RenderBeginFormAsync()}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
    {await FormButtonsAsync(new FormButton[] {
        new FormButton() { ButtonType= ButtonTypeEnum.Submit, Text=this.__ResStr("btnSave", "Log In") },
        new FormButton() { ButtonType= ButtonTypeEnum.Cancel, },
    })}
{await RenderEndFormAsync()}");

            if (model.ExternalProviders.Count > 0) {

                hb.Append($@"
<div class='t_external'>
    <h2>{HE(this.__ResStr("extProviders", "External Login Providers"))}</h2>");

                if (Manager.IsLocalHost) {
                    hb.Append($@"
    <div class='{Globals.CssDivWarning}'>
        {HE(this.__ResStr("localOnly", "External login providers may not work when your site uses Localhost."))}
    </div>");
                }

                if (model.ExternalProviders.Count > 1) {
                    hb.Append($@"<p>{HE(this.__ResStr("accts", "Log in using one of these accounts:"))}</p>");
                } else {
                    hb.Append($@"<p>{HE(this.__ResStr("acct1", "Log in using this account:"))}</p>");
                }

                hb.Append($@"
    <form action='{HAE(Utility.UrlFor(typeof(LoginExternalController), nameof(LoginExternalController.ExternalLogin_Partial)))}' method='post'>
        {HtmlBuilder.AntiForgeryToken()}
        {await HtmlHelper.ForDisplayAsync(model, nameof(model.ReturnUrl))}");

                int index = 0;
                foreach (FormButton formButton in model.ExternalProviders) {
                    hb.Append($@"
        <div class='t_extentry'>
            <img src='{HAE(model.Images[index])}' alt='{HAE(formButton.Title)}' title='{HAE(formButton.Title)}' />
            {await formButton.RenderAsync()}
        </div>");
                    ++index;
                }
                hb.Append($@"
    </form>
</div>");
            }

            return hb.ToString();
        }

        public async Task<string> RenderPartialViewAsync(LoginModule module, LoginModuleController.LoginModel model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append(await HtmlHelper.ForEditContainerAsync(model, "PropertyList"));
            return hb.ToString();

        }
    }
}
