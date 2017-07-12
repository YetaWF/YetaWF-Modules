/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

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
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Core.Support.TwoStepAuthorization;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Models;
using YetaWF.Modules.Identity.Support;
using YetaWF.Core.Packages;
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

        [Trim]
        [Header("-<p>You are entering an area of our web site, for which you need to register using your name/email address and password.</p>" +
                "<p>If you are a new user, please register a new account now. If you already established an account earlier, please enter this information here and click \"Log In\".</p>")]
        [Legend("Account Information")]
        public class LoginModel {

            public LoginModel() {
                Captcha = new RecaptchaV2Data();
                ExternalProviders = new List<FormButton>();
                Images = new List<string>();
            }

            [Caption("Name"), Description("Enter your user name - this is the name you used when you registered on this site")]
            [UIHint("Text40"), StringLength(Globals.MaxUser), UserNameValidation, Required, Trim]
            public string UserName { get; set; }

            [Caption("Password"), Description("Enter your password")]
            [UIHint("Password20"), StringLength(Globals.MaxPswd), Trim]
            public string Password { get; set; }

            [UIHint("Boolean")]
            [Caption("Remember Me"), Description("Select this option so your information is saved and you don't need to login again when you return to this site (when using the same browser)")]
            public bool RememberMe { get; set; }

            [Caption("Verification Code"), Description("Please enter the verification code you received via email to validate your account")]
            [UIHint("Text40"), StringLength(UserDefinition.MaxVerificationCode), Trim, SuppressIfEqual("ShowVerification", false)]
            public string VerificationCode{ get; set; }

            [Caption("Captcha"), Description("Please verify that you're a human and not a spam bot")]
            [UIHint("RecaptchaV2"), RecaptchaV2("Please verify that you're a human and not a spam bot"), SuppressIfEqual("ShowCaptcha", false)]
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

        [AllowGet]
        public ActionResult Login(string name, string pswd, string v, bool closeOnLogin = false, bool __f = false) {

            LoginConfigData config = LoginConfigDataProvider.GetConfig();
            bool isPersistent = config.PersistentLogin;

            LoginModel model = new LoginModel {
                UserName = name,
                Password = pswd,
                VerificationCode = v,
                Captcha = new RecaptchaV2Data(),
                RememberMe = isPersistent,
                CloseOnLogin = closeOnLogin,
            };
            model.ShowVerification = !string.IsNullOrWhiteSpace(model.VerificationCode);
            model.ShowCaptcha = config.Captcha && !model.ShowVerification;

            using (LoginConfigDataProvider logConfigDP = new LoginConfigDataProvider()) {
                List<LoginConfigDataProvider.LoginProviderDescription> loginProviders = logConfigDP.GetActiveExternalLoginProviders();
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
                    string url = VersionManager.GetAddOnPackageUrl(package.Domain, package.Product);
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

            LoginConfigData config = LoginConfigDataProvider.GetConfig();

            if (model.ShowCaptcha != (config.Captcha && !model.ShowVerification))
                throw new InternalError("Hidden field tampering detected");

            if (!ModelState.IsValid)
                return PartialView(model);

            return await CompleteLogin(model, config);
        }

        [AllowPost]
        [ExcludeDemoMode]
        public async Task<ActionResult> LoginDirect(string name, string password, Guid security) {

            Package package = AreaRegistration.CurrentPackage;
            Guid batchKey = WebConfigHelper.GetValue<Guid>(package.AreaName, "BatchKey");
            if (batchKey != security)
                return NotAuthorized();

            LoginModel model = new LoginModel {
                RememberMe = true,
                UserName = name,
                Password = password,
            };
            ActionResult result = await CompleteLogin(model, LoginConfigDataProvider.GetConfig());
            if (!model.Success)
                Manager.CurrentResponse.StatusCode = 401;
            return result;
        }

        private async Task<ActionResult> CompleteLogin(LoginModel model, LoginConfigData config) {

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
                ModelState.AddModelError("", this.__ResStr("invLogin", "Invalid user name or password"));
                return PartialView(model);
            }
            using (UserLoginInfoDataProvider logInfoDP = new UserLoginInfoDataProvider()) {
                if (logInfoDP.IsExternalUser(user.UserId))
                    throw new Error(this.__ResStr("extUser", "This account can only be accessed using an external login provider"));
            }

            TwoStepAuth twoStep = new TwoStepAuth();// clear any two-step info we may have
            twoStep.ClearTwoStepAuthetication(user.UserId);

            if (config.MaxLoginFailures != 0 && user.LoginFailures >= config.MaxLoginFailures) {
                ModelState.AddModelError("", this.__ResStr("maxAttemps", "The maximum number of login attempts has been exceeded - Your account has been suspended"));
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
                ModelState.AddModelError("", this.__ResStr("invLogin", "Invalid user name or password"));
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
                    emails.SendApprovalNeeded(user);

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
                    ModelState.AddModelError("VerificationCode", this.__ResStr("invVerification", "The verification code is invalid. Please make sure to copy/paste it from the email to avoid any typos."));
                    user.LoginFailures = user.LoginFailures + 1;
#if MVC6
                    await Managers.GetUserManager().UpdateAsync(user);
#else
                    Managers.GetUserManager().Update(user);
#endif
                } else
                    ModelState.AddModelError("VerificationCode", this.__ResStr("notValidated", "Your account has not yet been validated. You will receive an email with validation information. Please copy and enter the verification code here."));
                model.ShowVerification = true;
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
                ActionResult actionResult = TwoStepAuthetication(user);
                if (actionResult != null) {
                    Manager.SessionSettings.SiteSettings.SetValue<int>(LoginTwoStepController.IDENTITY_TWOSTEP_USERID, user.UserId);// marker that user has entered correct name/password
                    Manager.SessionSettings.SiteSettings.SetValue<string>(LoginTwoStepController.IDENTITY_TWOSTEP_NEXTURL, Manager.ReturnToUrl);// marker that user has entered correct name/password
                    Manager.SessionSettings.SiteSettings.SetValue<bool>(LoginTwoStepController.IDENTITY_TWOSTEP_CLOSEONLOGIN, model.CloseOnLogin);
                    Manager.SessionSettings.SiteSettings.Save();
                    return actionResult;
                }
                await LoginModuleController.UserLoginAsync(user, model.RememberMe);
                model.Success = true;
                Logging.AddLog("User {0} - logged on", model.UserName);
                if (model.CloseOnLogin)
                    return FormProcessed(model, OnClose: OnCloseEnum.CloseWindow, OnPopupClose: OnPopupCloseEnum.GotoNewPage, NextPage: Manager.ReturnToUrl, ForceRedirect: true);
                else
                    return FormProcessed(model, ForceRedirect: true);
            } else
                throw new InternalError("badUserStatus", "Unexpected account status {0}", user.UserStatus);
        }

        /// <summary>
        /// Returns an action result to start the two-step authentication process (if any).
        /// </summary>
        private ActionResult TwoStepAuthetication(UserDefinition user) {
            TwoStepAuth twoStep = new TwoStepAuth();
            List<string> enabledTwoStepAuthentications = (from e in user.EnabledTwoStepAuthentications select e.Name).ToList();
            ModuleAction action = twoStep.GetLoginAction(enabledTwoStepAuthentications, user.UserId, user.UserName, user.Email);
            if (action == null)
                return null;
            return Redirect(action);
        }

        // User logoff
#if MVC6
        internal static async void UserLogoff()
#else
        internal static void UserLogoff()
#endif
        {
            Manager.EditMode = false;
#if MVC6
            SignInManager<UserDefinition> _signinManager = (SignInManager<UserDefinition>)YetaWFManager.ServiceProvider.GetService(typeof(SignInManager<UserDefinition>));
            await _signinManager.SignOutAsync();
#else
            IAuthenticationManager authManager = Manager.CurrentRequest.GetOwinContext().Authentication;
            authManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.ExternalBearer, DefaultAuthenticationTypes.TwoFactorCookie, DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
#endif
            Manager.SessionSettings.ClearAll();
        }

        // User login
        internal static async Task UserLoginAsync(UserDefinition user, bool? rememberme = null) {

            LoginModuleController.UserLogoff();

            bool isPersistent;
            if (rememberme == null) {
                LoginConfigData config = LoginConfigDataProvider.GetConfig();
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
    }
}