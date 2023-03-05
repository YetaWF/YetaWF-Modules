/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
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

public class UserPasswordModuleDataProvider : ModuleDefinitionDataProvider<Guid, UserPasswordModule>, IInstallableModel { }

[ModuleGuid("{2ca21dad-34d0-4e2c-83c2-e3f6b31ca630}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
[ModuleCategory("Login & Registration")]
public class UserPasswordModule : ModuleDefinition2 {

    public UserPasswordModule() : base() {
        Title = this.__ResStr("modTitle", "Change Password");
        Name = this.__ResStr("modName", "Change Password");
        Description = this.__ResStr("modSummary", "Used to edit the current user's account password. The Change Password Module can be accessed using User > Change Password (standard YetaWF site).");
        DefaultViewName = StandardViews.EditApply;
        ShowPasswordRules = true;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new UserPasswordModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return UserLevel_DefaultAllowedRoles; } }

    [Category("General"), Caption("Show Password Rules"), Description("Defines whether the password rules are shown")]
    [UIHint("Boolean")]
    [Data_NewValue]
    public bool ShowPasswordRules { get; set; }

    public ModuleAction GetAction_UserPassword(string url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Edit",
            LinkText = this.__ResStr("editLink", "Change Password"),
            MenuText = this.__ResStr("editText", "Change Password"),
            Tooltip = this.__ResStr("editTooltip", "Change your account password"),
            Legend = this.__ResStr("editLegend", "Changes your account password"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class EditModel {

        [Caption("Current Password"), Description("Enter your current password for verification")]
        [UIHint("Password20"), StringLength(Globals.MaxPswd)]
        public string OldPassword { get; set; }

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

        public string UserName { get; set; }

        public void SetData(UserDefinition user) {
            UserName = user.UserName;
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {

        if (!Manager.CurrentContext.User.Identity.IsAuthenticated)
            throw new Error(this.__ResStr("noUser", "There is no logged on user"));

        string userName = Manager.CurrentContext.User.Identity.Name;

        using (UserLoginInfoDataProvider logInfoDP = new UserLoginInfoDataProvider()) {
            string ext = await logInfoDP.GetExternalLoginProviderAsync(Manager.UserId);
            if (ext != null)
                return await RenderAsync(this.__ResStr("extUser", "Your account uses a {0} account - The password must be changed using your {0} account.", ext), ViewName: "ShowMessage", UseAreaViewName: false);
        }
        UserManager<UserDefinition> userManager = Managers.GetUserManager();
        UserDefinition user = await userManager.FindByNameAsync(userName);
        if (user == null)
            throw new Error(this.__ResStr("notFound", "User \"{0}\" not found."), userName);

        EditModel model = new EditModel { };
        model.SetData(user);

        using (LoginConfigDataProvider logConfigDP = new LoginConfigDataProvider()) {
            model.PasswordRules = ShowPasswordRules ? logConfigDP.PasswordRules : null;
        }

        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(EditModel model) {
        // get current user we're changing
        if (!Manager.CurrentContext.User.Identity.IsAuthenticated)
            throw new Error(this.__ResStr("noUser", "There is no logged on user"));

        string userName = Manager.CurrentContext.User.Identity.Name;

        using (UserLoginInfoDataProvider logInfoDP = new UserLoginInfoDataProvider()) {
            string ext = await logInfoDP.GetExternalLoginProviderAsync(Manager.UserId);
            if (ext != null)
                throw new Error(this.__ResStr("extUserPswd", "This account can only be accessed using an external login provider"));
        }

        UserManager<UserDefinition> userManager = Managers.GetUserManager();
        UserDefinition user = await userManager.FindByNameAsync(userName);
        if (user == null)
            throw new Error(this.__ResStr("notFound", "User \"{0}\" not found."), userName);

        model.SetData(user);
        if (ShowPasswordRules) {
            using (LoginConfigDataProvider logConfigDP = new LoginConfigDataProvider()) {
                model.PasswordRules = logConfigDP.PasswordRules;
            }
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

        result = await userManager.ChangePasswordAsync(user, model.OldPassword ?? "", model.NewPassword);
        if (!result.Succeeded) {
            foreach (var err in result.Errors) {
                ModelState.AddModelError(nameof(model.OldPassword), err.Description);
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
        if (!result.Succeeded)
            throw new Error(string.Join(" - ", (from e in result.Errors select e.Description)));

        // logoff/logon for any side effects in identity (like SecurityStamp update/cookies)
        await LoginModule.UserLoginAsync(user);

        return await FormProcessedAsync(model, this.__ResStr("okSaved", "Your new password has been saved"), ForceRedirect: forceReload);
    }
}
