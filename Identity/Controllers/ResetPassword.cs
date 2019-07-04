/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Models;
using System.Threading.Tasks;
using YetaWF.Core;
#if MVC6
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
#else
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class ResetPasswordModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.ResetPasswordModule> {

        public ResetPasswordModuleController() { }

        [Trim]
        public class EditModel {

            [Caption("Reset Key"), Description("Enter your reset key you received in the email we sent to you in order to reset your password")]
            [UIHint("Text40"), StringLength(80)]
            public string ResetKey { get; set; }

            [Caption("New Password"), Description("Enter your desired new password")]
            [UIHint("Password20"), StringLength(Globals.MaxPswd), Required]
            public string NewPassword { get; set; }

            [Caption("New Password Confirmation"), Description("Enter your new password again to confirm")]
            [UIHint("Password20"), StringLength(Globals.MaxPswd), Required, SameAs("NewPassword", "The password confirmation doesn't match the password entered")]
            public string ConfirmPassword { get; set; }

            [UIHint("Hidden"), ReadOnly]
            public int UserId { get; set; }
        }

        [AllowGet]
        public async Task<ActionResult> ResetPassword(int userId = 0, string resetKey = null) {

            UserManager<UserDefinition> userManager = Managers.GetUserManager();
            UserDefinition user;
#if MVC6
            user = await userManager.FindByIdAsync(userId.ToString());
#else
            user = userManager.FindById(userId.ToString());
#endif
            if (user == null)
                throw new Error(this.__ResStr("notFound", "User account not found."));
            using (UserLoginInfoDataProvider logInfoDP = new UserLoginInfoDataProvider()) {
                if (await logInfoDP.IsExternalUserAsync(user.UserId))
                    return View("ShowMessage", this.__ResStr("extUser", "This account uses an external login provider - The password (if available) must be set up using the external login provider."), UseAreaViewName: false);
            }

            EditModel model = new EditModel {
                ResetKey = resetKey,
                UserId = userId,
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> ResetPassword_Partial(EditModel model) {

            UserManager<UserDefinition> userManager = Managers.GetUserManager();
            UserDefinition user;
#if MVC6
            user = await userManager.FindByIdAsync(model.UserId.ToString());
#else
            user = userManager.FindById(model.UserId.ToString());
#endif
            if (user == null)
                throw new Error(this.__ResStr("notFound", "User account not found."));
            using (UserLoginInfoDataProvider logInfoDP = new UserLoginInfoDataProvider()) {
                if (await logInfoDP.IsExternalUserAsync(model.UserId))
                    throw new Error(this.__ResStr("extUser", "This account uses an external login provider - The password (if available) must be set up using the external login provider."));
            }

            if (!ModelState.IsValid)
                return PartialView(model);

            Guid? resetKey = null;
            if (user.ResetKey == null || user.ResetValidUntil == null || user.ResetValidUntil < DateTime.UtcNow) {
                ModelState.AddModelError(nameof(model.ResetKey), this.__ResStr("expired", "The reset key has expired and is not longer valid"));
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
                return PartialView(model);

            // change the password
            IdentityResult result;
#if MVC6
            IPasswordValidator<UserDefinition> passVal = (IPasswordValidator<UserDefinition>)YetaWFManager.ServiceProvider.GetService(typeof(IPasswordValidator<UserDefinition>));
            result = await passVal.ValidateAsync(userManager, user, model.NewPassword);
#else
            result = await userManager.PasswordValidator.ValidateAsync(model.NewPassword);
#endif
            if (!result.Succeeded) {
                foreach (var err in result.Errors) {
#if MVC6
                    ModelState.AddModelError("NewPassword", err.Description);
#else
                    ModelState.AddModelError("NewPassword", err);
#endif
                }
                return PartialView(model);
            }
#if MVC6
            await userManager.RemovePasswordAsync(user);
            result = await userManager.AddPasswordAsync(user, model.NewPassword);
#else
            //$$$$$$
            result = userManager.ChangePassword(user.Id, model.OldPassword ?? "", model.NewPassword);
#endif
            if (!result.Succeeded) {
                foreach (var err in result.Errors) {
#if MVC6
                    ModelState.AddModelError(nameof(model.NewPassword), err.Description);
#else
                    ModelState.AddModelError(nameof(model.NewPassword), err);
#endif
                }
                return PartialView(model);
            }
#if MVC6
            IPasswordHasher<UserDefinition> passwordHasher = (IPasswordHasher<UserDefinition>) YetaWFManager.ServiceProvider.GetService(typeof(IPasswordHasher<UserDefinition>));
            user.PasswordHash = passwordHasher.HashPassword(user, model.NewPassword);
#else
            user.PasswordHash = userManager.PasswordHasher.HashPassword(model.NewPassword);
#endif

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
#if MVC6
            result = await userManager.UpdateAsync(user);
#else
            result = userManager.Update(user);
#endif
            if (!result.Succeeded) {
                foreach (var err in result.Errors) {
#if MVC6
                    ModelState.AddModelError(nameof(model.NewPassword), err.Description);
#else
                    ModelState.AddModelError(nameof(model.NewPassword), err);
#endif
                }
                return PartialView(model);
            }

            // logoff/logon for any side effects in identity (like SecurityStamp update/cookies)
            await LoginModuleController.UserLoginAsync(user);

            return FormProcessed(model, this.__ResStr("okSaved", "Your new password has been saved"), ForceRedirect: forceReload);
        }
    }
}
