/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TinyLogin#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.TinyLogin.Controllers;
using YetaWF.Modules.TinyLogin.Modules;

namespace YetaWF.Modules.TinyLogin.Views {

    public class TinyLoginView : YetaWFView, IYetaWFView<TinyLoginModule, TinyLoginModuleController.TinyLoginModel> {

        public const string ViewName = "TinyLogin";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<string> RenderViewAsync(TinyLoginModule module, TinyLoginModuleController.TinyLoginModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            if (model.LoggedOn) {

                ModuleAction logoffAction = await module.GetAction_LogoffAsync(model.LogoffUrl);
                ModuleAction userNameAction = await module.GetAction_UserNameAsync(model.UserUrl, model.UserName!, model.UserTooltip);

                hb.Append($@"
<div class='t_haveuser t_link'>
    {await logoffAction.RenderAsync(ModuleAction.RenderModeEnum.NormalLinks)}
</div>
<div class='t_haveuser t_user'>
    {await userNameAction.RenderAsync(ModuleAction.RenderModeEnum.NormalLinks)}
</div>");
            } else {

                ModuleAction loginAction = await module.GetAction_LoginAsync(model.LogonUrl);
                ModuleAction? registerAction = await module.GetAction_RegisterAsync(model.RegisterUrl);

                hb.Append($@"
<div class='t_nouser t_logon'>
    {await loginAction.RenderAsync(ModuleAction.RenderModeEnum.NormalLinks)}
</div>
<div class='t_nouser t_register'>");

                if (registerAction != null) {
                    hb.Append($@"
                    {await registerAction.RenderAsync(ModuleAction.RenderModeEnum.NormalLinks)}");
                }
                hb.Append($@"
</div>");
            }

            return hb.ToString();
        }
    }
}
