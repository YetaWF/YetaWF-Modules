/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Log;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Core.Support.UrlHistory;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Models;
#if MVC6
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
#else
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Web;
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    // Standard MVC Controller
    // Standard MVC Controller
    // Standard MVC Controller
    // In order to log in through an external provider, we are using a STANDARD MVC controller

    public class LoginExternalController : YetaWFController {

        public LoginExternalController() { }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        //[ExcludeDemoMode]
        public ActionResult ExternalLogin(string provider, string returnUrl) {
            AllowJavascriptResult = false;
            // Request a redirect to the external login provider
            if (Manager.IsDemo)
                throw new Error("This action is not available in Demo mode.");
            if (provider == null)
                throw new InternalError("No provider");
#if MVC6
            SignInManager<UserDefinition> _signinManager = (SignInManager<UserDefinition>)YetaWFManager.ServiceProvider.GetService(typeof(SignInManager<UserDefinition>));
            var redirectUrl = Url.Action("ExternalLoginCallback", "LoginExternal", new { ReturnUrl = returnUrl }, "https");
            var properties = _signinManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
#else
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "LoginExternal", new { ReturnUrl = returnUrl }, "https"));
#endif
        }

        [AllowGet]
#if MVC6
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
#else
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl = null)
#endif
        {
            AllowJavascriptResult = false;
            ExternalLoginInfo loginInfo;
#if MVC6
            if (remoteError != null)
                throw new Error(this.__ResStr("extErr", "The external login provider reported this error: {0}", remoteError));

            SignInManager<UserDefinition> _signinManager = (SignInManager<UserDefinition>)YetaWFManager.ServiceProvider.GetService(typeof(SignInManager<UserDefinition>));
            loginInfo = await _signinManager.GetExternalLoginInfoAsync();
#else
            IAuthenticationManager authManager = HttpContext.GetOwinContext().Authentication;
            loginInfo = await authManager.GetExternalLoginInfoAsync();
#endif
            if (loginInfo == null) {
                Logging.AddErrorLog("AuthenticationManager.GetExternalLoginInfoAsync() returned null");
                return Redirect(Helper.GetSafeReturnUrl(Manager.CurrentSite.LoginUrl));
            }
            using (LoginConfigDataProvider logConfigDP = new LoginConfigDataProvider()) {
                List<LoginConfigDataProvider.LoginProviderDescription> loginProviders = logConfigDP.GetActiveExternalLoginProviders();
#if MVC6
                if ((from l in loginProviders where l.InternalName == loginInfo.LoginProvider select l).FirstOrDefault() == null) {
                    Logging.AddErrorLog("Callback from external login provider {0} which is not active", loginInfo.LoginProvider);
                    return Redirect(Helper.GetSafeReturnUrl(Manager.CurrentSite.LoginUrl));
                }
#else
                if ((from l in loginProviders where l.InternalName == loginInfo.Login.LoginProvider select l).FirstOrDefault() == null) {
                    Logging.AddErrorLog("Callback from external login provider {0} which is not active", loginInfo.Login.LoginProvider);
                    return Redirect(Helper.GetSafeReturnUrl(Manager.CurrentSite.LoginUrl));
                }
#endif
            }

            // get our registration defaults
            LoginConfigData config = LoginConfigDataProvider.GetConfig();

            // Sign in the user with this external login provider if the user already has a login
            UserDefinition user;
#if MVC6
            user = await Managers.GetUserManager().FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);
#else
            user = await Managers.GetUserManager().FindAsync(loginInfo.Login);
#endif
            if (user == null) {
                // If the user does not have an account, then prompt the user to create an account
                // we will go to a page where the user can set up a local account
                Manager.OriginList = new List<Origin>();
                Manager.OriginList.Add(new Origin() { Url = returnUrl });// where to go after setup
                Manager.OriginList.Add(new Origin() { Url = Helper.GetSafeReturnUrl(Manager.CurrentSite.ExternalAccountSetupUrl) }); // setup
                return Redirect(Manager.ReturnToUrl);
            }

            // determine what to do based on account status
            if (user.UserStatus == UserStatusEnum.Approved) {
                await LoginModuleController.UserLoginAsync(user);
                Logging.AddLog("User {0} - logged on", user.UserName);
                returnUrl = QueryHelper.AddRando(returnUrl); // to defeat client-side caching
                return Redirect(returnUrl);
            } else if (user.UserStatus == UserStatusEnum.Rejected) {
                LoginModuleController.UserLogoff();
                Logging.AddErrorLog("User {0} - rejected user", user.UserName);
                if (string.IsNullOrWhiteSpace(config.RejectedUrl))
                    return Redirect(MessageUrl("Your account has been rejected by the site administrator."));
                return Redirect(Helper.GetSafeReturnUrl(config.RejectedUrl));
            } else if (user.UserStatus == UserStatusEnum.Suspended) {
                LoginModuleController.UserLogoff();
                Logging.AddErrorLog("User {0} - suspended user", user.UserName);
                if (string.IsNullOrWhiteSpace(config.SuspendedUrl))
                    return Redirect(MessageUrl("Your account has been suspended by the site administrator."));
                return Redirect(Helper.GetSafeReturnUrl(config.SuspendedUrl));
            } else if (user.UserStatus == UserStatusEnum.NeedValidation) {
                LoginModuleController.UserLogoff();
                Logging.AddErrorLog("User {0} - not yet validated", user.UserName);
                if (string.IsNullOrWhiteSpace(config.VerificationPendingUrl))
                    return Redirect(MessageUrl(this.__ResStr("notValidated", "Your account has not yet been validated. You will receive an email with validation information. Once received, please use the information in the email to complete the registration.")));
                return Redirect(Helper.GetSafeReturnUrl(config.VerificationPendingUrl));
            } else if (user.UserStatus == UserStatusEnum.NeedApproval) {
                LoginModuleController.UserLogoff();
                Logging.AddErrorLog("User {0} - not yet approved", user.UserName);
                if (string.IsNullOrWhiteSpace(config.ApprovalPendingUrl))
                    return Redirect(MessageUrl(this.__ResStr("notApproved", "Your account has not yet been approved by the site administrator. You will receive an email confirmation as soon as your account is active.")));
                return Redirect(Helper.GetSafeReturnUrl(config.ApprovalPendingUrl));
            } else {
                LoginModuleController.UserLogoff();
                throw new InternalError("badUserStatus", "Unexpected account status {0}", user.UserStatus);
            }
        }

        /// <summary>
        /// Redirect with a message - THIS ONLY WORKS FOR A GET REQUEST
        /// </summary>
        /// <param name="popupText"></param>
        /// <param name="popupTitle"></param>
        /// <returns></returns>
        private string MessageUrl(string popupText, string popupTitle = null) {
            // we're in a get request, possibly without module, so all we can do is redirect and show the message in the ShowMessage module
            // the ShowMessage module is in the Basics package and we reference it by permanent Guid
            string url = YetaWFManager.Manager.CurrentSite.MakeUrl(ModuleDefinition.GetModulePermanentUrl(new Guid("{b486cdfc-3726-4549-889e-1f833eb49865}")));
            QueryHelper query = QueryHelper.FromUrl(url, out url);
            query["Message"] = popupText;
            query["Title"] = popupTitle;
            return query.ToUrl(url);
        }

        internal static class Helper {
            public static string GetSafeReturnUrl(string url) {
                if (string.IsNullOrWhiteSpace(url))
                    url = YetaWFManager.Manager.CurrentSite.HomePageUrl;
                return url;
            }
        }
#if MVC6
#else
        private class ChallengeResult : HttpUnauthorizedResult {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null) {
            }

            public ChallengeResult(string provider, string redirectUri, string userId) {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context) {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
#endif
    }
}
