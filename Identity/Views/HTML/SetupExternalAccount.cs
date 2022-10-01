/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Identity.Controllers;
using YetaWF.Modules.Identity.Modules;

namespace YetaWF.Modules.Identity.Views {

    public class SetupExternalAccountView : YetaWFView, IYetaWFView2<SetupExternalAccountModule, SetupExternalAccountModuleController.SetupExternalAccountModel> {

        public const string ViewName = "SetupExternalAccount";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<string> RenderViewAsync(SetupExternalAccountModule module, SetupExternalAccountModuleController.SetupExternalAccountModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
{await RenderBeginFormAsync()}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
    {await FormButtonsAsync(new FormButton[] {
        new FormButton() { ButtonType= ButtonTypeEnum.Submit, Text=this.__ResStr("btnSave", "Setup External Account") },
        new FormButton() { ButtonType= ButtonTypeEnum.Cancel, },
    })}
{await RenderEndFormAsync()}");

            return hb.ToString();
        }

        public async Task<string> RenderPartialViewAsync(SetupExternalAccountModule module, SetupExternalAccountModuleController.SetupExternalAccountModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            if (model.AllowNewUser) {

                hb.Append($@"
<p class='t_header'>
    {this.__ResStr("extAcct", "You have successfully authenticated with <strong>{0}</strong>.<br/>Please enter your information below and click the Setup External Account button to finish logging in.", HE(model.LoginProviderDisplay))}
</p>");

            } else {

                hb.Append($@"
<p class='t_header'>
    {this.__ResStr("extAcctInvite", "You have successfully authenticated with <strong>{0}</strong>.<br/>Please enter the invitation code below and click the Setup External Account button to finish logging in.", HE(model.LoginProviderDisplay))}
</p>");

            }

            hb.Append($@"
{await HtmlHelper.ForEditContainerAsync(model, "PropertyList")}");

            return hb.ToString();

        }
    }
}
