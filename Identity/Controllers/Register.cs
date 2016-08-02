/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Models;
using YetaWF.Modules.Identity.Modules;
using YetaWF.Modules.Identity.Support;

namespace YetaWF.Modules.Identity.Controllers {

    public class RegisterModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.RegisterModule> {

        public RegisterModuleController() { }

        [Trim]
        public class RegisterModel {

            public RegisterModel() {
                Captcha = new RecaptchaData();
            }

            [Caption("User Name"), Description("Enter your user name to register")]
            [UIHint("Text40"), SuppressIfEqual("RegistrationType", RegistrationTypeEnum.EmailOnly), StringLength(Globals.MaxUser), UserNameValidation, Required, Trim]
            public string UserName { get; set; }

            [Caption("Email Address"), Description("Enter your email address to register - this is the email address used by this site to communicate with you")]
            [UIHint("Email"), SuppressIfEqual("RegistrationType", RegistrationTypeEnum.NameOnly), StringLength(Globals.MaxEmail), EmailValidation, Required, Trim]
            public string Email { get; set; }

            [Caption("Password"), Description("Enter your desired password")]
            [UIHint("Password20"), Required]
            public string Password { get; set; }

            [Caption("Password Confirmation"), Description("Enter your password again to confirm")]
            [UIHint("Password20"), Required, SameAs("Password", "The password confirmation doesn't match the password entered")]
            public string ConfirmPassword { get; set; }

            [Caption("Captcha"), Description("Please enter the code shown so we can verify you're a human and not a spam bot")]
            [UIHint("Recaptcha"), Recaptcha("Please correct the code entered"), SuppressIfEqual("ShowCaptcha", false), Trim]
            public RecaptchaData Captcha { get; set; }

            [UIHint("Hidden")]
            public RegistrationTypeEnum RegistrationType { get; set; }
            [UIHint("Hidden")]
            public bool ShowCaptcha { get; set; }

            [UIHint("Hidden")]
            public string ReturnUrl { get; set; }
        }

        [HttpGet]
        public ActionResult Register() {

            LoginConfigData config = LoginConfigDataProvider.GetConfig();
            if (!config.AllowUserRegistration)
                throw new Error(this.__ResStr("cantRegister", "This site does not allow new user registration"));

            RegisterModel model = new RegisterModel {
                RegistrationType = config.RegistrationType,
                ShowCaptcha = config.Captcha,
                Captcha = new RecaptchaData() { VerifyPresence = true },
                ReturnUrl = Manager.ReturnToUrl,
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> Register_Partial(RegisterModel model) {
            LoginConfigData config = LoginConfigDataProvider.GetConfig();
            if (!config.AllowUserRegistration)
                throw new Error(this.__ResStr("cantRegister", "This site does not allow new user registration"));

            if (model.ShowCaptcha != config.Captcha)
                throw new InternalError("Hidden field tampering detected - model.ShowCaptcha != Module.Captcha");

            model.RegistrationType = config.RegistrationType;// don't trust what we get from user

            if (!ModelState.IsValid)
                return PartialView(model);

            // Create a new user and save all info
            UserDefinition user = new UserDefinition();
            switch (config.RegistrationType) {
                default:
                case RegistrationTypeEnum.NameAndEmail:
                    user.UserName = model.UserName;
                    user.Email = model.Email;
                    break;
                case RegistrationTypeEnum.EmailOnly:
                    user.UserName = user.Email = model.Email;
                    break;
                case RegistrationTypeEnum.NameOnly:
                    user.UserName = user.Email = model.UserName;
                    break;
            }

            if (config.SavePlainTextPassword)
                user.PasswordPlainText = model.Password;

            if (config.RegistrationType == RegistrationTypeEnum.NameAndEmail) {
                using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                    // Email == user.Email
                    List<DataProviderFilterInfo> filters = new List<DataProviderFilterInfo> {
                        new DataProviderFilterInfo {
                            Field = "Email", Operator = "==", Value = user.Email,
                        },
                    };
                    UserDefinition userExists = dataProvider.GetItem(filters);
                    if (userExists != null && user.UserName != userExists.Email) {
                        ModelState.AddModelError("Email", this.__ResStr("emailUsed", "An account with email address {0} already exists.", user.Email));
                        return PartialView(model);
                    }
                }
            }

            // set account status
            if (config.VerifyNewUsers)
                user.UserStatus = UserStatusEnum.NeedValidation;
            else if (config.ApproveNewUsers)
                user.UserStatus = UserStatusEnum.NeedApproval;
            else
                user.UserStatus = UserStatusEnum.Approved;
            user.RegistrationIP = Manager.UserHostAddress;

            // create user
            var result = await Managers.GetUserManager().CreateAsync(user, model.Password);
            if (!result.Succeeded) {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("UserName", error);
                return PartialView(model);
            }

            // send appropriate email based on account status
            Emails emails = new Emails();
            if (user.UserStatus == UserStatusEnum.NeedValidation) {
                emails.SendVerification(user, config.BccVerification ? Manager.CurrentSite.AdminEmail : null);
                string nextPage = string.IsNullOrWhiteSpace(config.VerificationPendingUrl) ? Manager.CurrentSite.HomePageUrl : config.VerificationPendingUrl;
                return FormProcessed(model, this.__ResStr("okAwaitRegistration", "An email has just been sent to your email address \"{0}\" to complete the registration. Allow a few minutes for delivery. Once received, use the information in the email to complete the registration.", model.Email),
                    this.__ResStr("okRegTitle", "Welcome!"),
                    NextPage: nextPage);
            } else if (user.UserStatus == UserStatusEnum.NeedApproval) {
                emails.SendApprovalNeeded(user);
                string nextPage = string.IsNullOrWhiteSpace(config.ApprovalPendingUrl) ? Manager.CurrentSite.HomePageUrl : config.ApprovalPendingUrl;
                return FormProcessed(model, this.__ResStr("okAwaitApproval", "An email has just been sent to the site adminstrator for approval of your new account. Once processed and approved, you will receive an email confirming your approval."),
                    this.__ResStr("okRegTitle", "Welcome!"),
                    NextPage: nextPage);
            } else if (user.UserStatus == UserStatusEnum.Approved) {
                if (config.NotifyAdminNewUsers)
                    emails.SendNewUserCreated(user);
                await LoginModuleController.UserLoginAsync(user, config.PersistentLogin);
                return FormProcessed(model, this.__ResStr("okRegText", "Your new account has been successfully registered."), this.__ResStr("okRegTitle", "Welcome!"),
                    NextPage: model.ReturnUrl);
            } else
                throw new InternalError("badUserStatus", "Unexpected account status {0}", user.UserStatus);
        }

        [HttpGet]
        [Permission("ChangeAccounts")]
        [ExcludeDemoMode]
        public ActionResult Approve(string userName) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                UserDefinition user = GetUser(userName, dataProvider);
                if (user.UserStatus != UserStatusEnum.Approved) {
                    if (user.UserStatus != UserStatusEnum.NeedApproval)
                        throw new Error(this.__ResStr("notCantApprove", "User {0} is no longer new and cannot be approved.", userName));
                    user.UserStatus = UserStatusEnum.Approved;
                    UpdateStatusEnum status = dataProvider.UpdateItem(user);
                    switch (status) {
                        default:
                        case UpdateStatusEnum.NewKeyExists:
                            throw new InternalError("Unexpected status {0}", status);
                        case UpdateStatusEnum.RecordDeleted:
                            throw new Error(this.__ResStr("approveUserNotFound", "The user account for user {0} no longer exists.", userName));
                        case UpdateStatusEnum.OK:
                            break;
                    }
                }
                Emails emails = new Emails();
                emails.SendApproval(user);
                return Reload(null, Reload: ReloadEnum.ModuleParts, PopupText: this.__ResStr("userApproved", "The user account for user {0} has been marked as approved. An email has been sent to the user.", userName));
            }
        }

        [HttpGet]
        [Permission("ChangeAccounts")]
        [ExcludeDemoMode]
        public ActionResult Reject(string userName) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                UserDefinition user = GetUser(userName, dataProvider);
                if (user.UserStatus != UserStatusEnum.Rejected) {
                    user.UserStatus = UserStatusEnum.Rejected;
                    UpdateStatusEnum status = dataProvider.UpdateItem(user);
                    switch (status) {
                        default:
                        case UpdateStatusEnum.NewKeyExists:
                            throw new InternalError("Unexpected status {0}", status);
                        case UpdateStatusEnum.RecordDeleted:
                            throw new Error(this.__ResStr("rejectUserNotFound", "The user account for user {0} no longer exists.", userName));
                        case UpdateStatusEnum.OK:
                            break;
                    }
                }
                Emails emails = new Emails();
                emails.SendRejected(user);

                return Reload(null, Reload: ReloadEnum.ModuleParts, PopupText: this.__ResStr("userRejected", "The user account for user {0} has been marked as rejected. An email has been sent to the user.", userName));
            }
        }

        [HttpGet]
        [Permission("ChangeAccounts")]
        [ExcludeDemoMode]
        public ActionResult Suspend(string userName) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                UserDefinition user = GetUser(userName, dataProvider);
                if (user.UserStatus != UserStatusEnum.Suspended) {
                    user.UserStatus = UserStatusEnum.Suspended;
                    UpdateStatusEnum status = dataProvider.UpdateItem(user);
                    switch (status) {
                        default:
                        case UpdateStatusEnum.NewKeyExists:
                            throw new InternalError("Unexpected status {0}", status);
                        case UpdateStatusEnum.RecordDeleted:
                            throw new Error(this.__ResStr("rejectUserNotFound", "The user account for user {0} no longer exists.", userName));
                        case UpdateStatusEnum.OK:
                            break;
                    }
                }
                Emails emails = new Emails();
                emails.SendSuspended(user);

                return Reload(null, Reload: ReloadEnum.ModuleParts, PopupText: this.__ResStr("userSuspended", "The user account for user {0} has been marked as suspended. An email has been sent to the user.", userName));
            }
        }
        private UserDefinition GetUser(string userName, UserDefinitionDataProvider dataProvider) {
            if (string.IsNullOrWhiteSpace(userName))
                throw new Error(this.__ResStr("noItem", "No user name specified"));
            UserDefinition user = dataProvider.GetItem(userName);
            if (user == null)
                throw new Error(this.__ResStr("notFoundUser", "User {0} not found.", userName));
            return user;
        }
    }
}