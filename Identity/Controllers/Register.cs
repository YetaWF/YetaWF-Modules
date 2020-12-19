/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Addons;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Models;
using YetaWF.Modules.Identity.Modules;
using YetaWF.Modules.Identity.Support;
using YetaWF.Core.Components;
using YetaWF.Core.Extensions;
using YetaWF.Core.Identity;
using System.Linq;
using YetaWF.Core.Modules;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
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
                Actions = new List<ModuleAction>();
            }

            [Caption("User Name"), Description("Enter your user name to register")]
            [UIHint("Text40"), StringLength(Globals.MaxUser), UserNameValidation, Trim]
            [SuppressIf(nameof(RegistrationType), RegistrationTypeEnum.EmailOnly)]
            [RequiredIfNot(nameof(RegistrationType), RegistrationTypeEnum.EmailOnly)]
            public string UserName { get; set; }

            [Caption("Email Address"), Description("Enter your email address to register - This is the email address used by this site to communicate with you")]
            [UIHint("Email"), StringLength(Globals.MaxEmail), EmailValidation, Trim]
            [SuppressIf(nameof(RegistrationType), RegistrationTypeEnum.NameOnly)]
            [RequiredIfNot(nameof(RegistrationType), RegistrationTypeEnum.NameOnly)]
            public string Email { get; set; }

            [Caption("Partner"), Description("Enter your partner name")]
            [UIHint("Email"), StringLength(40), Trim]
            public string PartnerName { get; set; }

            [Caption("Password"), Description("Enter your desired password")]
            [UIHint("Password20"), StringLength(Globals.MaxPswd), Required]
            public string Password { get; set; }
            public string Password_AutoComplete { get { return "new-password"; } }

            [Caption(" "), Description("")]
            [UIHint("String"), ReadOnly]
            [SuppressEmpty]
            public string PasswordRules { get; set; }

            [Caption("Password Confirmation"), Description("Enter your password again to confirm")]
            [UIHint("Password20"), Required, StringLength(Globals.MaxPswd), SameAs("Password", "The password confirmation doesn't match the password entered")]
            public string ConfirmPassword { get; set; }
            public string ConfirmPassword_AutoComplete { get { return "new-password"; } }

            [Caption("Captcha"), Description("Please verify that you're a human and not a spam bot")]
            [UIHint("RecaptchaV2"), RecaptchaV2("Please verify that you're a human and not a spam bot"), SuppressIf(nameof(ShowCaptcha), false)]
            public RecaptchaV2Data Captcha { get; set; }

            [Caption(""), Description("")]
            [UIHint("ModuleActions"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.NormalLinks), ReadOnly]
            [SuppressEmpty]
            public List<ModuleAction> Actions { get; set; }

            [UIHint("Hidden"), ReadOnly]
            public RegistrationTypeEnum RegistrationType { get; set; }
            [UIHint("Hidden"), ReadOnly]
            public bool ShowCaptcha { get; set; }

            [UIHint("Hidden"), ReadOnly]
            public bool CloseOnLogin { get; set; }

            [UIHint("Hidden"), ReadOnly]
            public string QueryString { get; set; }

            public List<string> Images { get; set; }
            public List<FormButton> ExternalProviders { get; set; }

            [UIHint("Hidden"), ReadOnly]
            public string ReturnUrl { get; set; } // for external login only

            public async Task UpdateAsync() {
                LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
                LoginModule loginMod = (LoginModule)await ModuleDefinition.CreateUniqueModuleAsync(typeof(LoginModule));
                bool closeOnLogin;
                Manager.TryGetUrlArg<bool>("CloseOnLogin", out closeOnLogin, false);
                ModuleAction logAction = await loginMod.GetAction_LoginAsync(config.LoginUrl, Force: true, CloseOnLogin: closeOnLogin);
                if (logAction != null)
                    logAction.AddToOriginList = false;
                Actions.New(logAction);
            }
        }

        [AllowGet]
        public async Task<ActionResult> Register(bool closeOnLogin = false) {

            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            if (!config.AllowUserRegistration)
                throw new Error(this.__ResStr("cantRegister", "This site does not allow new user registration"));

            using (LoginConfigDataProvider logConfigDP = new LoginConfigDataProvider()) {

                RegisterModel model = new RegisterModel {
                    RegistrationType = config.RegistrationType,
                    ShowCaptcha = config.Captcha && !Manager.IsLocalHost,
                    Captcha = new RecaptchaV2Data(),
                    CloseOnLogin = closeOnLogin,
                    QueryString = Manager.RequestQueryString.ToQueryString(),
                    PasswordRules = Module.ShowPasswordRules ? logConfigDP.PasswordRules : null,
                };

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
                    string url = VersionManager.GetAddOnPackageUrl(package.AreaName);
                    model.Images.Add(Manager.GetCDNUrl(string.Format("{0}Icons/LoginProviders/{1}.png", url, provider.InternalName)));
                }
                if (Manager.HaveReturnToUrl)
                    model.ReturnUrl = Manager.ReturnToUrl;

                await model.UpdateAsync();
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> Register_Partial(RegisterModel model) {
            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            if (!config.AllowUserRegistration)
                throw new Error(this.__ResStr("cantRegister", "This site does not allow new user registration"));

            if (model.ShowCaptcha != config.Captcha && !Manager.IsLocalHost)
                throw new InternalError("Hidden field tampering detected");
            if (!model.ShowCaptcha && ModelState[nameof(model.Captcha)] != null)
                ModelState[nameof(model.Captcha)].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;

            if (!string.IsNullOrWhiteSpace(model.PartnerName))// bot trap
                return NotAuthorized();

            model.RegistrationType = config.RegistrationType;// don't trust what we get from user
            if (Module.ShowPasswordRules) {
                using (LoginConfigDataProvider logConfigDP = new LoginConfigDataProvider()) {
                    model.PasswordRules = logConfigDP.PasswordRules;
                }
            }

            await model.UpdateAsync();
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

            user.PasswordPlainText = config.SavePlainTextPassword ? model.Password : null;

            if (config.RegistrationType == RegistrationTypeEnum.NameAndEmail) {
                using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                    // Email == user.Email
                    List<DataProviderFilterInfo> filters = new List<DataProviderFilterInfo> {
                        new DataProviderFilterInfo {
                            Field = nameof(UserDefinition.Email), Operator = "==", Value = user.Email,
                        },
                    };
                    UserDefinition userExists = await dataProvider.GetItemAsync(filters);
                    if (userExists != null && user.UserName != userExists.Email) {
                        ModelState.AddModelError(nameof(model.Email), this.__ResStr("emailUsed", "An account with email address {0} already exists.", user.Email));
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
                    ModelState.AddModelError(nameof(model.UserName), error.Description);
                    ModelState.AddModelError(nameof(model.Email), error.Description);
                    ModelState.AddModelError(nameof(model.Password), error.Description);
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

                string nextUrl = null;
                if (Manager.HaveReturnToUrl)
                    nextUrl = Manager.ReturnToUrl;
                if (string.IsNullOrWhiteSpace(nextUrl) && !string.IsNullOrWhiteSpace(Module.PostRegisterUrl)) {
                    nextUrl = Module.PostRegisterUrl;
                    if (Module.PostRegisterQueryString)
                        nextUrl += model.QueryString.AddQSSeparator() + model.QueryString;
                }
                if (string.IsNullOrWhiteSpace(nextUrl))
                    nextUrl = await Resource.ResourceAccess.GetUserPostLoginUrlAsync((from u in user.RolesList select u.RoleId).ToList());
                if (string.IsNullOrWhiteSpace(nextUrl))
                    nextUrl = Manager.CurrentSite.PostLoginUrl;

                if (!string.IsNullOrWhiteSpace(nextUrl)) {
                    return FormProcessed(model, this.__ResStr("okRegText", "Your new account has been successfully registered."), this.__ResStr("okRegTitle", "Welcome!"),
                        OnClose: OnCloseEnum.GotoNewPage, OnPopupClose: OnPopupCloseEnum.GotoNewPage, NextPage: nextUrl, ForceRedirect: true);
                }
                return FormProcessed(model, this.__ResStr("okRegText", "Your new account has been successfully registered."), this.__ResStr("okRegTitle", "Welcome!"), ForceRedirect: true);

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
