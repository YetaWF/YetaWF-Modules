/* Copyright � 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

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
using System;
using YetaWF.Core.Identity;
using YetaWF.Modules.Identity.Modules;
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

            [TextAbove("You can request help recovering your password with your email address. Information will be sent to your email address and should arrive in your inbox within a few minutes. Please make sure to update your spam filters to avoid rejecting this email.")]
            [Caption("Email Address"), Description("Enter the email address associated with your account")]
            [UIHint("Email"), StringLength(Globals.MaxEmail), EmailValidation, Trim]
            [ProcessIf(nameof(RegistrationType), RegistrationTypeEnum.EmailOnly)]
            [RequiredIf(nameof(RegistrationType), RegistrationTypeEnum.EmailOnly)]
            public string Email { get; set; }

            [TextAbove("You can request help recovering your password with your account name. Information will be sent to the registered email address and should arrive in your inbox within a few minutes. Please make sure to update your spam filters to avoid rejecting this email.")]
            [Caption("Name"), Description("Enter the name associated with your account")]
            [UIHint("Text40"), StringLength(Globals.MaxUser), UserNameValidation, Trim]
            [ProcessIf(nameof(RegistrationType), RegistrationTypeEnum.NameAndEmail)]
            [ProcessIf(nameof(RegistrationType), RegistrationTypeEnum.NameOnly)]
            [RequiredIf(nameof(RegistrationType), RegistrationTypeEnum.NameAndEmail)]
            [RequiredIf(nameof(RegistrationType), RegistrationTypeEnum.NameOnly)]
            public string UserName { get; set; }

            [Caption("Captcha"), Description("Please verify that you're a human and not a spam bot")]
            [UIHint("RecaptchaV2"), RecaptchaV2("Please verify that you're a human and not a spam bot"), SuppressIf("ShowCaptcha", false)]
            public RecaptchaV2Data Captcha { get; set; }
            [UIHint("Hidden")]
            public bool ShowCaptcha { get; set; }

            [UIHint("Hidden")]
            public RegistrationTypeEnum RegistrationType { get; set; }

            public EditModel() {
                Captcha = new RecaptchaV2Data();
            }
        }

        [AllowGet]
        public async Task<ActionResult> ForgotPassword() {
            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            EditModel model = new EditModel {
                ShowCaptcha = config.CaptchaForgotPassword,
                RegistrationType = config.RegistrationType,
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
            model.RegistrationType = config.RegistrationType;
            if (!ModelState.IsValid)
                return PartialView(model);

            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {

                UserDefinition userDef;
                switch (model.RegistrationType) {
                    case RegistrationTypeEnum.NameOnly:
                    case RegistrationTypeEnum.NameAndEmail:
                        userDef = await userDP.GetItemAsync(model.UserName);
                        if (userDef == null) {
                            ModelState.AddModelError(nameof(model.UserName), this.__ResStr("badName", "According to our records there is no account associated with this name"));
                            return PartialView(model);
                        }
                        break;
                    default:
                    case RegistrationTypeEnum.EmailOnly:
                        userDef = await userDP.GetItemByEmailAsync(model.Email);
                        if (userDef == null) {
                            ModelState.AddModelError(nameof(model.Email), this.__ResStr("badEmail", "According to our records there is no account associated with this email address"));
                            return PartialView(model);
                        }
                        break;
                }

                using (UserLoginInfoDataProvider logInfoDP = new UserLoginInfoDataProvider()) {
                    if (await logInfoDP.IsExternalUserAsync(Manager.UserId)) {
                        ModelState.AddModelError(nameof(model.Email), this.__ResStr("extUser", "This account can only be accessed using an external login provider"));
                        ModelState.AddModelError(nameof(model.UserName), this.__ResStr("extUser", "This account can only be accessed using an external login provider"));
                        return PartialView(model);
                    }
                }
                switch (userDef.UserStatus) {
                    case UserStatusEnum.Approved:
                        Emails emails = new Emails();
                        if (config.SavePlainTextPassword) {
                            await emails.SendForgottenEmailAsync(userDef, config.BccForgottenPassword ? Manager.CurrentSite.AdminEmail : null);
                            string from = emails.GetSendingEmailAddress();
                            return FormProcessed(model, this.__ResStr("okForgot", "We just sent an email to your email address with your password information - Please allow a few minutes for delivery and make sure your spam filters allow emails from {0}", from), OnClose: OnCloseEnum.Nothing, OnPopupClose: OnPopupCloseEnum.ReloadModule);
                        } else {
                            if (userDef.ResetKey != null && userDef.ResetValidUntil != null && userDef.ResetValidUntil > DateTime.UtcNow) {
                                // preserve existing key in case user resends
                            } else {
                                userDef.ResetKey = Guid.NewGuid();
                                userDef.ResetValidUntil = DateTime.UtcNow.Add(config.ResetTimeSpan);
                                if (await userDP.UpdateItemAsync(userDef) != Core.DataProvider.UpdateStatusEnum.OK)// update reset key info
                                    throw new Error(this.__ResStr("resetUpdate", "User information could not be updated"));
                            }
                            await emails.SendPasswordResetEmailAsync(userDef, config.BccForgottenPassword ? Manager.CurrentSite.AdminEmail : null);
                            string from = emails.GetSendingEmailAddress();
                            return FormProcessed(model, this.__ResStr("okReset", "We just sent an email to your email address to reset your password - Please allow a few minutes for delivery and make sure your spam filters allow emails from {0}", from), OnClose: OnCloseEnum.Nothing, OnPopupClose: OnPopupCloseEnum.ReloadModule);
                        }
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