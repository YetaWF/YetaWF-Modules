/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Log;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core.Support.TwoStepAuthorization;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Models;
using YetaWF.Modules.Identity.Modules;
using YetaWF.Modules.Identity.Support;

namespace YetaWF.Modules.Identity.Controllers {

    public class LoginModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.LoginModule> {

        [Legend("Account Information")]
        public class LoginModel {

            public LoginModel() {
                Captcha = new RecaptchaV2Data();
                ExternalProviders = new List<FormButton>();
                Images = new List<string>();
            }

            [Caption(""), Description("")]
            [UIHint("Raw"), ReadOnly]
            [SuppressIfNot(nameof(RegistrationType), RegistrationTypeEnum.NameAndEmail)]
            [SuppressIfNot(nameof(AllowNewUser), true)]
            public string HeaderNameAndEmailNewUser { get { return this.__ResStr("headerNameEmailNew", "<p>You are entering an area of our web site, for which you need to register using your name and password. If you are a new user, please register a new account now. If you already established an account earlier, please enter the account information here and click \"Log In\".</p>"); } }

            [Caption(""), Description("")]
            [UIHint("Raw"), ReadOnly]
            [SuppressIfNot(nameof(RegistrationType), RegistrationTypeEnum.EmailOnly)]
            [SuppressIfNot(nameof(AllowNewUser), true)]
            public string HeaderEmailNewUser { get { return this.__ResStr("headerEmailNew", "<p>You are entering an area of our web site, for which you need to register using your email address and password. If you are a new user, please register a new account now. If you already established an account earlier, please enter the account information here and click \"Log In\".</p>"); } }

            [Caption(""), Description("")]
            [UIHint("Raw"), ReadOnly]
            [SuppressIfNot(nameof(RegistrationType), RegistrationTypeEnum.NameAndEmail)]
            [SuppressIfNot(nameof(AllowNewUser), false)]
            public string HeaderNameAndEmail { get { return this.__ResStr("headerNameEmail", "<p>If you have an account, please enter the account information here and click \"Log In\". This site does not accept new user registrations.</p>"); } }

            [Caption(""), Description("")]
            [UIHint("Raw"), ReadOnly]
            [SuppressIfNot(nameof(RegistrationType), RegistrationTypeEnum.EmailOnly)]
            [SuppressIfNot(nameof(AllowNewUser), false)]
            public string HeaderEmail { get { return this.__ResStr("headerName", "<p>If you have an account, please enter the account information here and click \"Log In\". This site does not accept new user registrations.</p>"); } }

            [Caption("Name"), Description("Enter your user name - This is the name you used when you registered on this site")]
            [UIHint("Text40"), StringLength(Globals.MaxUser), UserNameValidation, Trim]
            [ProcessIf(nameof(RegistrationType), RegistrationTypeEnum.NameAndEmail)]
            [ProcessIf(nameof(RegistrationType), RegistrationTypeEnum.NameOnly)]
            [RequiredIf(nameof(RegistrationType), RegistrationTypeEnum.NameAndEmail)]
            [RequiredIf(nameof(RegistrationType), RegistrationTypeEnum.NameOnly)]
            public string UserName { get; set; }

            [Caption("Email Address"), Description("Enter your email address to register - This is the email address used by this site to communicate with you")]
            [UIHint("Email"), EmailValidation, Trim]
            [ProcessIf(nameof(RegistrationType), RegistrationTypeEnum.EmailOnly)]
            [RequiredIf(nameof(RegistrationType), RegistrationTypeEnum.EmailOnly)]
            public string Email { get; set; }

            [Caption("Password"), Description("Enter your password")]
            [UIHint("Password20"), StringLength(Globals.MaxPswd), Required, Trim]
            public string Password { get; set; }

            [UIHint("Boolean")]
            [Caption("Remember Me"), Description("Select this option so your information is saved and you don't need to login again when you return to this site (when using the same browser)")]
            public bool RememberMe { get; set; }

            [Caption("Verification Code"), Description("Please enter the verification code you received via email to validate your account")]
            [UIHint("Text80"), StringLength(UserDefinition.MaxVerificationCode), Trim]
            [SuppressIf(nameof(ShowVerification), false)]
            public string VerificationCode { get; set; }

            [Caption(""), Description("")]
            [UIHint("ModuleAction"), ReadOnly, SuppressEmpty]
            public ModuleAction ResendVerificationCode { get; set; }

            [Caption("Captcha"), Description("Please verify that you're a human and not a spam bot")]
            [UIHint("RecaptchaV2"), RecaptchaV2("Please verify that you're a human and not a spam bot")]
            [SuppressIf(nameof(ShowCaptcha), false)]
            public RecaptchaV2Data Captcha { get; set; }

            [UIHint("Hidden"), ReadOnly]
            public bool ShowVerification { get; set; }
            [UIHint("Hidden"), ReadOnly]
            public bool ShowCaptcha { get; set; }
            [UIHint("Hidden"), ReadOnly]
            public bool CloseOnLogin { get; set; }

            public List<string> Images { get; set; }
            public List<FormButton> ExternalProviders { get; set; }

            public bool Success { get; set; }

            [UIHint("Hidden"), ReadOnly]
            public RegistrationTypeEnum RegistrationType { get; set; }
            [UIHint("Hidden"), ReadOnly]
            public bool AllowNewUser { get; set; }
            [UIHint("Hidden"), ReadOnly]
            public string ReturnUrl { get; set; } // for external login only
        }

        [AllowGet]
        public async Task<ActionResult> Login(string name, string pswd, string v, bool closeOnLogin = false, bool __f = false) {

            // add popup support for possible 2fa
            await YetaWFCoreRendering.Render.AddPopupsAddOnsAsync();

            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            bool isPersistent = config.PersistentLogin;

            LoginModel model = new LoginModel {
                AllowNewUser = config.AllowUserRegistration,
                RegistrationType = config.RegistrationType,
                UserName = name,
                Email = name,
                Password = pswd,
                VerificationCode = v,
                Captcha = new RecaptchaV2Data(),
                RememberMe = isPersistent,
                CloseOnLogin = closeOnLogin,
                ShowVerification = !string.IsNullOrWhiteSpace(v),
                ShowCaptcha = config.Captcha && string.IsNullOrWhiteSpace(v) && !Manager.IsLocalHost,
            };
            if (Manager.HaveReturnToUrl)
                model.ReturnUrl = Manager.ReturnToUrl;

            using (LoginConfigDataProvider logConfigDP = new LoginConfigDataProvider()) {
                List<LoginConfigDataProvider.LoginProviderDescription> loginProviders = await logConfigDP.GetActiveExternalLoginProvidersAsync();
                if (loginProviders.Count > 0 && Manager.IsInPopup)
                    throw new InternalError("When using external login providers, the Login module cannot be used in a popup window");
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
            }
            if (__f)
                Manager.CurrentResponse.StatusCode = 401;

            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> Login_Partial(LoginModel model) {

            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            model.AllowNewUser = config.AllowUserRegistration;
            model.RegistrationType = config.RegistrationType;

            if (model.ShowCaptcha != (config.Captcha && !model.ShowVerification && !Manager.IsLocalHost))
                throw new InternalError("Hidden field tampering detected");

            if (!ModelState.IsValid && config.RegistrationType == RegistrationTypeEnum.EmailOnly) {
                if (ModelState[nameof(model.Email)] != null && ModelState[nameof(model.Email)].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid) {
                    // we allow the superuser name as login in case we use email registration and the superuser name is not a valid email address
                    if (string.Compare(model.Email, SuperuserDefinitionDataProvider.SuperUserName, true) == 0)
                        ModelState.Remove(nameof(model.Email));
                }
            }
            if (!model.ShowCaptcha && ModelState[nameof(model.Captcha)] != null)
                ModelState[nameof(model.Captcha)].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;

            if (!ModelState.IsValid)
                return PartialView(model);

            if (config.RegistrationType == RegistrationTypeEnum.EmailOnly)
                model.UserName = model.Email;

            return await CompleteLoginAsync(model, config, useTwoStep: true);
        }

        [AllowPost]
        [ExcludeDemoMode]
        public async Task<ActionResult> LoginDirect(string name, string password, Guid security) {

            Package package = AreaRegistration.CurrentPackage;
            Guid batchKey = WebConfigHelper.GetValue<Guid>(package.AreaName, "BatchKey");
            if (batchKey != security)
                return NotAuthorized();

            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            LoginModel model = new LoginModel {
                AllowNewUser = config.AllowUserRegistration,
                RegistrationType = config.RegistrationType,
                RememberMe = true,
                UserName = name,
                Password = password,
            };

            ActionResult result = await CompleteLoginAsync(model, await LoginConfigDataProvider.GetConfigAsync(), useTwoStep: false);
            if (!model.Success)
                Manager.CurrentResponse.StatusCode = 401;
            return result;
        }

        [AllowGet]
        [ExcludeDemoMode]
        public async Task<ActionResult> LoginDirectGet(string name, string password, Guid security) {

            Package package = AreaRegistration.CurrentPackage;
            Guid batchKey = WebConfigHelper.GetValue<Guid>(package.AreaName, "BatchKey");
            if (batchKey != security)
                return NotAuthorized();

            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            LoginModel model = new LoginModel {
                AllowNewUser = config.AllowUserRegistration,
                RegistrationType = config.RegistrationType,
                RememberMe = true,
                UserName = name,
                Password = password
            };

            ActionResult result = await CompleteLoginAsync(model, await LoginConfigDataProvider.GetConfigAsync(), useTwoStep: false);
            if (!model.Success)
                Manager.CurrentResponse.StatusCode = 401;
            return new EmptyResult();
        }

        private async Task<ActionResult> CompleteLoginAsync(LoginModel model, LoginConfigData config, bool useTwoStep) {

            Manager.SessionSettings.SiteSettings.ClearValue(LoginTwoStepController.IDENTITY_TWOSTEP_USERID);
            Manager.SessionSettings.SiteSettings.ClearValue(LoginTwoStepController.IDENTITY_TWOSTEP_NEXTURL);
            Manager.SessionSettings.SiteSettings.ClearValue(LoginTwoStepController.IDENTITY_TWOSTEP_CLOSEONLOGIN);
            Manager.SessionSettings.SiteSettings.Save();
            model.Success = false;

            // make sure it's a valid user
            UserDefinition user = await Managers.GetUserManager().FindByNameAsync(model.UserName);
            if (user == null) {
                Logging.AddErrorLog("User login failed: {0} - no such user", model.UserName);
                ModelState.AddModelError(nameof(LoginModel.Email), this.__ResStr("invLogin", "Invalid user name or password"));
                ModelState.AddModelError(nameof(LoginModel.UserName), this.__ResStr("invLogin", "Invalid user name or password"));
                ModelState.AddModelError(nameof(LoginModel.Password), this.__ResStr("invLogin", "Invalid user name or password"));
                return PartialView(model);
            }
            using (UserLoginInfoDataProvider logInfoDP = new UserLoginInfoDataProvider()) {
                if (await logInfoDP.IsExternalUserAsync(user.UserId))
                    throw new Error(this.__ResStr("extUser", "This account can only be accessed using an external login provider"));
            }

            TwoStepAuth twoStep = new TwoStepAuth();// clear any two-step info we may have
            await twoStep.ClearTwoStepAutheticationAsync(user.UserId);

            if (config.MaxLoginFailures != 0 && user.LoginFailures >= config.MaxLoginFailures) {
                ModelState.AddModelError(nameof(LoginModel.Email), this.__ResStr("maxAttemps", "The maximum number of login attempts has been exceeded - Your account has been suspended"));
                ModelState.AddModelError(nameof(LoginModel.UserName), this.__ResStr("maxAttemps", "The maximum number of login attempts has been exceeded - Your account has been suspended"));
                if (user.UserStatus != UserStatusEnum.Suspended) {
                    user.UserStatus = UserStatusEnum.Suspended;
                    await Managers.GetUserManager().UpdateAsync(user);
                }
                return PartialView(model);
            }

            UserDefinition foundUser = user;
            user = null;

            // Handle random super user password (only supported on Core)
            if (foundUser.UserId == SuperuserDefinitionDataProvider.SuperUserId && SuperuserDefinitionDataProvider.SuperuserAvailable && SuperuserDefinitionDataProvider.SuperUserPasswordRandom &&
                    model.UserName == SuperuserDefinitionDataProvider.SuperUserName && model.Password == SuperuserDefinitionDataProvider.SuperUserPassword) {
                user = foundUser;
            }

            if (user == null) {
                user = await Managers.GetUserManager().FindByNameAsync(model.UserName);
                if (string.IsNullOrWhiteSpace(model.Password) || !await Managers.GetUserManager().CheckPasswordAsync(user, model.Password))
                    user = null;
            }
            if (user == null) {
                foundUser.LoginFailures = foundUser.LoginFailures + 1;
                await Managers.GetUserManager().UpdateAsync(foundUser);

                Logging.AddErrorLog("User login failed: {0}, {1}, {2}", model.UserName, model.Password, model.VerificationCode);
                ModelState.AddModelError(nameof(LoginModel.Email), this.__ResStr("invLogin", "Invalid user name or password"));
                ModelState.AddModelError(nameof(LoginModel.UserName), this.__ResStr("invLogin", "Invalid user name or password"));
                ModelState.AddModelError(nameof(LoginModel.Password), this.__ResStr("invLogin", "Invalid user name or password"));
                return PartialView(model);
            }

            // if verification code valid, advance user to approved or needs approval
            if (user.UserStatus == UserStatusEnum.NeedValidation && model.VerificationCode == user.VerificationCode) {
                Logging.AddLog("User {0} validated ({1})", model.UserName, model.VerificationCode);

                if (config.ApproveNewUsers) {
                    user.UserStatus = UserStatusEnum.NeedApproval;
                    user.LastActivityDate = DateTime.UtcNow;
                    user.LastActivityIP = Manager.UserHostAddress;
                    await Managers.GetUserManager().UpdateAsync(user);

                    Emails emails = new Emails();
                    await emails.SendApprovalNeededAsync(user);

                    string nextPage = string.IsNullOrWhiteSpace(config.ApprovalPendingUrl) ? Manager.CurrentSite.HomePageUrl : config.ApprovalPendingUrl;
                    return FormProcessed(model,
                        this.__ResStr("notApproved", "You just verified your account. Now your account has to be approved by the site administrator. You will receive an email confirmation as soon as your account is active."),
                        NextPage: nextPage);
                }
                user.UserStatus = UserStatusEnum.Approved;
                // this is saved below, before we're logged in
            }

            // check what to do based on account status
            if (user.UserStatus == UserStatusEnum.NeedValidation) {
                if (model.ShowVerification) {
                    Logging.AddErrorLog("User {0} - invalid verification code({1})", model.UserName, model.VerificationCode);
                    ModelState.AddModelError(nameof(LoginModel.VerificationCode), this.__ResStr("invVerification", "The verification code is invalid. Please make sure to copy/paste it from the email to avoid any typos."));
                    user.LoginFailures = user.LoginFailures + 1;
                    await Managers.GetUserManager().UpdateAsync(user);
                } else
                    ModelState.AddModelError(nameof(LoginModel.VerificationCode), this.__ResStr("notValidated", "Your account has not yet been verified. You will receive an email with verification information. Please copy and enter the verification code here."));
                model.ShowVerification = true;
                model.ResendVerificationCode = await Module.GetAction_ResendVerificationEmailAsync(user.UserName);
                model.ShowCaptcha = false;
                return PartialView(model);
            } else if (user.UserStatus == UserStatusEnum.NeedApproval) {
                Logging.AddErrorLog("User {0} - not yet approved", model.UserName);
                string nextPage = string.IsNullOrWhiteSpace(config.ApprovalPendingUrl) ? Manager.CurrentSite.HomePageUrl : config.ApprovalPendingUrl;
                return FormProcessed(model,
                    this.__ResStr("notApproved2", "Your account has not yet been approved by the site administrator. You will receive an email confirmation as soon as your account is active."),
                    NextPage: nextPage);
            } else if (user.UserStatus == UserStatusEnum.Rejected) {
                Logging.AddErrorLog("User {0} - rejected user", model.UserName);
                string nextPage = string.IsNullOrWhiteSpace(config.RejectedUrl) ? Manager.CurrentSite.HomePageUrl : config.RejectedUrl;
                return FormProcessed(model,
                    this.__ResStr("accountRejected", "Your account has been rejected by the site administrator."),
                    NextPage: nextPage);
            } else if (user.UserStatus == UserStatusEnum.Suspended) {
                Logging.AddErrorLog("User {0} - suspended user", model.UserName);
                string nextPage = string.IsNullOrWhiteSpace(config.SuspendedUrl) ? Manager.CurrentSite.HomePageUrl : config.SuspendedUrl;
                return FormProcessed(model,
                    this.__ResStr("accountSuspended", "Your account has been suspended."),
                    NextPage: nextPage);
            } else if (user.UserStatus == UserStatusEnum.Approved) {

                string nextUrl = null;
                if (Manager.HaveReturnToUrl)
                    nextUrl = Manager.ReturnToUrl;
                if (string.IsNullOrWhiteSpace(nextUrl))
                    nextUrl = await Resource.ResourceAccess.GetUserPostLoginUrlAsync((from u in user.RolesList select u.RoleId).ToList());
                if (string.IsNullOrWhiteSpace(nextUrl))
                    nextUrl = Manager.CurrentSite.PostLoginUrl;

                if (useTwoStep) {
                    ActionResult actionResult = await TwoStepAuthetication(user);
                    if (actionResult != null) {
                        Manager.SessionSettings.SiteSettings.SetValue<int>(LoginTwoStepController.IDENTITY_TWOSTEP_USERID, user.UserId);// marker that user has entered correct name/password
                        Manager.SessionSettings.SiteSettings.SetValue<string>(LoginTwoStepController.IDENTITY_TWOSTEP_NEXTURL, nextUrl);
                        Manager.SessionSettings.SiteSettings.SetValue<bool>(LoginTwoStepController.IDENTITY_TWOSTEP_CLOSEONLOGIN, model.CloseOnLogin);
                        Manager.SessionSettings.SiteSettings.Save();
                        return actionResult;
                    }
                }
                await LoginModuleController.UserLoginAsync(user, model.RememberMe);
                model.Success = true;
                Logging.AddLog("User {0} - logged on", model.UserName);

                if (!string.IsNullOrWhiteSpace(nextUrl))
                    return FormProcessed(model, OnClose: OnCloseEnum.GotoNewPage, OnPopupClose: OnPopupCloseEnum.GotoNewPage, NextPage: nextUrl, ForceRedirect: true);
                return FormProcessed(model, ForceRedirect: true);

            } else
                throw new InternalError("badUserStatus", "Unexpected account status {0}", user.UserStatus);
        }

        /// <summary>
        /// Returns an action result to start the two-step authentication process (if any).
        /// </summary>
        private async Task<ActionResult> TwoStepAuthetication(UserDefinition user) {
            TwoStepAuth twoStep = new TwoStepAuth();
            List<string> enabledTwoStepAuthentications = (from e in user.EnabledTwoStepAuthentications select e.Name).ToList();
            ModuleAction action = await twoStep.GetLoginActionAsync(enabledTwoStepAuthentications, user.UserId, user.UserName, user.Email);
            if (action == null)
                return null;
            return Redirect(action);
        }

        // User logoff
        internal static async Task UserLogoffAsync() {
            Manager.EditMode = false;

            SignInManager<UserDefinition> _signinManager = (SignInManager<UserDefinition>)YetaWFManager.ServiceProvider.GetService(typeof(SignInManager<UserDefinition>));
            await _signinManager.SignOutAsync();

            Manager.SessionSettings.ClearAll();
        }

        // User login
        public static async Task UserLoginAsync(UserDefinition user, bool? rememberme = null) {

            await LoginModuleController.UserLogoffAsync();

            bool isPersistent;
            if (rememberme == null) {
                LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
                isPersistent = config.PersistentLogin;
            } else {
                isPersistent = (bool) rememberme;
            }

            SignInManager<UserDefinition> _signinManager = (SignInManager<UserDefinition>)YetaWFManager.ServiceProvider.GetService(typeof(SignInManager<UserDefinition>));
            await _signinManager.SignInAsync(user, isPersistent);

            // superuser
            int superuserRole = Resource.ResourceAccess.GetSuperuserRoleId();
            if (user.RolesList != null && user.RolesList.Contains(new Role { RoleId = superuserRole }, new RoleComparer()))
                Manager.SetSuperUserRole(true);

            // accept user
            user.LastLoginDate = DateTime.UtcNow;
            user.LastLoginIP = Manager.UserHostAddress;
            user.LastActivityDate = DateTime.UtcNow;
            user.LastActivityIP = Manager.UserHostAddress;
            user.LoginFailures = 0;
            await Managers.GetUserManager().UpdateAsync(user);
        }

        [AllowPost]
        [ExcludeDemoMode]
        public async Task<ActionResult> ResendVerificationEmail(string userName) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                if (string.IsNullOrWhiteSpace(userName))
                    throw new Error(this.__ResStr("noItem", "Unable to send verification email (Reason Code 1)"));
                UserDefinition user = await dataProvider.GetItemAsync(userName);
                if (user == null)
                    throw new Error(this.__ResStr("notFoundUser", "Unable to send verification email (Reason Code 2)"));
                if (user.UserStatus != UserStatusEnum.NeedValidation)
                    throw new Error(this.__ResStr("noNeed", "Unable to send verification email (Reason Code 3)"));
                Emails emails = new Emails();
                await emails.SendVerificationAsync(user);
                return Reload(null, Reload: ReloadEnum.ModuleParts, PopupText: this.__ResStr("verificationSent", "Verification email sent."));
            }
        }
    }
}
