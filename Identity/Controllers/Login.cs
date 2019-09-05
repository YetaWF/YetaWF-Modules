/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Addons;
using YetaWF.Core.Controllers;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Log;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Core.Support.TwoStepAuthorization;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Models;
using YetaWF.Modules.Identity.Support;
using YetaWF.Core.Packages;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
#if MVC6
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
#else
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Web;
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class LoginModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.LoginModule> {

        public class LoginModel {

            public LoginModel() {
                Captcha = new RecaptchaV2Data();
                ExternalProviders = new List<FormButton>();
                Images = new List<string>();
            }

            public virtual string UserName { get; set; }
            public virtual string Password { get; set; }
            public virtual bool RememberMe { get; set; }
            public virtual string VerificationCode { get; set; }

            [Caption(""), Description("")]
            [UIHint("ModuleAction"), ReadOnly, SuppressEmpty]
            public ModuleAction ResendVerificationCode { get; set; }

            [Caption("Captcha"), Description("Please verify that you're a human and not a spam bot")]
            [UIHint("RecaptchaV2"), RecaptchaV2("Please verify that you're a human and not a spam bot"), SuppressIf("ShowCaptcha", false)]
            public RecaptchaV2Data Captcha { get; set; }

            [UIHint("Hidden")]
            public bool ShowVerification { get; set; }
            [UIHint("Hidden")]
            public bool ShowCaptcha { get; set; }
            [UIHint("Hidden")]
            public bool CloseOnLogin { get; set; }

            public List<string> Images { get; set; }
            public List<FormButton> ExternalProviders { get; set; }

            public bool Success { get; set; }
        }


        [Trim]
        [Header("-<p>You are entering an area of our web site, for which you need to register using your name and password. " +
                "If you are a new user, please register a new account now. If you already established an account earlier, please enter the account information here and click \"Log In\".</p>")]
        [Legend("Account Information")]
        public class LoginModelNameAllowNew : LoginModel {

            public LoginModelNameAllowNew() {
                Captcha = new RecaptchaV2Data();
                ExternalProviders = new List<FormButton>();
                Images = new List<string>();
            }

            [Caption("Name"), Description("Enter your user name - This is the name you used when you registered on this site")]
            [UIHint("Text40"), StringLength(Globals.MaxUser), UserNameValidation, Required, Trim]
            public override string UserName { get { return base.UserName; } set { base.UserName = value; } }

            [Caption("Password"), Description("Enter your password")]
            [UIHint("Password20"), StringLength(Globals.MaxPswd), Trim]
            public override string Password { get { return base.Password; } set { base.Password = value; } }

            [UIHint("Boolean")]
            [Caption("Remember Me"), Description("Select this option so your information is saved and you don't need to login again when you return to this site (when using the same browser)")]
            public override bool RememberMe { get { return base.RememberMe; } set { base.RememberMe = value; } }

            [Caption("Verification Code"), Description("Please enter the verification code you received via email to validate your account")]
            [UIHint("Text40"), StringLength(UserDefinition.MaxVerificationCode), Trim, SuppressIf("ShowVerification", false)]
            public override string VerificationCode { get { return base.VerificationCode; } set { base.VerificationCode = value; } }
        }

        [Trim]
        [Header("-<p>You are entering an area of our web site, for which you need to register using your email address and password. " +
                "If you are a new user, please register a new account now. If you already established an account earlier, please enter the account information here and click \"Log In\".</p>")]
        [Legend("Account Information")]
        public class LoginModelEmailAllowNew : LoginModel {

            public LoginModelEmailAllowNew() {
                Captcha = new RecaptchaV2Data();
                ExternalProviders = new List<FormButton>();
                Images = new List<string>();
            }

            [Caption("Email Address"), Description("Enter your email address - This is the email address you used when you registered on this site")]
            [UIHint("Text40"), StringLength(Globals.MaxUser), UserNameValidation, Required, Trim]
            public override string UserName { get { return base.UserName; } set { base.UserName = value; } }

            [Caption("Password"), Description("Enter your password")]
            [UIHint("Password20"), StringLength(Globals.MaxPswd), Trim]
            public override string Password { get { return base.Password; } set { base.Password = value; } }

            [UIHint("Boolean")]
            [Caption("Remember Me"), Description("Select this option so your information is saved and you don't need to login again when you return to this site (when using the same browser)")]
            public override bool RememberMe { get { return base.RememberMe; } set { base.RememberMe = value; } }

            [Caption("Verification Code"), Description("Please enter the verification code you received via email to validate your account")]
            [UIHint("Text40"), StringLength(UserDefinition.MaxVerificationCode), Trim, SuppressIf("ShowVerification", false)]
            public override string VerificationCode { get { return base.VerificationCode; } set { base.VerificationCode = value; } }
        }

        [Trim]
        [Header("-<p>If you have an account, please enter the account information here and click \"Log In\". This site does not accept new user registrations.</p>")]
        [Legend("Account Information")]
        public class LoginModelNameNoNew : LoginModel {

            public LoginModelNameNoNew() {
                Captcha = new RecaptchaV2Data();
                ExternalProviders = new List<FormButton>();
                Images = new List<string>();
            }

            [Caption("Name"), Description("Enter your user name - This is the name you used when you registered on this site")]
            [UIHint("Text40"), StringLength(Globals.MaxUser), UserNameValidation, Required, Trim]
            public override string UserName { get { return base.UserName; } set { base.UserName = value; } }

            [Caption("Password"), Description("Enter your password")]
            [UIHint("Password20"), StringLength(Globals.MaxPswd), Trim]
            public override string Password { get { return base.Password; } set { base.Password = value; } }

            [UIHint("Boolean")]
            [Caption("Remember Me"), Description("Select this option so your information is saved and you don't need to login again when you return to this site (when using the same browser)")]
            public override bool RememberMe { get { return base.RememberMe; } set { base.RememberMe = value; } }

            [Caption("Verification Code"), Description("Please enter the verification code you received via email to validate your account")]
            [UIHint("Text40"), StringLength(UserDefinition.MaxVerificationCode), Trim, SuppressIf("ShowVerification", false)]
            public override string VerificationCode { get { return base.VerificationCode; } set { base.VerificationCode = value; } }
        }

        [Trim]
        [Header("-<p>If you have an account, please enter the account information here and click \"Log In\". This site does not accept new user registrations.</p>")]
        [Legend("Account Information")]
        public class LoginModelEmailNoNew : LoginModel {

            public LoginModelEmailNoNew() {
                Captcha = new RecaptchaV2Data();
                ExternalProviders = new List<FormButton>();
                Images = new List<string>();
            }

            [Caption("Email Address"), Description("Enter your email address - This is the email address you used when you registered on this site")]
            [UIHint("Text40"), StringLength(Globals.MaxUser), UserNameValidation, Required, Trim]
            public override string UserName { get { return base.UserName; } set { base.UserName = value; } }

            [Caption("Password"), Description("Enter your password")]
            [UIHint("Password20"), StringLength(Globals.MaxPswd), Trim]
            public override string Password { get { return base.Password; } set { base.Password = value; } }

            [UIHint("Boolean")]
            [Caption("Remember Me"), Description("Select this option so your information is saved and you don't need to login again when you return to this site (when using the same browser)")]
            public override bool RememberMe { get { return base.RememberMe; } set { base.RememberMe = value; } }

            [Caption("Verification Code"), Description("Please enter the verification code you received via email to validate your account")]
            [UIHint("Text40"), StringLength(UserDefinition.MaxVerificationCode), Trim, SuppressIf("ShowVerification", false)]
            public override string VerificationCode { get { return base.VerificationCode; } set { base.VerificationCode = value; } }
        }

        private LoginModel GetLoginModel(LoginConfigData config, LoginModel origModel = null) {
            LoginModel model;
            if (config.AllowUserRegistration) {
                switch (config.RegistrationType) {
                    default:
                    case Modules.RegistrationTypeEnum.EmailOnly:
                        model = new LoginModelEmailAllowNew();
                        break;
                    case Modules.RegistrationTypeEnum.NameOnly:
                        model = new LoginModelNameAllowNew();
                        break;
                    case Modules.RegistrationTypeEnum.NameAndEmail:
                        model = new LoginModelNameAllowNew();
                        break;
                }
            } else {
                switch (config.RegistrationType) {
                    default:
                    case Modules.RegistrationTypeEnum.EmailOnly:
                        model = new LoginModelEmailNoNew();
                        break;
                    case Modules.RegistrationTypeEnum.NameOnly:
                        model = new LoginModelNameNoNew();
                        break;
                    case Modules.RegistrationTypeEnum.NameAndEmail:
                        model = new LoginModelNameNoNew();
                        break;
                }
            }
            if (origModel != null)
                ObjectSupport.CopyData(origModel, model);
            return model;
        }


        [AllowGet]
        public async Task<ActionResult> Login(string name, string pswd, string v, bool closeOnLogin = false, bool __f = false) {

            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            bool isPersistent = config.PersistentLogin;

            LoginModel model = GetLoginModel(config);
            model.UserName = name;
            model.Password = pswd;
            model.VerificationCode = v;
            model.Captcha = new RecaptchaV2Data();
            model.RememberMe = isPersistent;
            model.CloseOnLogin = closeOnLogin;
            model.ShowVerification = !string.IsNullOrWhiteSpace(model.VerificationCode);
            model.ShowCaptcha = config.Captcha && !model.ShowVerification && !Manager.IsLocalHost;

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

            LoginModel realModel = GetLoginModel(config, model);

            if (realModel.ShowCaptcha != (config.Captcha && !realModel.ShowVerification && !Manager.IsLocalHost))
                throw new InternalError("Hidden field tampering detected");

            if (!ModelState.IsValid)
                return PartialView(realModel);

            return await CompleteLoginAsync(realModel, config, useTwoStep: true);
        }

        [AllowPost]
        [ExcludeDemoMode]
        public async Task<ActionResult> LoginDirect(string name, string password, Guid security) {

            Package package = AreaRegistration.CurrentPackage;
            Guid batchKey = WebConfigHelper.GetValue<Guid>(package.AreaName, "BatchKey");
            if (batchKey != security)
                return NotAuthorized();

            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            LoginModel model = GetLoginModel(config);
            model.RememberMe = true;
            model.UserName = name;
            model.Password = password;

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
            LoginModel model = GetLoginModel(config);
            model.RememberMe = true;
            model.UserName = name;
            model.Password = password;

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
            UserDefinition user = null;
            user = await Managers.GetUserManager().FindByNameAsync(model.UserName);
            if (user == null) {
                Logging.AddErrorLog("User login failed: {0} - no such user", model.UserName);
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
                ModelState.AddModelError(nameof(LoginModel.UserName), this.__ResStr("maxAttemps", "The maximum number of login attempts has been exceeded - Your account has been suspended"));
                if (user.UserStatus != UserStatusEnum.Suspended) {
                    user.UserStatus = UserStatusEnum.Suspended;
#if MVC6
                    await Managers.GetUserManager().UpdateAsync(user);
#else
                    Managers.GetUserManager().Update(user);
#endif
                }
                return PartialView(model);
            }

            if (user.PasswordHash != null || !string.IsNullOrWhiteSpace(user.PasswordPlainText) || !string.IsNullOrWhiteSpace(model.Password)) {
                UserDefinition foundUser = user;
                user = null;
#if MVC6
                user = await Managers.GetUserManager().FindByNameAsync(model.UserName);
                if (string.IsNullOrWhiteSpace(model.Password) || !await Managers.GetUserManager().CheckPasswordAsync(user, model.Password))
                    user = null;
#else
                if (!string.IsNullOrWhiteSpace(model.Password))
                    user = await Managers.GetUserManager().FindAsync(model.UserName, model.Password);
#endif
                if (user == null) {
                    foundUser.LoginFailures = foundUser.LoginFailures + 1;
#if MVC6
                    await Managers.GetUserManager().UpdateAsync(foundUser);
#else
                    Managers.GetUserManager().Update(foundUser);
#endif
                }
            }
            if (user == null) {
                Logging.AddErrorLog("User login failed: {0}, {1}, {2}", model.UserName, model.Password, model.VerificationCode);
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
#if MVC6
                    await Managers.GetUserManager().UpdateAsync(user);
#else
                    Managers.GetUserManager().Update(user);
#endif

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
#if MVC6
                    await Managers.GetUserManager().UpdateAsync(user);
#else
                    Managers.GetUserManager().Update(user);
#endif
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
                if (useTwoStep) {
                    ActionResult actionResult = await TwoStepAuthetication(user);
                    if (actionResult != null) {
                        Manager.SessionSettings.SiteSettings.SetValue<int>(LoginTwoStepController.IDENTITY_TWOSTEP_USERID, user.UserId);// marker that user has entered correct name/password
                        Manager.SessionSettings.SiteSettings.SetValue<string>(LoginTwoStepController.IDENTITY_TWOSTEP_NEXTURL, Manager.ReturnToUrl);// marker that user has entered correct name/password
                        Manager.SessionSettings.SiteSettings.SetValue<bool>(LoginTwoStepController.IDENTITY_TWOSTEP_CLOSEONLOGIN, model.CloseOnLogin);
                        Manager.SessionSettings.SiteSettings.Save();
                        return actionResult;
                    }
                }
                await LoginModuleController.UserLoginAsync(user, model.RememberMe);
                model.Success = true;
                Logging.AddLog("User {0} - logged on", model.UserName);

                if (model.CloseOnLogin) {
                    return FormProcessed(model, OnClose: OnCloseEnum.CloseWindow, OnPopupClose: OnPopupCloseEnum.GotoNewPage, NextPage: Manager.ReturnToUrl, ForceRedirect: true);
                } else {
                    string nextUrl = await Resource.ResourceAccess.GetUserPostLoginUrlAsync(Manager.UserRoles);
                    if (string.IsNullOrWhiteSpace(nextUrl))
                        nextUrl = Manager.CurrentSite.PostLoginUrl;
                    if (!string.IsNullOrWhiteSpace(nextUrl))
                        return FormProcessed(model, OnClose: OnCloseEnum.GotoNewPage, OnPopupClose: OnPopupCloseEnum.GotoNewPage, NextPage: nextUrl, ForceRedirect: true);
                    return FormProcessed(model, ForceRedirect: true);
                }
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
        internal static
#if MVC6
                async
#endif
                Task UserLogoffAsync() {
            Manager.EditMode = false;
#if MVC6
            SignInManager<UserDefinition> _signinManager = (SignInManager<UserDefinition>)YetaWFManager.ServiceProvider.GetService(typeof(SignInManager<UserDefinition>));
            await _signinManager.SignOutAsync();
#else
            IAuthenticationManager authManager = Manager.CurrentRequest.GetOwinContext().Authentication;
            authManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.ExternalBearer, DefaultAuthenticationTypes.TwoFactorCookie, DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
#endif
            Manager.SessionSettings.ClearAll();
#if MVC6
#else
            return Task.CompletedTask;
#endif
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
#if MVC6
            SignInManager<UserDefinition> _signinManager = (SignInManager<UserDefinition>)YetaWFManager.ServiceProvider.GetService(typeof(SignInManager<UserDefinition>));
            await _signinManager.SignInAsync(user, isPersistent);
#else
            IAuthenticationManager authManager = Manager.CurrentRequest.GetOwinContext().Authentication;
            var identity = await Managers.GetUserManager().CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            authManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
#endif

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
#if MVC6
            await Managers.GetUserManager().UpdateAsync(user);
#else
            Managers.GetUserManager().Update(user);
#endif
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