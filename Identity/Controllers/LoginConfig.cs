/* Copyright �2020 Softel vdm, Inc.. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Modules;
using System;
using YetaWF.Core.DataProvider.Attributes;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class LoginConfigModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.LoginConfigModule> {

        public LoginConfigModuleController() { }

        [Trim]
        public class Model {

            [Category("Accounts"), Caption("Allow New Users"), Description("Allow registration of new users")]
            [UIHint("Boolean")]
            public bool AllowUserRegistration { get; set; }

            [Category("Accounts"), Caption("Registration Type"), Description("How a user is registered on this site")]
            [UIHint("Enum")]
            public RegistrationTypeEnum RegistrationType { get; set; }

            [Category("Accounts"), Caption("Save Password"), Description("Defines whether the user's password is saved so it can be recovered (and emailed to the user if necessary) - If passwords are not saved password, a reset email is sent to the user instead which requires a provided key to be entered to reset the password - Not used for external login providers")]
            [UIHint("Boolean")]
            public bool SavePlainTextPassword { get; set; }

            [Category("Accounts"), Caption("Password Reset"), Description("Defines how long a password reset email is valid - Password reset emails are only sent if passwords are not saved by this site (Save Password) - Not used for external login providers")]
            [UIHint("TimeSpanDHM")]
            [ProcessIf(nameof(SavePlainTextPassword), false, Disable = true)]
            public TimeSpan ResetTimeSpan { get; set; }

            [Category("Accounts"), Caption("Verification Required"), Description("Defines whether new users need to be verified before they have access to the site - Verification is performed by sending an email to the new user with a verification code, which must be entered when logging in the first time")]
            [UIHint("Boolean")]
            public bool VerifyNewUsers { get; set; }

            [Category("Accounts"), Caption("Approval Required"), Description("Defines whether new users need to be approved before they have access to the site - Approval is typically given by the site administrator")]
            [UIHint("Boolean")]
            public bool ApproveNewUsers { get; set; }

            [Category("Accounts"), Caption("Persistent Login"), Description("Defines whether a user login is persistent (saved using cookies)")]
            [UIHint("Boolean")]
            public bool PersistentLogin { get; set; }

            [Category("Accounts"), Caption("Max. Login Failures"), Description("Defines the maximum number of login failures before the account is suspended - If 0 is specified login failures will not suspend accounts - A successful login resets the login failure count to 0")]
            [UIHint("IntValue4"), Range(0,9999), Required]
            public int MaxLoginFailures { get; set; }

            [Category("Notifications"), Caption("Notify Admin - New User"), Description("Defines whether the administrator receives email notification when a new user account is created by a new user")]
            [UIHint("Boolean")]
            public bool NotifyAdminNewUsers { get; set; }

            [Category("Notifications"), Caption("Notify Admin - Verification Emails"), Description("Defines whether the site administrator receives a copy of the verification email sent to new users")]
            [UIHint("Boolean")]
            public bool BccVerification { get; set; }

            [Category("Notifications"), Caption("Notify Admin - Forgot Password"), Description("Defines whether the site administrator receives a copy of the forgotten password email sent to users")]
            [UIHint("Boolean")]
            public bool BccForgottenPassword { get; set; }

            [TextAbove("Please select which roles require mandatory two-step authentication. The site Superuser can always access the site even if two-step authentication is required but has not yet been set up. Other users must set up two-step authentication before full access to the site is granted.")]
            [Category("Two-Step Auth"), Caption("Two-Step Authentication"), Description("Defines which roles require mandatory two-step authentication")]
            [UIHint("YetaWF_Identity_RolesSelector"), AdditionalMetadata("ExcludeUser2FA", true)]
            public SerializableList<Role> TwoStepAuth { get; set; }

            [Category("Captcha"), Caption("Use Captcha"), Description("Defines whether the user has to pass a \"human\" test when logging in or registering a new account (Yes), otherwise no test is performed (No)")]
            [UIHint("Boolean")]
            public bool Captcha { get; set; }

            [Category("Captcha"), Caption("Use Captcha (Forgot Pswd)"), Description("For forgotten passwords, defines whether the user has to pass a \"human\" test (Yes), otherwise no test is performed (No)")]
            [UIHint("Boolean")]
            public bool CaptchaForgotPassword { get; set; }


            [Category("Urls")]
            [Caption("Login Url"), Description("The Url where the user can login using an existing account")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
            [StringLength(Globals.MaxUrl), Trim]
            public string LoginUrl { get; set; }

            [Category("Urls")]
            [Caption("Register Url"), Description("The Url where the user can register a new user account")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
            [StringLength(Globals.MaxUrl), Trim]
            public string RegisterUrl { get; set; }

            [Category("Urls")]
            [Caption("Two-Step Authentication Url"), Description("The Url where the user is redirected if two-step authentication hasn't been set up yet")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
            [StringLength(Globals.MaxUrl), Trim]
            public string TwoStepAuthUrl { get; set; }

            [Category("Urls")]
            [Caption("Change Password Url"), Description("The Url where the user is redirected if the password must be changed")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
            [StringLength(Globals.MaxUrl), Trim]
            [Data_NewValue]
            public string ChangePasswordUrl { get; set; }

            [Category("Urls")]
            [Caption("Forgotten Password Url"), Description("The Url where the user can retrieve a forgotten password for an existing account")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
            [StringLength(Globals.MaxUrl), Trim]
            public string ForgotPasswordUrl { get; set; }

            [Category("Urls")]
            [Caption("Verification Pending Url"), Description("The Url where the user is redirected when account verification is pending after registering or while logging on")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
            [StringLength(Globals.MaxUrl), Trim]
            public string VerificationPendingUrl { get; set; }

            [Category("Urls")]
            [Caption("Approval Pending Url"), Description("The Url where the user is redirected when account approval is pending after registering or while logging on")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
            [StringLength(Globals.MaxUrl), Trim]
            public string ApprovalPendingUrl { get; set; }

            [Category("Urls")]
            [Caption("Rejected Url"), Description("The Url where the user is redirected when his/her account has been rejected")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
            [StringLength(Globals.MaxUrl), Trim]
            public string RejectedUrl { get; set; }

            [Category("Urls")]
            [Caption("Suspended Url"), Description("The Url where the user is redirected when his/her account has been suspended")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
            [StringLength(Globals.MaxUrl), Trim]
            public string SuspendedUrl { get; set; }

            [Category("Urls")]
            [Caption("Logged Off Url"), Description("The Url where the user is redirected after logging off")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
            [StringLength(Globals.MaxUrl), Trim]
            public string LoggedOffUrl { get; set; }

            public LoginConfigData ConfigData { get; set; }

            public LoginConfigData GetData(LoginConfigData data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void SetData(LoginConfigData data) {
                ObjectSupport.CopyData(data, this);
            }
            public Model() {
                YetaWFManager.Syncify(async () => { // this is needed during model validation
                    ConfigData = await LoginConfigDataProvider.GetConfigAsync();
                });
                TwoStepAuth = new SerializableList<Role>();
            }
        }

        [AllowGet]
        public async Task<ActionResult> LoginConfig() {
            using (LoginConfigDataProvider dataProvider = new LoginConfigDataProvider()) {
                Model model = new Model { };
                LoginConfigData data = await dataProvider.GetItemAsync();
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "User login configuration was not found."));
                model.SetData(data);
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> LoginConfig_Partial(Model model) {
            using (LoginConfigDataProvider dataProvider = new LoginConfigDataProvider()) {
                LoginConfigData data = await dataProvider.GetItemAsync();// get the original item
                if (!ModelState.IsValid)
                    return PartialView(model);
                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display
                await dataProvider.UpdateConfigAsync(data);
                Manager.Need2FAState = null;// we may have changed two-step auth settings, so re-evaluate
                return FormProcessed(model, this.__ResStr("okSaved", "Configuration settings saved"));
            }
        }
    }
}
