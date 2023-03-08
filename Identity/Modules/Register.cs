/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Extensions;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Endpoints;
using YetaWF.Modules.Identity.Models;
using YetaWF.Modules.Identity.Support;

namespace YetaWF.Modules.Identity.Modules;

public class RegisterModuleDataProvider : ModuleDefinitionDataProvider<Guid, RegisterModule>, IInstallableModel { }

public enum RegistrationTypeEnum {
    [EnumDescription("Name Only", "The user's name is used to register on this site")]
    NameOnly = 0,
    [EnumDescription("Email Only", "The user's email address is used to register on this site")]
    EmailOnly = 1,
    [EnumDescription("Name and Email", "The user's name and email address are used to register on this site")]
    NameAndEmail = 2,
}

[ModuleGuid("{60E09334-3ECA-466f-BDF9-9933971B0991}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
[ModuleCategory("Login & Registration")]
public class RegisterModule : ModuleDefinition2 {

    public RegisterModule() {
        Title = this.__ResStr("title", "New User Registration");
        Name = this.__ResStr("title", "New User Registration");
        Description = this.__ResStr("modSummary", "Used by new users to register a new account on the current site. The User Login Settings Module can be used to disable new user registration. The New User Registration can be accessed using User > Register (standard YetaWF site).");
        ShowHelp = true;
        ShowPasswordRules = true;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new RegisterModuleDataProvider(); }
    public override DataProviderImpl GetConfigDataProvider() { return new LoginConfigDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }
    public override List<RoleDefinition> ExtraRoles {
        get {
            return new List<RoleDefinition>() {
                new RoleDefinition("ChangeAccounts",
                    this.__ResStr("roleChangeC", "Change Accounts"), this.__ResStr("roleChange", "The role has permission to change other users' account status"),
                    this.__ResStr("userChangeC", "Change Accounts"), this.__ResStr("userChange", "The user has permission to change other users' account status")),
            };
        }
    }

    [Category("General"), Caption("Post Login Url"), Description("Defines the page to display once the user is logged on - If omitted, the Url to return to is determined automatically")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
    [StringLength(Globals.MaxUrl), Trim]
    public string PostRegisterUrl { get; set; }

    [Category("General"), Caption("Post Login Query String"), Description("Defines whether the original query string is forwarded with the Post Login Url")]
    [UIHint("Boolean"), ProcessIfSupplied(nameof(PostRegisterUrl))]
    [Data_NewValue]
    public bool PostRegisterQueryString { get; set; }

    [Category("General"), Caption("Show Password Rules"), Description("Defines whether the password rules are shown")]
    [UIHint("Boolean")]
    [Data_NewValue]
    public bool ShowPasswordRules { get; set; }

    public async Task<ModuleAction> GetAction_RegisterAsync(string url, bool Force = false, bool CloseOnLogin = false) {
        LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
        if (!config.AllowUserRegistration) return null;
        if (!Force && Manager.HaveUser) return null;
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = CloseOnLogin ? new { CloseOnLogin = CloseOnLogin } : null,
            Image = await CustomIconAsync("Register.png"),
            LinkText = this.__ResStr("regLink", "Register a new user account"),
            MenuText = this.__ResStr("regText", "Register"),
            Tooltip = this.__ResStr("regTooltip", "If you don't have an account on this site, click to register"),
            Legend = this.__ResStr("regLegend", "Allows you to register on this site as a new user"),
            Style = Manager.IsInPopup ? ModuleAction.ActionStyleEnum.ForcePopup : ModuleAction.ActionStyleEnum.Normal,
            Location = ModuleAction.ActionLocationEnum.NoAuto | ModuleAction.ActionLocationEnum.InPopup | ModuleAction.ActionLocationEnum.ModuleLinks,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            SaveReturnUrl = false,
            AddToOriginList = false,
        };
    }
    public async Task<ModuleAction> GetAction_ApproveAsync(string userName) {
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(RegisterModuleEndpoints), nameof(RegisterModuleEndpoints.Approve)),
            Image = await CustomIconAsync("Approve.png"),
            QueryArgs = new { UserName = userName },
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("sendApprovedLink", "Approve User"),
            MenuText = this.__ResStr("sendApprovedMenu", "Approve User"),
            Legend = this.__ResStr("sendApprovedLegend", "Marks the user account as approved"),
            Tooltip = this.__ResStr("sendApprovedTT", "Marks the user account as approved"),
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
        };
    }
    public async Task<ModuleAction> GetAction_RejectAsync(string userName) {
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(RegisterModuleEndpoints), nameof(RegisterModuleEndpoints.Reject)),
            Image = await CustomIconAsync("Reject.png"),
            QueryArgs = new { UserName = userName },
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("sendRejectedLink", "Reject User"),
            MenuText = this.__ResStr("sendRejectedMenu", "Reject User"),
            Legend = this.__ResStr("sendRejectedLegend", "Marks the user account as rejected"),
            Tooltip = this.__ResStr("sendRejectedTT", "Marks the user account as rejected"),
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
        };
    }

    [Trim]
    public class RegisterModel {

        public RegisterModel() {
            Captcha = new RecaptchaV2Data();
            ExternalProviders = new List<FormButton>();
            Images = new List<string>();
            Actions = new List<ModuleAction>();
        }

        [Caption("User Name"), Description("Enter your user name to register")]
        [UIHint("Text40"), StringLength(Globals.MaxUser), UserNameValidation, Trim]
        [SuppressIf(nameof(RegistrationType), RegistrationTypeEnum.EmailOnly)]
        [RequiredIfNot(nameof(RegistrationType), RegistrationTypeEnum.EmailOnly)]
        public string UserName { get; set; }

        [Caption("Email Address"), Description("Enter your email address to register - This is the email address used by this site to communicate with you")]
        [UIHint("Email"), StringLength(Globals.MaxEmail), EmailValidation, Trim]
        [SuppressIf(nameof(RegistrationType), RegistrationTypeEnum.NameOnly)]
        [RequiredIfNot(nameof(RegistrationType), RegistrationTypeEnum.NameOnly)]
        public string Email { get; set; }

        [Caption("Partner"), Description("Enter your partner name")]
        [UIHint("Email"), StringLength(40), Trim]
        public string PartnerName { get; set; }

        [Caption("Password"), Description("Enter your desired password")]
        [UIHint("Password20"), StringLength(Globals.MaxPswd), Required]
        public string Password { get; set; }
        public string Password_AutoComplete { get { return "new-password"; } }

        [Caption(" "), Description("")]
        [UIHint("String"), ReadOnly]
        [SuppressEmpty]
        public string PasswordRules { get; set; }

        [Caption("Password Confirmation"), Description("Enter your password again to confirm")]
        [UIHint("Password20"), Required, StringLength(Globals.MaxPswd), SameAs("Password", "The password confirmation doesn't match the password entered")]
        public string ConfirmPassword { get; set; }
        public string ConfirmPassword_AutoComplete { get { return "new-password"; } }

        [Caption("Captcha"), Description("Please verify that you're a human and not a spam bot")]
        [UIHint("RecaptchaV2"), RecaptchaV2("Please verify that you're a human and not a spam bot"), SuppressIf(nameof(ShowCaptcha), false)]
        public RecaptchaV2Data Captcha { get; set; }

        [Caption(""), Description("")]
        [UIHint("ModuleActions"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.NormalLinks), ReadOnly]
        [SuppressEmpty]
        public List<ModuleAction> Actions { get; set; }

        [UIHint("Hidden"), ReadOnly]
        public RegistrationTypeEnum RegistrationType { get; set; }
        [UIHint("Hidden"), ReadOnly]
        public bool ShowCaptcha { get; set; }

        [UIHint("Hidden"), ReadOnly]
        public bool CloseOnLogin { get; set; }

        [UIHint("Hidden"), ReadOnly]
        public string QueryString { get; set; }

        public List<string> Images { get; set; }
        public List<FormButton> ExternalProviders { get; set; }

        [UIHint("Hidden"), ReadOnly]
        public string ReturnUrl { get; set; } // for external login only

        public async Task UpdateAsync() {
            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            LoginModule loginMod = (LoginModule)await ModuleDefinition.CreateUniqueModuleAsync(typeof(LoginModule));
            bool closeOnLogin;
            Manager.TryGetUrlArg<bool>("CloseOnLogin", out closeOnLogin, false);
            ModuleAction logAction = await loginMod.GetAction_LoginAsync(config.LoginUrl, Force: true, CloseOnLogin: closeOnLogin);
            if (logAction != null)
                logAction.AddToOriginList = false;
            Actions.New(logAction);
        }
    }

    public async Task<ActionInfo> RenderModuleAsync(bool closeOnLogin) {
        LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
        if (!config.AllowUserRegistration)
            throw new Error(this.__ResStr("cantRegister", "This site does not allow new user registration"));

        using (LoginConfigDataProvider logConfigDP = new LoginConfigDataProvider()) {

            RegisterModel model = new RegisterModel {
                RegistrationType = config.RegistrationType,
                ShowCaptcha = config.Captcha && !Manager.IsLocalHost,
                Captcha = new RecaptchaV2Data(),
                CloseOnLogin = closeOnLogin,
                QueryString = Manager.RequestQueryString.ToQueryString(),
                PasswordRules = ShowPasswordRules ? logConfigDP.PasswordRules : null,
            };

            List<LoginConfigDataProvider.LoginProviderDescription> loginProviders = await logConfigDP.GetActiveExternalLoginProvidersAsync();
            if (loginProviders.Count > 0 && Manager.IsInPopup)
                throw new InternalError("When using external login providers, the Register module cannot be used in a popup window");
            foreach (LoginConfigDataProvider.LoginProviderDescription provider in loginProviders) {
                model.ExternalProviders.Add(new FormButton() {
                    ButtonType = ButtonTypeEnum.Submit,
                    Name = "provider",
                    Text = provider.InternalName,
                    Title = this.__ResStr("logAccountTitle", "Log in using your {0} account", provider.DisplayName),
                    CssClass = "t_" + provider.InternalName.ToLower(),
                });
                YetaWF.Core.Packages.Package package = AreaRegistration.CurrentPackage;
                string url = Package.GetAddOnPackageUrl(package.AreaName);
                model.Images.Add(Manager.GetCDNUrl(string.Format("{0}Icons/LoginProviders/{1}.png", url, provider.InternalName)));
            }
            if (Manager.HaveReturnToUrl)
                model.ReturnUrl = Manager.ReturnToUrl;

            await model.UpdateAsync();
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(RegisterModel model) {
        LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
        if (!config.AllowUserRegistration)
            throw new Error(this.__ResStr("cantRegister", "This site does not allow new user registration"));

        if (model.ShowCaptcha != config.Captcha && !Manager.IsLocalHost)
            throw new InternalError("Hidden field tampering detected");
        if (!model.ShowCaptcha) {
            Core.Endpoints.Support.ModelState.PropertyState propertyState = ModelState.GetProperty(nameof(model.Captcha));
            if (propertyState != null)
                ModelState.AddValid(nameof(model.Captcha));
        }

        if (!string.IsNullOrWhiteSpace(model.PartnerName))// bot trap
            return Results.Unauthorized();

        model.RegistrationType = config.RegistrationType;// don't trust what we get from user
        if (ShowPasswordRules) {
            using (LoginConfigDataProvider logConfigDP = new LoginConfigDataProvider()) {
                model.PasswordRules = logConfigDP.PasswordRules;
            }
        }

        await model.UpdateAsync();
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);

        // Create a new user and save all info
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

        user.PasswordPlainText = config.SavePlainTextPassword ? model.Password : null;

        if (config.RegistrationType == RegistrationTypeEnum.NameAndEmail) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                // Email == user.Email
                List<DataProviderFilterInfo> filters = new List<DataProviderFilterInfo> {
                        new DataProviderFilterInfo {
                            Field = nameof(UserDefinition.Email), Operator = "==", Value = user.Email,
                        },
                    };
                UserDefinition userExists = await dataProvider.GetItemAsync(filters);
                if (userExists != null && user.UserName != userExists.Email) {
                    ModelState.AddModelError(nameof(model.Email), this.__ResStr("emailUsed", "An account with email address {0} already exists.", user.Email));
                    return await PartialViewAsync(model);
                }
            }
        }

        // set account status
        if (config.VerifyNewUsers)
            user.UserStatus = UserStatusEnum.NeedValidation;
        else if (config.ApproveNewUsers)
            user.UserStatus = UserStatusEnum.NeedApproval;
        else
            user.UserStatus = UserStatusEnum.Approved;
        user.RegistrationIP = Manager.UserHostAddress;

        // create user
        var result = await Managers.GetUserManager().CreateAsync(user, model.Password);
        if (!result.Succeeded) {
            foreach (var error in result.Errors) {
                ModelState.AddModelError(nameof(model.UserName), error.Description);
                ModelState.AddModelError(nameof(model.Email), error.Description);
                ModelState.AddModelError(nameof(model.Password), error.Description);
            }
            return await PartialViewAsync(model);
        }

        // send appropriate email based on account status
        Emails emails = new Emails();
        if (user.UserStatus == UserStatusEnum.NeedValidation) {
            await emails.SendVerificationAsync(user, config.BccVerification ? Manager.CurrentSite.AdminEmail : null);
            string nextPage = string.IsNullOrWhiteSpace(config.VerificationPendingUrl) ? Manager.CurrentSite.HomePageUrl : config.VerificationPendingUrl;
            return await FormProcessedAsync(model, this.__ResStr("okAwaitRegistration", "An email has just been sent to your email address \"{0}\" to complete the registration. Allow a few minutes for delivery. Once received, use the information in the email to complete the registration.", model.Email),
                this.__ResStr("okRegTitle", "Welcome!"),
                NextPage: nextPage);
        } else if (user.UserStatus == UserStatusEnum.NeedApproval) {
            await emails.SendApprovalNeededAsync(user);
            string nextPage = string.IsNullOrWhiteSpace(config.ApprovalPendingUrl) ? Manager.CurrentSite.HomePageUrl : config.ApprovalPendingUrl;
            return await FormProcessedAsync(model, this.__ResStr("okAwaitApproval", "An email has just been sent to the site administrator for approval of your new account. Once processed and approved, you will receive an email confirming your approval."),
                this.__ResStr("okRegTitle", "Welcome!"),
                NextPage: nextPage);
        } else if (user.UserStatus == UserStatusEnum.Approved) {

            if (config.NotifyAdminNewUsers)
                await emails.SendNewUserCreatedAsync(user);
            await LoginModule.UserLoginAsync(user, config.PersistentLogin);

            string nextUrl = null;
            if (Manager.HaveReturnToUrl)
                nextUrl = Manager.ReturnToUrl;
            if (string.IsNullOrWhiteSpace(nextUrl) && !string.IsNullOrWhiteSpace(PostRegisterUrl)) {
                nextUrl = PostRegisterUrl;
                if (PostRegisterQueryString)
                    nextUrl += model.QueryString.AddQSSeparator() + model.QueryString;
            }
            if (string.IsNullOrWhiteSpace(nextUrl))
                nextUrl = await Resource.ResourceAccess.GetUserPostLoginUrlAsync((from u in user.RolesList select u.RoleId).ToList());
            if (string.IsNullOrWhiteSpace(nextUrl))
                nextUrl = Manager.CurrentSite.PostLoginUrl;

            if (!string.IsNullOrWhiteSpace(nextUrl)) {
                return await FormProcessedAsync(model, this.__ResStr("okRegText", "Your new account has been successfully registered."), this.__ResStr("okRegTitle", "Welcome!"),
                    OnClose: OnCloseEnum.GotoNewPage, OnPopupClose: OnPopupCloseEnum.GotoNewPage, NextPage: nextUrl, ForceRedirect: true);
            }
            return await FormProcessedAsync(model, this.__ResStr("okRegText", "Your new account has been successfully registered."), this.__ResStr("okRegTitle", "Welcome!"), ForceRedirect: true);

        } else
            throw new InternalError("badUserStatus", "Unexpected account status {0}", user.UserStatus);
    }
}
