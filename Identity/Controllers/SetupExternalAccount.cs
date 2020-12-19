/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Log;
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
            [Caption("Name"), Description("Enter your user name - This is the name used to register on this site")]
            [UIHint("Text40"), StringLength(Globals.MaxUser), UserNameValidation, Trim]
            [ProcessIf(nameof(RegistrationType), RegistrationTypeEnum.NameAndEmail, nameof(AllowNewUser), true, nameof(ExistingUser), false)]
            [ProcessIf(nameof(RegistrationType), RegistrationTypeEnum.NameOnly, nameof(AllowNewUser), true, nameof(ExistingUser), false)]
            [RequiredIf(nameof(RegistrationType), RegistrationTypeEnum.NameAndEmail, nameof(AllowNewUser), true, nameof(ExistingUser), false)]
            [RequiredIf(nameof(RegistrationType), RegistrationTypeEnum.NameOnly, nameof(AllowNewUser), true, nameof(ExistingUser), false)]
            public string UserName { get; set; }

            [Caption("Name"), Description("Shows your user name - This is the name used to register on this site")]
            [UIHint("Text40"), StringLength(Globals.MaxUser), UserNameValidation, Trim]
            [ProcessIf(nameof(RegistrationType), RegistrationTypeEnum.NameAndEmail, nameof(ExistingUser), true)]
            [ProcessIf(nameof(RegistrationType), RegistrationTypeEnum.NameOnly, nameof(ExistingUser), true)]
            public string UserNameDisplay { get { return UserName; } }

            [Caption("Email Address"), Description("Enter your email address to register - This is the email address used by this site to communicate with you")]
            [UIHint("Email"), StringLength(Globals.MaxEmail), EmailValidation, Trim]
            [ProcessIf(nameof(RegistrationType), RegistrationTypeEnum.NameAndEmail, nameof(AllowNewUser), true, nameof(ExistingUser), false)]
            [ProcessIf(nameof(RegistrationType), RegistrationTypeEnum.EmailOnly, nameof(AllowNewUser), true, nameof(ExistingUser), false)]
            [RequiredIf(nameof(RegistrationType), RegistrationTypeEnum.NameAndEmail, nameof(AllowNewUser), true, nameof(ExistingUser), false)]
            [RequiredIf(nameof(RegistrationType), RegistrationTypeEnum.EmailOnly, nameof(AllowNewUser), true, nameof(ExistingUser), false)]
            public string Email { get; set; }

            [Caption("Email Address"), Description("Shows your email address - You can change your email address once registration is complete")]
            [UIHint("String"), ReadOnly]
            [ProcessIf(nameof(RegistrationType), RegistrationTypeEnum.NameAndEmail, nameof(ExistingUser), true)]
            [ProcessIf(nameof(RegistrationType), RegistrationTypeEnum.EmailOnly, nameof(ExistingUser), true)]
            public string EmailDisplay { get { return Email; } }

            [Caption("Invitation Code"), Description("Enter the invitation code you received to sign up to this site - If you have not received an invitation code, please contact your site administrator")]
            [UIHint("Text80"), StringLength(80), Trim]
            [ProcessIf(nameof(ExistingUser), true)]
            [RequiredIf(nameof(ExistingUser), true)]
            public string InvitationCode { get; set; }

            [UIHint("Hidden"), ReadOnly]
            public RegistrationTypeEnum RegistrationType { get; set; }
            [UIHint("Hidden"), ReadOnly]
            public bool AllowNewUser { get; set; }
            [UIHint("Hidden"), ReadOnly]
            public bool ExistingUser { get; set; }
            public string LoginProviderDisplay { get; set; }
        }

        [AllowGet]
        public async Task<ActionResult> SetupExternalAccount() {

            // get the registration module for some defaults
            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            bool allowNewUser = config.AllowUserRegistration;

            ExtUserInfo extInfo = await GetUserInfo(config);

            if (!allowNewUser && extInfo.ExistingUser == null)
                throw new Error(this.__ResStr("noInvite", "This site allows new accounts by invitation only - According to our records there is no account on this site for your {0} credentials", extInfo.LoginProviderDisplay));

            SetupExternalAccountModel model = new SetupExternalAccountModel {
                AllowNewUser = allowNewUser,
                ExistingUser = extInfo.ExistingUser != null,
                UserName = extInfo.Name,
                Email = extInfo.Email,
                RegistrationType = config.RegistrationType,
                LoginProviderDisplay = extInfo.LoginProviderDisplay,
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> SetupExternalAccount_Partial(SetupExternalAccountModel model) {

            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            bool allowNewUser = config.AllowUserRegistration;

            ExtUserInfo extInfo = await GetUserInfo(config);

            model.AllowNewUser = allowNewUser;
            model.ExistingUser = extInfo.ExistingUser != null;
            model.LoginProviderDisplay = extInfo.LoginProviderDisplay;
            model.RegistrationType = config.RegistrationType;
            if (extInfo.ExistingUser != null) {
                model.UserName = extInfo.Name;
                model.Email = extInfo.Email;
            }

            if (!ModelState.IsValid)
                return PartialView(model);

            UserDefinition user;
            if (allowNewUser && extInfo.ExistingUser == null) {

                // set new user info

                user = new UserDefinition();
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
                user.RegistrationIP = Manager.UserHostAddress;

                // create new local account
                var createResult = await Managers.GetUserManager().CreateAsync(user);
                if (!createResult.Succeeded)
                    throw new Error(string.Join(" - ", (from e in createResult.Errors select e.Description)));

            } else {

                // invited user

                // verify invitation
                if (extInfo.ExistingUser.VerificationCode != model.InvitationCode) {
                    ModelState.AddModelError(nameof(model.InvitationCode), this.__ResStr("badInvite", "The invitation code is invalid"));
                    return PartialView(model);
                }
                user = extInfo.ExistingUser;
                user.UserStatus = UserStatusEnum.Approved;
            }

            // add login provider info
            IdentityResult result = await Managers.GetUserManager().AddLoginAsync(user, extInfo.LoginInfo);
            if (!result.Succeeded)
                throw new Error(string.Join(" - ", (from e in result.Errors select e.Description)));

            // send appropriate email based on account status
            Emails emails = new Emails();
            if (user.UserStatus == UserStatusEnum.NeedValidation) {
                await emails.SendVerificationAsync(user, config.BccVerification ? Manager.CurrentSite.AdminEmail : null);
                string nextPage = string.IsNullOrWhiteSpace(config.VerificationPendingUrl) ? Manager.CurrentSite.HomePageUrl : config.VerificationPendingUrl;
                return FormProcessed(model, this.__ResStr("okAwaitRegistration", "An email has just been sent to your email address \"{0}\" to complete the registration. Allow a few minutes for delivery. Once received, please use the information in the email to complete the registration.", model.Email),
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
                return FormProcessed(model, this.__ResStr("okRegText", "Your new account has been successfully registered."), this.__ResStr("okRegTitle", "Welcome!"),
                    NextPage: Manager.ReturnToUrl, ForceRedirect: true);
            } else
                throw new InternalError("badUserStatus", "Unexpected account status {0}", user.UserStatus);
        }

        public class ExtUserInfo {
            public string LoginProvider { get; internal set; }
            public string LoginProviderDisplay { get; internal set; }
            public string Email { get; internal set; }
            public string Name { get; internal set; }
            public UserDefinition ExistingUser { get; internal set; }
            public ExternalLoginInfo LoginInfo { get; internal set; }
        }

        private async Task<ExtUserInfo> GetUserInfo(LoginConfigData config) {

            ExtUserInfo info = new ExtUserInfo();

            SignInManager<UserDefinition> _signinManager = (SignInManager<UserDefinition>)YetaWFManager.ServiceProvider.GetService(typeof(SignInManager<UserDefinition>));
            ExternalLoginInfo loginInfo = await _signinManager.GetExternalLoginInfoAsync();
            if (loginInfo == null) {
                Logging.AddErrorLog("AuthenticationManager.GetExternalLoginInfoAsync() returned null");
                throw new Error(this.__ResStr("noExtLogin", "No external login has been processed"));
            }
            info.LoginInfo = loginInfo;
            info.Email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email);
            info.Name = loginInfo.Principal.FindFirstValue(ClaimTypes.Name);
            info.LoginProvider = loginInfo.LoginProvider;
            info.LoginProviderDisplay = loginInfo.ProviderDisplayName;

            // Check whether this is an invited user
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                List<DataProviderFilterInfo> filters = null;
                switch (config.RegistrationType) {
                    default:
                    case RegistrationTypeEnum.NameAndEmail:
                        filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = nameof(UserDefinition.UserName), Operator = "==", Value = info.Name, });
                        filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = nameof(UserDefinition.Email), Operator = "==", Value = info.Email, });
                        break;
                    case RegistrationTypeEnum.EmailOnly:
                        filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = nameof(UserDefinition.Email), Operator = "==", Value = info.Email, });
                        break;
                    case RegistrationTypeEnum.NameOnly:
                        filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = nameof(UserDefinition.UserName), Operator = "==", Value = info.Name, });
                        break;
                }
                info.ExistingUser = await dataProvider.GetItemAsync(filters);
            }
            return info;
        }
    }
}
