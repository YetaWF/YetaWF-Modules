/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Log;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Models;
using YetaWF.Modules.Identity.Support;

namespace YetaWF.Modules.Identity.Controllers {

    public class LoginModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.LoginModule> {

        public LoginModuleController() { }

        [Trim]
        [Header("-<p>You are entering an area of our web site, for which you need to register using your name/email address and password.</p>" +
                "<p>If you are a new user, please register a new account now. If you already established an account earlier, please enter this information here and click \"Log In\".</p>")]
        [Legend("Account Information")]
        public class LoginModel {

            public LoginModel() {
                Captcha = new RecaptchaV2Data();
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
            public string ReturnUrl { get; set; }
        }

        [HttpGet]
        public ActionResult Login(string name, string pswd, string v) {

            LoginConfigData config = LoginConfigDataProvider.GetConfig();
            bool isPersistent = config.PersistentLogin;

            LoginModel model = new LoginModel {
                UserName = name,
                Password = pswd,
                VerificationCode = v,
                Captcha = new RecaptchaV2Data(),
                RememberMe = isPersistent,
                ReturnUrl = Manager.ReturnToUrl,
            };
            model.ShowVerification = !string.IsNullOrWhiteSpace(model.VerificationCode);
            model.ShowCaptcha = config.Captcha && !model.ShowVerification;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> Login_Partial(LoginModel model) {
            LoginConfigData config = LoginConfigDataProvider.GetConfig();

            if (model.ShowCaptcha != (config.Captcha && !model.ShowVerification))
                throw new InternalError("Hidden field tampering detected");

            if (!ModelState.IsValid)
                return PartialView(model);

            // make sure it's a valid user
            UserDefinition user = null;
            user = await Managers.GetUserManager().FindByNameAsync(model.UserName);
            if (user == null) {
                Logging.AddErrorLog("User login failed: {0} - no such user", model.UserName);
                ModelState.AddModelError("", this.__ResStr("invLogin", "Invalid username or password"));
                return PartialView(model);
            }
            if (user.PasswordHash != null || !string.IsNullOrWhiteSpace(user.PasswordPlainText) || !string.IsNullOrWhiteSpace(model.Password)) {
                user = null;
                if (!string.IsNullOrWhiteSpace(model.Password))
                    user = await Managers.GetUserManager().FindAsync(model.UserName, model.Password);
            }
            if (user == null) {
                Logging.AddErrorLog("User login failed: {0}, {1}, {2}", model.UserName, model.Password, model.VerificationCode);
                ModelState.AddModelError("", this.__ResStr("invLogin", "Invalid username or password"));
                return PartialView(model);
            }

            // if verification code valid, advance user to approved or needs approval
            if (user.UserStatus == UserStatusEnum.NeedValidation && model.VerificationCode == user.VerificationCode) {
                Logging.AddLog("User {0} validated ({1})", model.UserName, model.VerificationCode);

                if (config.ApproveNewUsers) {
                    user.UserStatus = UserStatusEnum.NeedApproval;
                    user.LastActivityDate = DateTime.UtcNow;
                    user.LastActivityIP = Manager.UserHostAddress;
                    Managers.GetUserManager().Update(user);

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
                    this.__ResStr("accountSuspended", "Your account has been suspended by the site administrator."),
                    NextPage: nextPage);
            } else if (user.UserStatus == UserStatusEnum.Approved) {
                await LoginModuleController.UserLoginAsync(user, model.RememberMe);
                Logging.AddLog("User {0} - logged on", model.UserName);
                return FormProcessed(model, NextPage: model.ReturnUrl);
            } else
                throw new InternalError("badUserStatus", "Unexpected account status {0}", user.UserStatus);
        }

        // User logoff
        internal static void UserLogoff() {
            Manager.EditMode = false;
            IAuthenticationManager authManager = Manager.CurrentRequest.GetOwinContext().Authentication;
            authManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.ExternalBearer, DefaultAuthenticationTypes.TwoFactorCookie, DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
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

            IAuthenticationManager authManager = Manager.CurrentRequest.GetOwinContext().Authentication;
            var identity = await Managers.GetUserManager().CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            authManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);

            int superuserRole = Resource.ResourceAccess.GetSuperuserRoleId();
            if (user.RolesList != null && user.RolesList.Contains(new Role { RoleId = superuserRole }, new RoleComparer()))
                Manager.SetSuperUserRole(true);

            user.LastLoginDate = DateTime.UtcNow;
            user.LastLoginIP = Manager.UserHostAddress;
            user.LastActivityDate = DateTime.UtcNow;
            user.LastActivityIP = Manager.UserHostAddress;
            Managers.GetUserManager().Update(user);
        }
    }
}