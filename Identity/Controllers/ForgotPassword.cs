/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Support;
using YetaWF.Core.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class ForgotPasswordModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.ForgotPasswordModule> {

        public ForgotPasswordModuleController() { }

        [Trim]
        public class EditModel {

            [TextAbove("You can request your password by providing your email address. The password will be sent to your email address and should arrive in your inbox within a few minutes. Please make sure to update your spam filters to avoid rejecting this email.")]
            [Caption("Email Address"), Description("Enter the email address associated with your account")]
            [UIHint("Email"), StringLength(Globals.MaxEmail), Required]
            public string Email { get; set; }

            [Caption("Captcha"), Description("Please verify that you're a human and not a spam bot")]
            [UIHint("RecaptchaV2"), RecaptchaV2("Please verify that you're a human and not a spam bot"), SuppressIfEqual("ShowCaptcha", false)]
            public RecaptchaV2Data Captcha { get; set; }
            [UIHint("Hidden")]
            public bool ShowCaptcha { get; set; }

            public EditModel() {
                Captcha = new RecaptchaV2Data();
            }
        }

        [AllowGet]
        public async Task<ActionResult> ForgotPassword() {
            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            EditModel model = new EditModel {
                ShowCaptcha = config.CaptchaForgotPassword,
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> ForgotPassword_Partial(EditModel model) {

            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            if (model.ShowCaptcha != config.CaptchaForgotPassword)
                throw new InternalError("Hidden field tampering detected");

            model.ShowCaptcha = config.CaptchaForgotPassword;
            if (!ModelState.IsValid)
                return PartialView(model);
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                UserDefinition userDef = await userDP.GetItemByEmailAsync(model.Email);
                if (userDef == null) {
                    ModelState.AddModelError("Email", this.__ResStr("badEmail", "According to our records there is no account associated with this email address"));
                    return PartialView(model);
                }
                using (UserLoginInfoDataProvider logInfoDP = new UserLoginInfoDataProvider()) {
                    if (await logInfoDP.IsExternalUserAsync(Manager.UserId)) {
                        ModelState.AddModelError("Email", this.__ResStr("extUser", "This account can only be accessed using an external login provider"));
                        return PartialView(model);
                    }
                }
                switch (userDef.UserStatus) {
                    case UserStatusEnum.Approved:
                        Emails emails = new Emails();
                        await emails.SendForgottenEmailAsync(userDef, config.BccForgottenPassword ? Manager.CurrentSite.AdminEmail : null);
                        return FormProcessed(model, this.__ResStr("okSaved", "We just sent an email to your email address with your password information - Please allow a few minutes for delivery and make sure your spam filters allow emails from {0}", Manager.CurrentSite.SMTP.Server), OnClose: OnCloseEnum.Nothing, OnPopupClose: OnPopupCloseEnum.ReloadModule);
                    case UserStatusEnum.NeedApproval:
                        ModelState.AddModelError("Email", this.__ResStr("needApproval", "This account has not yet been approved and is awaiting approval by the site administrator"));
                        break;
                    case UserStatusEnum.NeedValidation:
                        ModelState.AddModelError("Email", this.__ResStr("needValidation", "This account has not yet been verified - Please check your emails for our verification email"));
                        break;
                    case UserStatusEnum.Rejected:
                        ModelState.AddModelError("Email", this.__ResStr("rejected", "This account has been rejected and is not accessible"));
                        break;
                    case UserStatusEnum.Suspended:
                        ModelState.AddModelError("Email", this.__ResStr("suspended", "This account has been suspended and is not accessible"));
                        break;
                    default:
                        ModelState.AddModelError("Email", this.__ResStr("unknownState", "This account is in an undefined state and is not accessible"));
                        break;
                }
                return PartialView(model);
            }
        }
    }
}