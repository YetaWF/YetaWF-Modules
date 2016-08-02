/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Support;

namespace YetaWF.Modules.Identity.Controllers {

    public class ForgotPasswordModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.ForgotPasswordModule> {

        public ForgotPasswordModuleController() { }

        [Trim]
        public class EditModel {

            [TextAbove("You can request your password by providing your email address. The password will be sent to your email address and should arrive in your inbox within a few minutes. Please make sure to update your spam filters to avoid rejecting this email.")]
            [Caption("Email Address"), Description("Enter the email address associated with your account")]
            [UIHint("Email"), Required]
            public string  Email { get; set; }

            [Caption("Captcha"), Description("Please enter the code shown so we can verify you're a human and not a spam bot")]
            [UIHint("Recaptcha"), Trim, Recaptcha("Please correct the code entered"), SuppressIfEqual("ShowCaptcha", false)]
            public RecaptchaData Captcha { get; set; }
            [UIHint("Hidden")]
            public bool ShowCaptcha { get; set; }

            public EditModel() { }
        }

        [HttpGet]
        public ActionResult ForgotPassword() {
            LoginConfigData config = LoginConfigDataProvider.GetConfig();
            EditModel model = new EditModel {
                ShowCaptcha = config.CaptchaForgotPassword,
                Captcha = new RecaptchaData() { VerifyPresence = true },
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult ForgotPassword_Partial(EditModel model) {

            LoginConfigData config = LoginConfigDataProvider.GetConfig();
            model.ShowCaptcha = config.CaptchaForgotPassword;
            if (!ModelState.IsValid)
                return PartialView(model);
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                UserDefinition userDef = userDP.GetItemByEmail(model.Email);
                if (userDef == null) {
                    ModelState.AddModelError("Email", this.__ResStr("badEmail", "According to our records there is no account associated with this email address"));
                    return PartialView(model);
                }
                switch (userDef.UserStatus) {
                    case UserStatusEnum.Approved:
                        Emails emails = new Emails();
                        emails.SendForgottenEmail(userDef, config.BccForgottenPassword ? Manager.CurrentSite.AdminEmail : null);
                        return FormProcessed(model, this.__ResStr("okSaved", "We just sent an email to your email address with your password information - Please allow a few minutes for delivery and make sure your spam filters allow emails from {0}", Manager.CurrentSite.SMTP.Server), OnPopupClose: OnPopupCloseEnum.ReloadModule);
                    case UserStatusEnum.NeedApproval:
                        ModelState.AddModelError("Email", this.__ResStr("needApproval", "This account has not yet been approved and is awaiting approval by the site administrator"));
                        break;
                    case UserStatusEnum.NeedValidation:
                        ModelState.AddModelError("Email", this.__ResStr("needValidation", "This account has not yet been validated - Please check your emails for our validation email"));
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