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

    public class SetupExternalAccountView : YetaWFView, IYetaWFView2<SetupExternalAccountModule, SetupExternalAccountModuleController.SetupExternalAccountModel> {

        public const string ViewName = "SetupExternalAccount";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(SetupExternalAccountModule module, SetupExternalAccountModuleController.SetupExternalAccountModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
{await RenderBeginFormAsync()}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
    {await FormButtonsAsync(new FormButton[] {
        new FormButton() { ButtonType= ButtonTypeEnum.Submit, Text=this.__ResStr("btnSave", "SetupExternalAccount") },
        new FormButton() { ButtonType= ButtonTypeEnum.Cancel, },
    })}
{await RenderEndFormAsync()}");

            return hb.ToYHtmlString();
        }

        public async Task<YHtmlString> RenderPartialViewAsync(SetupExternalAccountModule module, SetupExternalAccountModuleController.SetupExternalAccountModel model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append($@"
<p class='t_header'>
    {this.__ResStr("extAcct", "You have successfully authenticated with <strong>{0}</strong>.<br/>Please enter a user name for this site below and click the SetupExternalAccount button to finish logging in.", HE(model.LoginProvider))}
</p>
{await HtmlHelper.ForEditContainerAsync(model, "PropertyList")}");

            return hb.ToYHtmlString();

        }
    }
}
