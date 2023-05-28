/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Support;

namespace YetaWF.Modules.Identity.Modules;

public class ForgotPasswordModuleDataProvider : ModuleDefinitionDataProvider<Guid, ForgotPasswordModule>, IInstallableModel { }

[ModuleGuid("{3437ee4d-747f-4bf1-aa3c-d0417751b24b}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
[ModuleCategory("Login & Registration")]
public class ForgotPasswordModule : ModuleDefinition {

    public ForgotPasswordModule() {
        Title = this.__ResStr("modTitle", "Forgot Password?");
        Name = this.__ResStr("modName", "Forgot Password?");
        Description = this.__ResStr("modSummary", "Allows a user to retrieve/reset a forgotten password for an existing account. This is used by the Login Module.");
    }

    public override IModuleDefinitionIO GetDataProvider() { return new ForgotPasswordModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

    public async Task<ModuleAction> GetAction_ForgotPasswordAsync(string url, bool CloseOnLogin = false) {
        LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
        if (config.SavePlainTextPassword) {
            return new ModuleAction() {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = CloseOnLogin ? new { CloseOnLogin = CloseOnLogin } : null,
                Image = "#Help",
                LinkText = this.__ResStr("forgotLink", "Forgot your password?"),
                MenuText = this.__ResStr("forgotText", "Forgot your password?"),
                Tooltip = this.__ResStr("forgotTooltip", "If you have an account and forgot your password, click to have an email sent to you with your password"),
                Legend = this.__ResStr("forgotLegend", "Used to send an email to you with your password if you have an account and forgot your password"),
                Style = Manager.IsInPopup ? ModuleAction.ActionStyleEnum.ForcePopup : ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto | ModuleAction.ActionLocationEnum.InPopup | ModuleAction.ActionLocationEnum.ModuleLinks,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,

            };
        } else {
            return new ModuleAction() {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = CloseOnLogin ? new { CloseOnLogin = CloseOnLogin } : null,
                Image = "#Help",
                LinkText = this.__ResStr("resetLink", "Forgot your password?"),
                MenuText = this.__ResStr("resetText", "Forgot your password?"),
                Tooltip = this.__ResStr("resetTooltip", "If you have an account and forgot your password, click to have an email sent to you to reset your password"),
                Legend = this.__ResStr("resetLegend", "Used to send an email to you to reset your password if you have an account and forgot your password"),
                Style = Manager.IsInPopup ? ModuleAction.ActionStyleEnum.ForcePopup : ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto | ModuleAction.ActionLocationEnum.InPopup | ModuleAction.ActionLocationEnum.ModuleLinks,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,

            };
        }
    }

    [Trim]
    public class EditModel {

        [TextAbove("You can request help recovering your password with your email address. Information will be sent to your email address and should arrive in your inbox within a few minutes. Please make sure to update your spam filters to avoid rejecting this email.")]
        [Caption("Email Address"), Description("Enter the email address associated with your account")]
        [UIHint("Email"), StringLength(Globals.MaxEmail), EmailValidation, Trim]
        [ProcessIf(nameof(RegistrationType), RegistrationTypeEnum.EmailOnly)]
        [RequiredIf(nameof(RegistrationType), RegistrationTypeEnum.EmailOnly)]
        public string Email { get; set; }

        [TextAbove("You can request help recovering your password with your account name. Information will be sent to the registered email address and should arrive in your inbox within a few minutes. Please make sure to update your spam filters to avoid rejecting this email.")]
        [Caption("Name"), Description("Enter the name associated with your account")]
        [UIHint("Text40"), StringLength(Globals.MaxUser), UserNameValidation, Trim]
        [ProcessIf(nameof(RegistrationType), RegistrationTypeEnum.NameOnly)]
        [RequiredIf(nameof(RegistrationType), RegistrationTypeEnum.NameOnly)]
        public string UserName { get; set; }

        [TextAbove("You can request help recovering your password with your name or your email address. Information will be sent to your email address and should arrive in your inbox within a few minutes. Please make sure to update your spam filters to avoid rejecting this email.")]
        [Caption("Name or Email Address"), Description("Enter the name or email address associated with your account")]
        [UIHint("Text40"), StringLength(Globals.MaxEmail), Trim]
        [ProcessIf(nameof(RegistrationType), RegistrationTypeEnum.NameAndEmail)]
        [RequiredIf(nameof(RegistrationType), RegistrationTypeEnum.NameAndEmail)]
        public string UserNameOrEmail { get; set; }

        [Caption(""), Description("")]
        [UIHint("ModuleActions"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.NormalLinks), ReadOnly]
        [SuppressEmpty]
        public List<ModuleAction> Actions { get; set; }

        [Caption("Captcha"), Description("Please verify that you're a human and not a spam bot")]
        [UIHint("RecaptchaV2"), RecaptchaV2("Please verify that you're a human and not a spam bot"), SuppressIf("ShowCaptcha", false)]
        public RecaptchaV2Data Captcha { get; set; }
        [UIHint("Hidden")]
        public bool ShowCaptcha { get; set; }

        [JsonPropertyName("g-recaptcha-response")]
        public string? g_recaptcha_response { get; set; }

        [UIHint("Hidden")]
        public RegistrationTypeEnum RegistrationType { get; set; }

        public EditModel() {
            Captcha = new RecaptchaV2Data();
            Actions = new List<ModuleAction>();
        }

        public async Task UpdateAsync() {
            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            RegisterModule regMod = (RegisterModule)await ModuleDefinition.CreateUniqueModuleAsync(typeof(RegisterModule));
            LoginModule loginMod = (LoginModule)await ModuleDefinition.CreateUniqueModuleAsync(typeof(LoginModule));
            bool closeOnLogin;
            Manager.TryGetUrlArg<bool>("CloseOnLogin", out closeOnLogin, false);

            ModuleAction logAction = await loginMod.GetAction_LoginAsync(config.LoginUrl, Force: true, CloseOnLogin: closeOnLogin);
            Actions.New(logAction);
            ModuleAction regAction = await regMod.GetAction_RegisterAsync(config.RegisterUrl, Force: true, CloseOnLogin: closeOnLogin);
            Actions.New(regAction);
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
        EditModel model = new EditModel {
            ShowCaptcha = config.CaptchaForgotPassword,
            RegistrationType = config.RegistrationType,
        };
        await model.UpdateAsync();
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(EditModel model) {

        LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
        if (model.ShowCaptcha != config.CaptchaForgotPassword)
            throw new InternalError("Hidden field tampering detected");

        await model.UpdateAsync();

        model.ShowCaptcha = config.CaptchaForgotPassword;
        model.RegistrationType = config.RegistrationType;
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);

        using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {

            UserDefinition userDef;
            switch (model.RegistrationType) {
                case RegistrationTypeEnum.NameOnly:
                    userDef = await userDP.GetItemAsync(model.UserName);
                    if (userDef == null) {
                        ModelState.AddModelError(nameof(model.UserName), this.__ResStr("badName", "According to our records there is no account associated with this name"));
                        return await PartialViewAsync(model);
                    }
                    break;
                case RegistrationTypeEnum.NameAndEmail:
                    userDef = await userDP.GetItemAsync(model.UserNameOrEmail);
                    if (userDef == null) {
                        userDef = await userDP.GetItemByEmailAsync(model.UserNameOrEmail);
                        if (userDef == null) {
                            ModelState.AddModelError(nameof(model.UserNameOrEmail), this.__ResStr("badNameEmail", "According to our records there is no account associated with this name or email address"));
                            return await PartialViewAsync(model);
                        }
                    }
                    break;
                default:
                case RegistrationTypeEnum.EmailOnly:
                    userDef = await userDP.GetItemByEmailAsync(model.Email);
                    if (userDef == null) {
                        ModelState.AddModelError(nameof(model.Email), this.__ResStr("badEmail", "According to our records there is no account associated with this email address"));
                        return await PartialViewAsync(model);
                    }
                    break;
            }

            using (UserLoginInfoDataProvider logInfoDP = new UserLoginInfoDataProvider()) {
                if (await logInfoDP.IsExternalUserAsync(userDef.UserId)) {
                    ModelState.AddModelError(nameof(model.Email), this.__ResStr("extEmail", "According to our records there is no account associated with this email address"));
                    ModelState.AddModelError(nameof(model.UserName), this.__ResStr("extName", "According to our records there is no account associated with this name"));
                    ModelState.AddModelError(nameof(model.UserNameOrEmail), this.__ResStr("extUser", "According to our records there is no account associated with this name or email address"));
                    return await PartialViewAsync(model);
                }
            }
            switch (userDef.UserStatus) {
                case UserStatusEnum.Approved:
                    Emails emails = new Emails();
                    if (config.SavePlainTextPassword) {
                        await emails.SendForgottenEmailAsync(userDef, config.BccForgottenPassword ? Manager.CurrentSite.AdminEmail : null);
                        string from = emails.GetSendingEmailAddress();
                        return await FormProcessedAsync(model, this.__ResStr("okForgot", "We just sent an email to your email address with your password information - Please allow a few minutes for delivery and make sure your spam filters allow emails from {0}", from), OnClose: OnCloseEnum.Nothing, OnPopupClose: OnPopupCloseEnum.ReloadModule);
                    } else {
                        if (userDef.ResetKey != null && userDef.ResetValidUntil != null && userDef.ResetValidUntil > DateTime.UtcNow) {
                            // preserve existing key in case user resends
                        } else {
                            userDef.ResetKey = Guid.NewGuid();
                            userDef.ResetValidUntil = DateTime.UtcNow.Add(config.ResetTimeSpan);
                            if (await userDP.UpdateItemAsync(userDef) != Core.DataProvider.UpdateStatusEnum.OK)// update reset key info
                                throw new Error(this.__ResStr("resetUpdate", "User information could not be updated"));
                        }
                        await emails.SendPasswordResetEmailAsync(userDef, config.BccForgottenPassword ? Manager.CurrentSite.AdminEmail : null);
                        string from = emails.GetSendingEmailAddress();
                        return await FormProcessedAsync(model, this.__ResStr("okReset", "We just sent an email to your email address to reset your password - Please allow a few minutes for delivery and make sure your spam filters allow emails from {0}", from), OnClose: OnCloseEnum.Nothing, OnPopupClose: OnPopupCloseEnum.ReloadModule);
                    }
                case UserStatusEnum.NeedApproval:
                    throw new Error(this.__ResStr("needApproval", "This account has not yet been approved and is awaiting approval by the site administrator"));
                case UserStatusEnum.NeedValidation:
                    throw new Error(this.__ResStr("needValidation", "This account has not yet been verified - Please check your emails for our verification email"));
                case UserStatusEnum.Rejected:
                    throw new Error(this.__ResStr("rejected", "This account has been rejected and is not accessible"));
                case UserStatusEnum.Suspended:
                    throw new Error(this.__ResStr("suspended", "This account has been suspended and is not accessible"));
                default:
                    throw new Error(this.__ResStr("unknownState", "This account is in an undefined state and is not accessible"));
            }
        }
    }
}
