/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

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
#if MVC6
            if (!Manager.CurrentContext.User.Identity.IsAuthenticated)
#else
            if (!Manager.CurrentRequest.IsAuthenticated)
#endif
            {
                throw new Error(this.__ResStr("noUser", "There is no logged on user"));
            }
            string userName = User.Identity.Name;

            using (UserLoginInfoDataProvider logInfoDP = new UserLoginInfoDataProvider()) {
                if (await logInfoDP.IsExternalUserAsync(Manager.UserId))
                    return View("ShowMessage", this.__ResStr("extUser", "This account uses an external login provider - The password (if available) must be set up using the external login provider."), UseAreaViewName: false);
            }
            UserManager<UserDefinition> userManager = Managers.GetUserManager();
            UserDefinition user;
#if MVC6
            user = await userManager.FindByNameAsync(userName);
#else
            user = userManager.FindByName(userName);
#endif
            if (user == null)
                throw new Error(this.__ResStr("notFound", "User \"{0}\" not found."), userName);

            EditModel model = new EditModel { };
            model.SetData(user);

            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> UserPassword_Partial(EditModel model) {
            // get current user we're changing
#if MVC6
            if (!Manager.CurrentContext.User.Identity.IsAuthenticated)
#else
            if (!Manager.CurrentRequest.IsAuthenticated)
#endif
            {
                throw new Error(this.__ResStr("noUser", "There is no logged on user"));
            }
            string userName = User.Identity.Name;

            using (UserLoginInfoDataProvider logInfoDP = new UserLoginInfoDataProvider()) {
                if (await logInfoDP.IsExternalUserAsync(Manager.UserId))
                    throw new Error(this.__ResStr("extUserPswd", "This account can only be accessed using an external login provider"));
            }

            UserManager<UserDefinition> userManager = Managers.GetUserManager();
            UserDefinition user;
#if MVC6
            user = await userManager.FindByNameAsync(userName);
#else
            user = userManager.FindByName(userName);
#endif
            if (user == null)
                throw new Error(this.__ResStr("notFound", "User \"{0}\" not found."), userName);

            model.SetData(user);
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
            result = await userManager.ChangePasswordAsync(user, model.OldPassword ?? "", model.NewPassword);
#else
            result = userManager.ChangePassword(user.Id, model.OldPassword ?? "", model.NewPassword);
#endif
            if (!result.Succeeded) {
                foreach (var err in result.Errors) {
#if MVC6
                    ModelState.AddModelError("OldPassword", err.Description);
#else
                    ModelState.AddModelError("OldPassword", err);
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
            if (config.SavePlainTextPassword)
                user.PasswordPlainText = model.NewPassword;

            // finally update the user definition
#if MVC6
            result = await userManager.UpdateAsync(user);
#else
            result = userManager.Update(user);
#endif
            if (!result.Succeeded) {
                foreach (var err in result.Errors) {
#if MVC6
                    ModelState.AddModelError("", err.Description);
#else
                    ModelState.AddModelError("", err);
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
