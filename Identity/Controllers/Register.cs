/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Addons;
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
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using Microsoft.Owin.Security;
using System.Web;
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class RegisterModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.RegisterModule> {

        public RegisterModuleController() { }

        [Trim]
        public class RegisterModel {

            public RegisterModel() {
                Captcha = new RecaptchaV2Data();
                ExternalProviders = new List<FormButton>();
                Images = new List<string>();
            }

            [Caption("User Name"), Description("Enter your user name to register")]
            [UIHint("Text40"), SuppressIfEqual("RegistrationType", RegistrationTypeEnum.EmailOnly), StringLength(Globals.MaxUser), UserNameValidation, Required, Trim]
            public string UserName { get; set; }

            [Caption("Email Address"), Description("Enter your email address to register - this is the email address used by this site to communicate with you")]
            [UIHint("Email"), SuppressIfEqual("RegistrationType", RegistrationTypeEnum.NameOnly), StringLength(Globals.MaxEmail), EmailValidation, Required, Trim]
            public string Email { get; set; }

            [Caption("Password"), Description("Enter your desired password")]
            [UIHint("Password20"), StringLength(Globals.MaxPswd), Required]
            public string Password { get; set; }

            [Caption("Password Confirmation"), Description("Enter your password again to confirm")]
            [UIHint("Password20"), Required, StringLength(Globals.MaxPswd), SameAs("Password", "The password confirmation doesn't match the password entered")]
            public string ConfirmPassword { get; set; }

            [Caption("Captcha"), Description("Please verify that you're a human and not a spam bot")]
            [UIHint("RecaptchaV2"), RecaptchaV2("Please verify that you're a human and not a spam bot"), SuppressIfEqual("ShowCaptcha", false)]
            public RecaptchaV2Data Captcha { get; set; }

            [UIHint("Hidden")]
            public RegistrationTypeEnum RegistrationType { get; set; }
            [UIHint("Hidden")]
            public bool ShowCaptcha { get; set; }

            [UIHint("Hidden")]
            public bool CloseOnLogin { get; set; }

            public List<string> Images { get; set; }
            public List<FormButton> ExternalProviders { get; set; }
        }

        [AllowGet]
        public async Task<ActionResult> Register(bool closeOnLogin = false) {

            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            if (!config.AllowUserRegistration)
                throw new Error(this.__ResStr("cantRegister", "This site does not allow new user registration"));

            RegisterModel model = new RegisterModel {
                RegistrationType = config.RegistrationType,
                ShowCaptcha = config.Captcha,
                Captcha = new RecaptchaV2Data(),
                CloseOnLogin = closeOnLogin,
            };

            using (LoginConfigDataProvider logConfigDP = new LoginConfigDataProvider()) {
                List<LoginConfigDataProvider.LoginProviderDescription> loginProviders = await logConfigDP.GetActiveExternalLoginProvidersAsync();
                if (loginProviders.Count > 0 && Manager.IsInPopup)
                    throw new InternalError("When using external login providers, the Register module cannot be used in a popup window");
                foreach (LoginConfigDataProvider.LoginProviderDescription provider in loginProviders) {
                    model.ExternalProviders.Add(new FormButton() {
                        ButtonType = ButtonTypeEnum.Submit,
                        Name = "provider",
                        Text = provider.InternalName,
                        Title = this.__ResStr("logAccountTitle", "Log in using your {0} account", provider.DisplayName),
                        CssClass = "t_" + provider.InternalName.ToLower(),
                    });
                    YetaWF.Core.Packages.Package package = AreaRegistration.CurrentPackage;
                    string url = VersionManager.GetAddOnPackageUrl(package.Domain, package.Product);
                    model.Images.Add(Manager.GetCDNUrl(string.Format("{0}Icons/LoginProviders/{1}.png", url, provider.InternalName)));
                }
            }

            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> Register_Partial(RegisterModel model) {
            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            if (!config.AllowUserRegistration)
                throw new Error(this.__ResStr("cantRegister", "This site does not allow new user registration"));

            if (model.ShowCaptcha != config.Captcha)
                throw new InternalError("Hidden field tampering detected");

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
                    UserDefinition userExists = await dataProvider.GetItemAsync(filters);
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
                foreach (var error in result.Errors) {
#if MVC6
                    ModelState.AddModelError("UserName", error.Description);
#else
                    ModelState.AddModelError("UserName", error);
#endif
                }
                return PartialView(model);
            }

            // send appropriate email based on account status
            Emails emails = new Emails();
            if (user.UserStatus == UserStatusEnum.NeedValidation) {
                await emails.SendVerificationAsync(user, config.BccVerification ? Manager.CurrentSite.AdminEmail : null);
                string nextPage = string.IsNullOrWhiteSpace(config.VerificationPendingUrl) ? Manager.CurrentSite.HomePageUrl : config.VerificationPendingUrl;
                return FormProcessed(model, this.__ResStr("okAwaitRegistration", "An email has just been sent to your email address \"{0}\" to complete the registration. Allow a few minutes for delivery. Once received, use the information in the email to complete the registration.", model.Email),
                    this.__ResStr("okRegTitle", "Welcome!"),
                    NextPage: nextPage);
            } else if (user.UserStatus == UserStatusEnum.NeedApproval) {
                await emails.SendApprovalNeededAsync(user);
                string nextPage = string.IsNullOrWhiteSpace(config.ApprovalPendingUrl) ? Manager.CurrentSite.HomePageUrl : config.ApprovalPendingUrl;
                return FormProcessed(model, this.__ResStr("okAwaitApproval", "An email has just been sent to the site administrator for approval of your new account. Once processed and approved, you will receive an email confirming your approval."),
                    this.__ResStr("okRegTitle", "Welcome!"),
                    NextPage: nextPage);
            } else if (user.UserStatus == UserStatusEnum.Approved) {
                if (config.NotifyAdminNewUsers)
                    await emails.SendNewUserCreatedAsync(user);
                await LoginModuleController.UserLoginAsync(user, config.PersistentLogin);
                if (model.CloseOnLogin)
                    return FormProcessed(model, this.__ResStr("okRegText", "Your new account has been successfully registered."), this.__ResStr("okRegTitle", "Welcome!"),
                        OnClose: OnCloseEnum.CloseWindow, OnPopupClose: OnPopupCloseEnum.GotoNewPage, NextPage: Manager.ReturnToUrl, ForceRedirect: true);
                else
                    return FormProcessed(model, this.__ResStr("okRegText", "Your new account has been successfully registered."), this.__ResStr("okRegTitle", "Welcome!"),
                        NextPage: Manager.ReturnToUrl, ForceRedirect: true);
            } else
                throw new InternalError("badUserStatus", "Unexpected account status {0}", user.UserStatus);
        }

        [AllowGet]
        [Permission("ChangeAccounts")]
        [ExcludeDemoMode]
        public async Task<ActionResult> Approve(string userName) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                UserDefinition user = await GetUserAsync(userName, dataProvider);
                if (user.UserStatus != UserStatusEnum.Approved) {
                    if (user.UserStatus != UserStatusEnum.NeedApproval)
                        throw new Error(this.__ResStr("notCantApprove", "User {0} is no longer new and cannot be approved.", userName));
                    user.UserStatus = UserStatusEnum.Approved;
                    UpdateStatusEnum status = await dataProvider.UpdateItemAsync(user);
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
                await emails.SendApprovalAsync(user);
                return Reload(null, Reload: ReloadEnum.ModuleParts, PopupText: this.__ResStr("userApproved", "The user account for user {0} has been marked as approved. An email has been sent to the user.", userName));
            }
        }

        [AllowGet]
        [Permission("ChangeAccounts")]
        [ExcludeDemoMode]
        public async Task<ActionResult> Reject(string userName) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                UserDefinition user = await GetUserAsync(userName, dataProvider);
                if (user.UserStatus != UserStatusEnum.Rejected) {
                    user.UserStatus = UserStatusEnum.Rejected;
                    UpdateStatusEnum status = await dataProvider.UpdateItemAsync(user);
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
                await emails.SendRejectedAsync(user);

                return Reload(null, Reload: ReloadEnum.ModuleParts, PopupText: this.__ResStr("userRejected", "The user account for user {0} has been marked as rejected. An email has been sent to the user.", userName));
            }
        }

        [AllowGet]
        [Permission("ChangeAccounts")]
        [ExcludeDemoMode]
        public async Task<ActionResult> Suspend(string userName) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                UserDefinition user = await GetUserAsync(userName, dataProvider);
                if (user.UserStatus != UserStatusEnum.Suspended) {
                    user.UserStatus = UserStatusEnum.Suspended;
                    UpdateStatusEnum status = await dataProvider.UpdateItemAsync(user);
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
                await emails.SendSuspendedAsync(user);

                return Reload(null, Reload: ReloadEnum.ModuleParts, PopupText: this.__ResStr("userSuspended", "The user account for user {0} has been marked as suspended. An email has been sent to the user.", userName));
            }
        }
        private async Task<UserDefinition> GetUserAsync(string userName, UserDefinitionDataProvider dataProvider) {
            if (string.IsNullOrWhiteSpace(userName))
                throw new Error(this.__ResStr("noItem", "No user name specified"));
            UserDefinition user = await dataProvider.GetItemAsync(userName);
            if (user == null)
                throw new Error(this.__ResStr("notFoundUser", "User {0} not found.", userName));
            return user;
        }
    }
}