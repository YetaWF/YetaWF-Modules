/* Copyright �2020 Softel vdm, Inc.. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Models;

namespace YetaWF.Modules.Identity.Controllers {

    public class UserPasswordModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.UserPasswordModule> {

        public UserPasswordModuleController() { }

        [Trim]
        public class EditModel {

            [Caption("Current Password"), Description("Enter your current password for verification")]
            [UIHint("Password20"), StringLength(Globals.MaxPswd)]
            public string OldPassword { get; set; }

            [Caption("New Password"), Description("Enter your desired new password")]
            [UIHint("Password20"), StringLength(Globals.MaxPswd), Required]
            public string NewPassword { get; set; }

            [Caption(" "), Description("")]
            [UIHint("String"), ReadOnly]
            [SuppressEmpty]
            public string PasswordRules { get; set; }

            [Caption("New Password Confirmation"), Description("Enter your new password again to confirm")]
            [UIHint("Password20"), StringLength(Globals.MaxPswd), Required, SameAs("NewPassword", "The password confirmation doesn't match the password entered")]
            public string ConfirmPassword { get; set; }

            public string UserName { get; set; }

            public void SetData(UserDefinition user) {
                UserName = user.UserName;
            }
        }

        [AllowGet]
        public async Task<ActionResult> UserPassword() {

            if (!Manager.CurrentContext.User.Identity.IsAuthenticated)
                throw new Error(this.__ResStr("noUser", "There is no logged on user"));

            string userName = User.Identity.Name;

            using (UserLoginInfoDataProvider logInfoDP = new UserLoginInfoDataProvider()) {
                string ext = await logInfoDP.GetExternalLoginProviderAsync(Manager.UserId);
                if (ext != null)
                    return View("ShowMessage", this.__ResStr("extUser", "Your account uses a {0} account - The password must be changed using your {0} account.", ext), UseAreaViewName: false);
            }
            UserManager<UserDefinition> userManager = Managers.GetUserManager();
            UserDefinition user = await userManager.FindByNameAsync(userName);
            if (user == null)
                throw new Error(this.__ResStr("notFound", "User \"{0}\" not found."), userName);

            EditModel model = new EditModel { };
            model.SetData(user);

            using (LoginConfigDataProvider logConfigDP = new LoginConfigDataProvider()) {
                model.PasswordRules = Module.ShowPasswordRules ? logConfigDP.PasswordRules : null;
            }

            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> UserPassword_Partial(EditModel model) {
            // get current user we're changing
            if (!Manager.CurrentContext.User.Identity.IsAuthenticated)
                throw new Error(this.__ResStr("noUser", "There is no logged on user"));

            string userName = User.Identity.Name;

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
            if (Module.ShowPasswordRules) {
                using (LoginConfigDataProvider logConfigDP = new LoginConfigDataProvider()) {
                    model.PasswordRules = logConfigDP.PasswordRules;
                }
            }

            if (!ModelState.IsValid)
                return PartialView(model);

            // change the password
            IPasswordValidator<UserDefinition> passVal = (IPasswordValidator<UserDefinition>)YetaWFManager.ServiceProvider.GetService(typeof(IPasswordValidator<UserDefinition>));
            IdentityResult result = await passVal.ValidateAsync(userManager, user, model.NewPassword);
            if (!result.Succeeded) {
                foreach (var err in result.Errors) {
                    ModelState.AddModelError(nameof(model.NewPassword), err.Description);
                }
                return PartialView(model);
            }

            result = await userManager.ChangePasswordAsync(user, model.OldPassword ?? "", model.NewPassword);
            if (!result.Succeeded) {
                foreach (var err in result.Errors) {
                    ModelState.AddModelError(nameof(model.OldPassword), err.Description);
                }
                return PartialView(model);
            }

            IPasswordHasher<UserDefinition> passwordHasher = (IPasswordHasher<UserDefinition>) YetaWFManager.ServiceProvider.GetService(typeof(IPasswordHasher<UserDefinition>));
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
            await LoginModuleController.UserLoginAsync(user);

            return FormProcessed(model, this.__ResStr("okSaved", "Your new password has been saved"), ForceRedirect: forceReload);
        }
    }
}
