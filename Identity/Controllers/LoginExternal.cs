/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Log;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Core.Support.UrlHistory;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Models;

namespace YetaWF.Modules.Identity.Controllers {

    // Standard MVC Controller
    // Standard MVC Controller
    // Standard MVC Controller
    // In order to log in through an external provider, we are using a STANDARD MVC controller

    public class LoginExternalController : YetaWFController {

        public LoginExternalController() { }

        [AllowPost]
        [ConditionalAntiForgeryToken] // An error due to antiforgery doesn't show a nice error page, whatevz
        public ActionResult ExternalLogin_Partial(string provider, string returnUrl) {
            // we have to handle error messages explicitly as middleware doesn't respond correctly to post/submit on POC when submitted using non-yetawf mechanisms
            try {
                if (YetaWFManager.IsDemo || Manager.IsDemoUser)
                    throw new Error("This action is not available in Demo mode.");

                // Request a redirect to the external login provider
                if (provider == null)
                    throw new InternalError("No external login provider found");

                SignInManager<UserDefinition> _signinManager = (SignInManager<UserDefinition>)YetaWFManager.ServiceProvider.GetService(typeof(SignInManager<UserDefinition>));
                var redirectUrl = Manager.CurrentSite.MakeFullUrl(Utility.UrlFor(typeof(LoginExternalController), nameof(ExternalLoginCallback), new { ReturnUrl = returnUrl }));
                var properties = _signinManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
                return Challenge(properties, provider);
            } catch (Exception exc) {
                return Redirect(MessageUrl(ErrorHandling.FormatExceptionMessage(exc)));
            }
        }

        [AllowGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null) {

            if (remoteError != null)
                throw new Error(this.__ResStr("extErr", "The external login provider reported this error: {0}", remoteError));

            SignInManager<UserDefinition> _signinManager = (SignInManager<UserDefinition>)YetaWFManager.ServiceProvider.GetService(typeof(SignInManager<UserDefinition>));
            ExternalLoginInfo loginInfo = await _signinManager.GetExternalLoginInfoAsync();
            if (loginInfo == null) {
                Logging.AddErrorLog("AuthenticationManager.GetExternalLoginInfoAsync() returned null");
                return Redirect(Helper.GetSafeReturnUrl(Manager.CurrentSite.LoginUrl));
            }
            using (LoginConfigDataProvider logConfigDP = new LoginConfigDataProvider()) {
                List<LoginConfigDataProvider.LoginProviderDescription> loginProviders = await logConfigDP.GetActiveExternalLoginProvidersAsync();
                if ((from l in loginProviders where l.InternalName == loginInfo.LoginProvider select l).FirstOrDefault() == null) {
                    Logging.AddErrorLog("Callback from external login provider {0} which is not active", loginInfo.LoginProvider);
                    return Redirect(Helper.GetSafeReturnUrl(Manager.CurrentSite.LoginUrl));
                }
            }

            // get our registration defaults
            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();

            // Sign in the user with this external login provider if the user already has a login
            UserDefinition user = await Managers.GetUserManager().FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);
            if (user == null) {
                // If the user does not have an account, then prompt the user to create an account
                // we will go to a page where the user can set up a local account
                Manager.OriginList = new List<Origin>();
                if (!string.IsNullOrWhiteSpace(returnUrl))
                    Manager.OriginList.Add(new Origin() { Url = returnUrl });// where to go after setup
                Manager.OriginList.Add(new Origin() { Url = Helper.GetSafeReturnUrl(Manager.CurrentSite.ExternalAccountSetupUrl) }); // setup
                return Redirect(Manager.ReturnToUrl);
            }

            if (string.IsNullOrWhiteSpace(returnUrl) && Manager.HaveReturnToUrl)
                returnUrl = Manager.ReturnToUrl;
            if (string.IsNullOrWhiteSpace(returnUrl))
                returnUrl = await Resource.ResourceAccess.GetUserPostLoginUrlAsync((from u in user.RolesList select u.RoleId).ToList());
            if (string.IsNullOrWhiteSpace(returnUrl))
                returnUrl = Manager.CurrentSite.PostLoginUrl;
            returnUrl = Helper.GetSafeReturnUrl(returnUrl);

            // determine what to do based on account status
            if (user.UserStatus == UserStatusEnum.Approved) {
                await LoginModuleController.UserLoginAsync(user);
                Logging.AddLog("User {0} - logged on", user.UserName);
                returnUrl = QueryHelper.AddRando(returnUrl); // to defeat client-side caching
                return Redirect(returnUrl);
            } else if (user.UserStatus == UserStatusEnum.Rejected) {
                await LoginModuleController.UserLogoffAsync();
                Logging.AddErrorLog("User {0} - rejected user", user.UserName);
                if (string.IsNullOrWhiteSpace(config.RejectedUrl))
                    return Redirect(MessageUrl("Your account has been rejected by the site administrator."));
                return Redirect(Helper.GetSafeReturnUrl(config.RejectedUrl));
            } else if (user.UserStatus == UserStatusEnum.Suspended) {
                await LoginModuleController.UserLogoffAsync();
                Logging.AddErrorLog("User {0} - suspended user", user.UserName);
                if (string.IsNullOrWhiteSpace(config.SuspendedUrl))
                    return Redirect(MessageUrl("Your account has been suspended by the site administrator."));
                return Redirect(Helper.GetSafeReturnUrl(config.SuspendedUrl));
            } else if (user.UserStatus == UserStatusEnum.NeedValidation) {
                await LoginModuleController.UserLogoffAsync();
                Logging.AddErrorLog("User {0} - not yet validated", user.UserName);
                if (string.IsNullOrWhiteSpace(config.VerificationPendingUrl))
                    return Redirect(MessageUrl(this.__ResStr("notValidated", "Your account has not yet been verified. You will receive an email with verification information. Once received, please use the information in the email to complete the registration.")));
                return Redirect(Helper.GetSafeReturnUrl(config.VerificationPendingUrl));
            } else if (user.UserStatus == UserStatusEnum.NeedApproval) {
                await LoginModuleController.UserLogoffAsync();
                Logging.AddErrorLog("User {0} - not yet approved", user.UserName);
                if (string.IsNullOrWhiteSpace(config.ApprovalPendingUrl))
                    return Redirect(MessageUrl(this.__ResStr("notApproved", "Your account has not yet been approved by the site administrator. You will receive an email confirmation as soon as your account is active.")));
                return Redirect(Helper.GetSafeReturnUrl(config.ApprovalPendingUrl));
            } else {
                await LoginModuleController.UserLogoffAsync();
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
    }
}
