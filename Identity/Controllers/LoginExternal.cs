/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.Owin.Security;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using YetaWF.Core.Localize;
using YetaWF.Core.Log;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Models;

namespace YetaWF.Modules.Identity.Controllers {

    // Standard MVC Controller
    // Standard MVC Controller
    // Standard MVC Controller
    // In order to log in through an external provider, we are using a STANDARD MVC controller

    public class LoginExternalController : Controller {

        public LoginExternalController() { }

        protected static YetaWFManager Manager { get { return YetaWFManager.Manager; } }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider) {
            // Request a redirect to the external login provider
            if (provider == null)
                throw new InternalError("No provider");
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "LoginExternal", new { }));
        }

        [HttpGet]
        public async Task<ActionResult> ExternalLoginCallback() {
            IAuthenticationManager authManager = HttpContext.GetOwinContext().Authentication;
            var loginInfo = await authManager.GetExternalLoginInfoAsync();
            if (loginInfo == null) {
                Logging.AddErrorLog("AuthenticationManager.GetExternalLoginInfoAsync() returned null");
                return Redirect(Helper.GetSafeReturnUrl(Manager.CurrentSite.LoginUrl));
            }

            // get our registration defaults
            LoginConfigData config = LoginConfigDataProvider.GetConfig();

            // Sign in the user with this external login provider if the user already has a login
            UserDefinition user = await Managers.GetUserManager().FindAsync(loginInfo.Login);
            if (user == null) {
                // If the user does not have an account, then prompt the user to create an account
                // we will go to a page where the user can set up a local account
                return Redirect(Helper.GetSafeReturnUrl(Manager.CurrentSite.ExternalAccountSetupUrl));
            }

            // determine what to do based on account status
            if (user.UserStatus == UserStatusEnum.Approved) {
                await LoginModuleController.UserLoginAsync(user);
                Logging.AddLog("User {0} - logged on", user.UserName);
                return Redirect(Manager.ReturnToUrl);
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
            UriBuilder uri = new UriBuilder(url);
            NameValueCollection qs = System.Web.HttpUtility.ParseQueryString(uri.Query);
            qs["Message"] = popupText;
            qs["Title"] = popupTitle;
            uri.Query = qs.ToString();
            return uri.ToString();
        }

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
        internal static class Helper {
            public static string GetSafeReturnUrl(string url) {
                if (string.IsNullOrWhiteSpace(url))
                    url = YetaWFManager.Manager.CurrentSite.HomePageUrl;
                return url;
            }
        }
    }
}