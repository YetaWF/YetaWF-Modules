/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Log;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Core.Support.TwoStepAuthorization;
using YetaWF.DataProvider;
using YetaWF.Modules.Identity.Addons;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Endpoints;
using YetaWF.Modules.Identity.Models;
using YetaWF.Modules.Identity.Support;

namespace YetaWF.Modules.Identity.Modules;

public class LoginModuleDataProvider : ModuleDefinitionDataProvider<Guid, LoginModule>, IInstallableModel { }

[ModuleGuid("{47C80477-1F25-4f9d-902C-E3D8B3A62686}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
[ModuleCategory("Login & Registration")]
public class LoginModule : ModuleDefinition {

    public LoginModule() {
        Title = this.__ResStr("title", "Login");
        Name = this.__ResStr("title", "Login");
        Description = this.__ResStr("modSummary", "Allows a user to enter account information to log into the site. The Login Module can be accessed using User > Login (standard YetaWF site).");
    }

    public override IModuleDefinitionIO GetDataProvider() { return new LoginModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

    public async Task<ModuleAction> GetAction_LoginAsync(string url = null, bool Force = false) {
        if (!Force && Manager.HaveUser) return null; // the login action should not be shown if a user is logged on
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = await CustomIconAsync("Login.png"),
            LinkText = this.__ResStr("loginLink", "Login using your existing account"),
            MenuText = this.__ResStr("loginText", "Login"),
            Tooltip = this.__ResStr("loginTooltip", "If you have an account on this site, click to log in"),
            Legend = this.__ResStr("loginLegend", "Logs into the site"),
            Style = Manager.IsInPopup ? ModuleAction.ActionStyleEnum.ForcePopup : ModuleAction.ActionStyleEnum.Normal,
            Location = ModuleAction.ActionLocationEnum.NoAuto | ModuleAction.ActionLocationEnum.InPopup | ModuleAction.ActionLocationEnum.ModuleLinks,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
        };
    }

    public async Task<ModuleAction> GetAction_LoginAsAsync(int userId, string userName) {
        if (!await Resource.ResourceAccess.IsResourceAuthorizedAsync(Info.Resource_AllowUserLogon))
            return null;
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(LoginDirectEndpoints), nameof(LoginDirectEndpoints.LoginAs)),
            QueryArgs = new { UserId = userId },
            Image = await CustomIconAsync("LoginAs.png"),
            LinkText = this.__ResStr("loginAsLink", "Login a user {0}", userName),
            MenuText = this.__ResStr("loginAsText", "not used"),
            Tooltip = this.__ResStr("loginAsTooltip", "Log in as user {0}", userName),
            Legend = this.__ResStr("loginAsLegend", "Logs in as another user"),
            Style = ModuleAction.ActionStyleEnum.OuterWindow,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            Category = ModuleAction.ActionCategoryEnum.Significant,
            Mode = ModuleAction.ActionModeEnum.Any,
            DontFollow = true,
        };
    }
    public async Task<ModuleAction> GetAction_ResendVerificationEmailAsync(string userName) {
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(LoginModuleEndpoints), nameof(LoginModuleEndpoints.ResendVerificationEmail)),
            QueryArgs = new { UserName = userName },
            Image = await CustomIconAsync("VerificationEmail.png"),
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("sendVerificationLink", "Resend Verification Email"),
            MenuText = this.__ResStr("sendVerificationMenu", "Resend Verification Email"),
            Tooltip = this.__ResStr("sendVerificationTT", "Sends a verification email to you"),
            Legend = this.__ResStr("sendVerificationLegend", "Sends a verification email to the user"),
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto
        };
    }

    [Legend("Account Information")]
    public class LoginModel {

        public LoginModel() {
            Captcha = new RecaptchaV2Data();
            ExternalProviders = new List<FormButton>();
            Images = new List<string>();
            Actions = new List<ModuleAction>();
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

        [JsonPropertyName("g-recaptcha-response")]
        public string g_recaptcha_response { get; set; }

        [Caption(""), Description("")]
        [UIHint("ModuleActions"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.NormalLinks), ReadOnly]
        [SuppressEmpty]
        public List<ModuleAction> Actions { get; set; }

        [UIHint("Hidden"), ReadOnly]
        public bool ShowVerification { get; set; }
        [UIHint("Hidden"), ReadOnly]
        public bool ShowCaptcha { get; set; }

        public List<string> Images { get; set; }
        public List<FormButton> ExternalProviders { get; set; }

        public bool Success { get; set; }

        [UIHint("Hidden"), ReadOnly]
        public RegistrationTypeEnum RegistrationType { get; set; }
        [UIHint("Hidden"), ReadOnly]
        public bool AllowNewUser { get; set; }
        [UIHint("Hidden"), ReadOnly]
        public string ReturnUrl { get; set; }

        public async Task UpdateAsync() {
            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            RegisterModule regMod = (RegisterModule)await ModuleDefinition.CreateUniqueModuleAsync(typeof(RegisterModule));
            ForgotPasswordModule pswdMod = (ForgotPasswordModule)await ModuleDefinition.CreateUniqueModuleAsync(typeof(ForgotPasswordModule));
            ModuleAction pswdAction = await pswdMod.GetAction_ForgotPasswordAsync(config.ForgotPasswordUrl);
            Actions.New(pswdAction);
            ModuleAction registerAction = await regMod.GetAction_RegisterAsync(config.RegisterUrl, Force: true);
            Actions.New(registerAction);
        }
    }

    public async Task<ActionInfo> RenderModuleAsync(string name, string pswd, string v, bool __f, string returnUrl) {

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
            ShowVerification = !string.IsNullOrWhiteSpace(v),
            ShowCaptcha = config.Captcha && string.IsNullOrWhiteSpace(v) && !Manager.IsLocalHost,
            ReturnUrl = returnUrl,
        };

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
                Package package = AreaRegistration.CurrentPackage;
                string url = Package.GetAddOnPackageUrl(package.AreaName);
                model.Images.Add(Manager.GetCDNUrl($"{url}Icons/LoginProviders/{provider.InternalName}.png"));
            }
        }
        if (__f)
            Manager.CurrentResponse.StatusCode = 401;

        await model.UpdateAsync();
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(LoginModel model) {

        await model.UpdateAsync();

        LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
        model.AllowNewUser = config.AllowUserRegistration;
        model.RegistrationType = config.RegistrationType;

        if (model.ShowCaptcha != (config.Captcha && !model.ShowVerification && !Manager.IsLocalHost))
            throw new InternalError("Hidden field tampering detected");

        if (!ModelState.IsValid && config.RegistrationType == RegistrationTypeEnum.EmailOnly) {
            Core.Endpoints.Support.ModelState.PropertyState emailState = ModelState.GetProperty(nameof(model.Email));
            if (emailState != null && !emailState.Valid) {
                // we allow the superuser name as login in case we use email registration and the superuser name is not a valid email address
                if (string.Compare(model.Email, SuperuserDefinitionDataProvider.SuperUserName, true) == 0)
                    ModelState.RemoveModelState(nameof(model.Email));
            }
        }
        if (!model.ShowCaptcha) {
            Core.Endpoints.Support.ModelState.PropertyState captchaState = ModelState.GetProperty(nameof(model.Captcha));
            if (captchaState != null)
                ModelState.AddValid(nameof(model.Captcha));
        }

        if (!ModelState.IsValid)
            return await PartialViewAsync(model);

        if (config.RegistrationType == RegistrationTypeEnum.EmailOnly)
            model.UserName = model.Email;

        return await CompleteLoginAsync(model, config, useTwoStep: true);
    }

    private async Task<IResult> CompleteLoginAsync(LoginModel model, LoginConfigData config, bool useTwoStep) {

        Manager.SessionSettings.SiteSettings.ClearValue(LoginTwoStepEndpoints.IDENTITY_TWOSTEP_USERID);
        Manager.SessionSettings.SiteSettings.ClearValue(LoginTwoStepEndpoints.IDENTITY_TWOSTEP_NEXTURL);
        Manager.SessionSettings.SiteSettings.Save();
        model.Success = false;

        // make sure it's a valid user
        UserDefinition user = await Managers.GetUserManager().FindByNameAsync(model.UserName);
        if (user == null) {
            Logging.AddErrorLog("User login failed: {0} - no such user", model.UserName);
            ModelState.AddModelError(nameof(LoginModel.Email), this.__ResStr("invLogin", "Invalid user name or password"));
            ModelState.AddModelError(nameof(LoginModel.UserName), this.__ResStr("invLogin", "Invalid user name or password"));
            ModelState.AddModelError(nameof(LoginModel.Password), this.__ResStr("invLogin", "Invalid user name or password"));
            return await PartialViewAsync(model);
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
            return await PartialViewAsync(model);
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
            return await PartialViewAsync(model);
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
                return await FormProcessedAsync(model,
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
            model.ResendVerificationCode = await GetAction_ResendVerificationEmailAsync(user.UserName);
            model.ShowCaptcha = false;
            return await PartialViewAsync(model);
        } else if (user.UserStatus == UserStatusEnum.NeedApproval) {
            Logging.AddErrorLog("User {0} - not yet approved", model.UserName);
            string nextPage = string.IsNullOrWhiteSpace(config.ApprovalPendingUrl) ? Manager.CurrentSite.HomePageUrl : config.ApprovalPendingUrl;
            return await FormProcessedAsync(model,
                this.__ResStr("notApproved2", "Your account has not yet been approved by the site administrator. You will receive an email confirmation as soon as your account is active."),
                NextPage: nextPage);
        } else if (user.UserStatus == UserStatusEnum.Rejected) {
            Logging.AddErrorLog("User {0} - rejected user", model.UserName);
            string nextPage = string.IsNullOrWhiteSpace(config.RejectedUrl) ? Manager.CurrentSite.HomePageUrl : config.RejectedUrl;
            return await FormProcessedAsync(model,
                this.__ResStr("accountRejected", "Your account has been rejected by the site administrator."),
                NextPage: nextPage);
        } else if (user.UserStatus == UserStatusEnum.Suspended) {
            Logging.AddErrorLog("User {0} - suspended user", model.UserName);
            string nextPage = string.IsNullOrWhiteSpace(config.SuspendedUrl) ? Manager.CurrentSite.HomePageUrl : config.SuspendedUrl;
            return await FormProcessedAsync(model,
                this.__ResStr("accountSuspended", "Your account has been suspended."),
                NextPage: nextPage);
        } else if (user.UserStatus == UserStatusEnum.Approved) {

            string nextUrl = model.ReturnUrl;
            if (string.IsNullOrWhiteSpace(nextUrl))
                nextUrl = await Resource.ResourceAccess.GetUserPostLoginUrlAsync((from u in user.RolesList select u.RoleId).ToList());
            if (string.IsNullOrWhiteSpace(nextUrl))
                nextUrl = Manager.CurrentSite.PostLoginUrl;
            if (string.IsNullOrWhiteSpace(nextUrl))
                nextUrl = YetaWFManager.Manager.CurrentSite.HomePageUrl;

            if (useTwoStep) {
                IResult actionResult = await TwoStepAuthetication(user);
                if (actionResult != null) {
                    Manager.SessionSettings.SiteSettings.SetValue<int>(LoginTwoStepEndpoints.IDENTITY_TWOSTEP_USERID, user.UserId);// marker that user has entered correct name/password
                    Manager.SessionSettings.SiteSettings.SetValue<string>(LoginTwoStepEndpoints.IDENTITY_TWOSTEP_NEXTURL, nextUrl);
                    Manager.SessionSettings.SiteSettings.Save();
                    return actionResult;
                }
            }
            await LoginModule.UserLoginAsync(user, model.RememberMe);
            model.Success = true;
            Logging.AddLog("User {0} - logged on", model.UserName);

            return await FormProcessedAsync(model, OnClose: OnCloseEnum.GotoNewPage, OnPopupClose: OnPopupCloseEnum.GotoNewPage, NextPage: nextUrl, ForceReload: true);

        } else
            throw new InternalError("badUserStatus", "Unexpected account status {0}", user.UserStatus);
    }

    /// <summary>
    /// Returns a result to start the two-step authentication process (if any).
    /// </summary>
    private async Task<IResult> TwoStepAuthetication(UserDefinition user) {
        TwoStepAuth twoStep = new TwoStepAuth();
        List<string> enabledTwoStepAuthentications = (from e in user.EnabledTwoStepAuthentications select e.Name).ToList();
        ModuleAction action = await twoStep.GetLoginActionAsync(enabledTwoStepAuthentications, user.UserId, user.UserName, user.Email);
        if (action == null)
            return null;
        return Redirect(action.GetCompleteUrl());
    }

    // User logoff
    internal static async Task UserLogoffAsync() {
        Manager.EditMode = false;

        SignInManager<UserDefinition> _signinManager = (SignInManager<UserDefinition>)Manager.ServiceProvider.GetService(typeof(SignInManager<UserDefinition>));
        await _signinManager.SignOutAsync();

        Manager.SessionSettings.ClearAll();
    }

    // User login
    public static async Task UserLoginAsync(UserDefinition user, bool? rememberme = null) {

        await LoginModule.UserLogoffAsync();

        bool isPersistent;
        if (rememberme == null) {
            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            isPersistent = config.PersistentLogin;
        } else {
            isPersistent = (bool)rememberme;
        }

        SignInManager<UserDefinition> _signinManager = (SignInManager<UserDefinition>)Manager.ServiceProvider.GetService(typeof(SignInManager<UserDefinition>));
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
}
