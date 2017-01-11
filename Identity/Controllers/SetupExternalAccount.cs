/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Log;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Models;
using YetaWF.Modules.Identity.Modules;
using YetaWF.Modules.Identity.Support;

namespace YetaWF.Modules.Identity.Controllers {

    public class SetupExternalAccountModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.SetupExternalAccountModule> {

        public SetupExternalAccountModuleController() { }

        [Trim]
        public class SetupExternalAccountModel {
            [Caption("Name"), Description("Enter your user name - this is the name used to register on this site")]
            [UIHint("Text40"), StringLength(Globals.MaxUser), UserNameValidation, Required, Trim]
            public string UserName { get; set; }

            [Caption("Email Address"), Description("Enter your email address to register - this is the email address used by this site to communicate with you")]
            [UIHint("Email"), SuppressIfEqual("RegistrationType", RegistrationTypeEnum.NameOnly), StringLength(Globals.MaxEmail), EmailValidation, Required, Trim]
            public string Email { get; set; }

            [UIHint("Hidden")]
            public string LoginProvider { get; set; }

            [UIHint("Hidden")]
            public RegistrationTypeEnum RegistrationType { get; set; }
        }

        [HttpGet]
        public async Task<ActionResult> SetupExternalAccount() {

            IAuthenticationManager authManager = HttpContext.GetOwinContext().Authentication;
            var loginInfo = await authManager.GetExternalLoginInfoAsync();
            if (loginInfo == null) {
                Logging.AddErrorLog("AuthenticationManager.GetExternalLoginInfoAsync() returned null");
                throw new Error(this.__ResStr("noExtLogin", "No external login has been processed"));
            }

            AuthenticateResult authResult = await authManager.AuthenticateAsync(DefaultAuthenticationTypes.ExternalCookie);
            ClaimsIdentity externalIdentity = authResult.Identity;
            if (!externalIdentity.IsAuthenticated)
                throw new InternalError("!IsAuthenticated");
            string name = externalIdentity.FindFirstValue(ClaimTypes.Name);
            string email = externalIdentity.FindFirstValue(ClaimTypes.Email);

            // get the registration module for some defaults
            LoginConfigData config = LoginConfigDataProvider.GetConfig();

            SetupExternalAccountModel model = new SetupExternalAccountModel {
                UserName = loginInfo.DefaultUserName,
                LoginProvider = loginInfo.Login.LoginProvider,
                RegistrationType = config.RegistrationType,
                Email = email,
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> SetupExternalAccount_Partial(SetupExternalAccountModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);

            IAuthenticationManager authManager = HttpContext.GetOwinContext().Authentication;
            var loginInfo = await authManager.GetExternalLoginInfoAsync();
            if (loginInfo == null) {
                Logging.AddErrorLog("AuthenticationManager.GetExternalLoginInfoAsync() returned null");
                return Redirect(Manager.CurrentSite.LoginUrl);
            }

            // get the registration module for some defaults
            LoginConfigData config = LoginConfigDataProvider.GetConfig();

            // set new user info
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
            if (config.VerifyNewUsers)
                user.UserStatus = UserStatusEnum.NeedValidation;
            else if (config.ApproveNewUsers)
                user.UserStatus = UserStatusEnum.NeedApproval;
            else
                user.UserStatus = UserStatusEnum.Approved;
            user.RegistrationIP = HttpContext.Request.UserHostAddress;

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

            // create account
            var result = await Managers.GetUserManager().CreateAsync(user);
            if (!result.Succeeded) {
                foreach (var error in result.Errors) {
                    ModelState.AddModelError("", error);
                }
                return PartialView(model);
            }
            // add login provider info
            result = await Managers.GetUserManager().AddLoginAsync(user.Id, loginInfo.Login);
            if (!result.Succeeded) {
                foreach (var error in result.Errors) {
                    ModelState.AddModelError("", error);
                }
                return PartialView(model);
            }

            // send appropriate email based on account status
            Emails emails = new Emails();
            if (user.UserStatus == UserStatusEnum.NeedValidation) {
                emails.SendVerification(user, config.BccVerification ? Manager.CurrentSite.AdminEmail : null);
                string nextPage = string.IsNullOrWhiteSpace(config.VerificationPendingUrl) ? Manager.CurrentSite.HomePageUrl : config.VerificationPendingUrl;
                return FormProcessed(model, this.__ResStr("okAwaitRegistration", "An email has just been sent to your email address \"{0}\" to complete the registration. Allow a few minutes for delivery. Once received, please use the information in the email to complete the registration.", model.Email),
                    this.__ResStr("okRegTitle", "Welcome!"),
                    NextPage: nextPage);
            } else if (user.UserStatus == UserStatusEnum.NeedApproval) {
                emails.SendApprovalNeeded(user);
                string nextPage = string.IsNullOrWhiteSpace(config.ApprovalPendingUrl) ? Manager.CurrentSite.HomePageUrl : config.ApprovalPendingUrl;
                return FormProcessed(model, this.__ResStr("okAwaitApproval", "An email has just been sent to the site administrator for approval of your new account. Once processed and approved, you will receive an email confirming your approval."),
                    this.__ResStr("okRegTitle", "Welcome!"),
                    NextPage: nextPage);
            } else if (user.UserStatus == UserStatusEnum.Approved) {
                if (config.NotifyAdminNewUsers)
                    emails.SendNewUserCreated(user);
                await LoginModuleController.UserLoginAsync(user, config.PersistentLogin);
                return FormProcessed(model, this.__ResStr("okRegText", "Your new account has been successfully registered."), this.__ResStr("okRegTitle", "Welcome!"),
                    NextPage: Manager.ReturnToUrl);
            } else
                throw new InternalError("badUserStatus", "Unexpected account status {0}", user.UserStatus);
        }
    }
}