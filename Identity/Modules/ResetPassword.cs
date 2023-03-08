/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Models;

namespace YetaWF.Modules.Identity.Modules;

public class ResetPasswordModuleDataProvider : ModuleDefinitionDataProvider<Guid, ResetPasswordModule>, IInstallableModel { }

[ModuleGuid("{9FFEAA14-1366-4462-B6F4-1035F1672DBC}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
[ModuleCategory("Login & Registration")]
public class ResetPasswordModule : ModuleDefinition2 {

    public ResetPasswordModule() : base() {
        Title = this.__ResStr("modTitle", "Reset Password");
        Name = this.__ResStr("modName", "Reset Password");
        Description = this.__ResStr("modSummary", "Allows a user to reset the login password.");
        DefaultViewName = StandardViews.Edit;
        ShowPasswordRules = true;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new ResetPasswordModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

    [Category("General"), Caption("Show Password Rules"), Description("Defines whether the password rules are shown")]
    [UIHint("Boolean")]
    [Data_NewValue]
    public bool ShowPasswordRules { get; set; }

    public ModuleAction GetAction_ResetPassword(string url, int userId, string resetKey) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { UserId = userId, ResetKey = resetKey },
            Image = "#Edit",
            LinkText = this.__ResStr("editLink", "Reset Password"),
            MenuText = this.__ResStr("editText", "Reset Password"),
            Tooltip = this.__ResStr("editTooltip", "Reset your account password"),
            Legend = this.__ResStr("editLegend", "Resets your account password"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class EditModel {

        [Caption("Reset Key"), Description("Enter your reset key you received in the email we sent to you in order to reset your password")]
        [UIHint("Text40"), StringLength(80)]
        public string ResetKey { get; set; }

        [Caption("New Password"), Description("Enter your desired new password")]
        [UIHint("Password20"), StringLength(Globals.MaxPswd), Required]
        public string NewPassword { get; set; }
        public string NewPassword_AutoComplete { get { return "new-password"; } }

        [Caption(" "), Description("")]
        [UIHint("String"), ReadOnly]
        [SuppressEmpty]
        public string PasswordRules { get; set; }

        [Caption("New Password Confirmation"), Description("Enter your new password again to confirm")]
        [UIHint("Password20"), StringLength(Globals.MaxPswd), Required, SameAs("NewPassword", "The password confirmation doesn't match the password entered")]
        public string ConfirmPassword { get; set; }
        public string ConfirmPassword_AutoComplete { get { return "new-password"; } }

        [UIHint("Hidden"), ReadOnly]
        public int UserId { get; set; }
    }

    public async Task<ActionInfo> RenderModuleAsync(int userId, string resetKey) {
        UserManager<UserDefinition> userManager = Managers.GetUserManager();
        UserDefinition user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new Error(this.__ResStr("notFound", "User account not found."));
        using (UserLoginInfoDataProvider logInfoDP = new UserLoginInfoDataProvider()) {
            if (await logInfoDP.IsExternalUserAsync(user.UserId))
                return await RenderAsync("ShowMessage", this.__ResStr("extUser", "Your account uses an external login provider - The password (if available) must be set up using the external login provider."), UseAreaViewName: false);
        }
        EditModel model = new EditModel {
            ResetKey = resetKey,
            UserId = userId,
        };
        if (resetKey != null) {
            if (user.ResetKey == null || user.ResetValidUntil == null || user.ResetValidUntil < DateTime.UtcNow) {
                ModelState.AddModelError(nameof(model.ResetKey), this.__ResStr("expired", "The reset key has expired and is no longer valid"));
            }
        }

        using (LoginConfigDataProvider logConfigDP = new LoginConfigDataProvider()) {
            model.PasswordRules = ShowPasswordRules ? logConfigDP.PasswordRules : null;
        }

        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(EditModel model) {

        UserManager<UserDefinition> userManager = Managers.GetUserManager();
        UserDefinition user = await userManager.FindByIdAsync(model.UserId.ToString());
        if (user == null)
            throw new Error(this.__ResStr("notFound", "User account not found."));
        using (UserLoginInfoDataProvider logInfoDP = new UserLoginInfoDataProvider()) {
            if (await logInfoDP.IsExternalUserAsync(model.UserId))
                throw new Error(this.__ResStr("extUser", "Your account uses an external login provider - The password (if available) must be set up using the external login provider."));
        }

        if (ShowPasswordRules) {
            using (LoginConfigDataProvider logConfigDP = new LoginConfigDataProvider()) {
                model.PasswordRules = logConfigDP.PasswordRules;
            }
        }
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);

        Guid? resetKey = null;
        if (user.ResetKey == null || user.ResetValidUntil == null || user.ResetValidUntil < DateTime.UtcNow) {
            ModelState.AddModelError(nameof(model.ResetKey), this.__ResStr("expired", "The reset key has expired and is no longer valid"));
        } else {
            try {
                resetKey = new Guid(model.ResetKey);
            } catch (Exception) {
                ModelState.AddModelError(nameof(model.ResetKey), this.__ResStr("invReset", "The reset key is invalid - Please make sure to copy/paste the key in its entirety"));
            }
        }
        if (resetKey != null && user.ResetKey != (Guid)resetKey) {
            ModelState.AddModelError(nameof(model.ResetKey), this.__ResStr("invReset", "The reset key is invalid - Please make sure to copy/paste the key in its entirety"));
        }
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);

        // change the password

        IPasswordValidator<UserDefinition> passVal = (IPasswordValidator<UserDefinition>)Manager.ServiceProvider.GetService(typeof(IPasswordValidator<UserDefinition>));
        IdentityResult result = await passVal.ValidateAsync(userManager, user, model.NewPassword);
        if (!result.Succeeded) {
            foreach (var err in result.Errors) {
                ModelState.AddModelError(nameof(model.NewPassword), err.Description);
            }
            return await PartialViewAsync(model);
        }
        await userManager.RemovePasswordAsync(user);
        result = await userManager.AddPasswordAsync(user, model.NewPassword);
        if (!result.Succeeded) {
            foreach (var err in result.Errors) {
                ModelState.AddModelError(nameof(model.NewPassword), err.Description);
            }
            return await PartialViewAsync(model);
        }
        IPasswordHasher<UserDefinition> passwordHasher = (IPasswordHasher<UserDefinition>)Manager.ServiceProvider.GetService(typeof(IPasswordHasher<UserDefinition>));
        user.PasswordHash = passwordHasher.HashPassword(user, model.NewPassword);

        // update user info
        LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
        user.LastPasswordChangedDate = DateTime.UtcNow;
        user.PasswordChangeIP = Manager.UserHostAddress;
        bool forceReload = false;
        if (user.NeedsNewPassword)
            forceReload = true; // we need to reload the page to get rid of the warning from NeedPasswordDisplay
        user.NeedsNewPassword = false;
        user.PasswordPlainText = config.SavePlainTextPassword ? model.NewPassword : null;
        user.ResetKey = null;
        user.ResetValidUntil = null;

        // finally update the user definition
        result = await userManager.UpdateAsync(user);
        if (!result.Succeeded) {
            foreach (var err in result.Errors) {
                ModelState.AddModelError(nameof(model.NewPassword), err.Description);
            }
            return await PartialViewAsync(model);
        }

        // logoff/logon for any side effects in identity (like SecurityStamp update/cookies)
        await LoginModule.UserLoginAsync(user);

        return await FormProcessedAsync(model, this.__ResStr("okSaved", "Your new password has been saved"), ForceRedirect: forceReload);
    }
}
