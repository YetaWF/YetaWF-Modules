/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Identity;
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
            [UIHint("Password20")]
            public string OldPassword { get; set; }

            [Caption("New Password"), Description("Enter your desired new password")]
            [UIHint("Password20"), Required]
            public string NewPassword { get; set; }

            [Caption("New Password Confirmation"), Description("Enter your new password again to confirm")]
            [UIHint("Password20"), Required, SameAs("NewPassword", "The password confirmation doesn't match the password entered")]
            public string ConfirmPassword { get; set; }

            public string UserName { get; set; }
            public string LoginProvider { get; set; }
            public bool HavePassword { get; set; }

            public void SetData(UserDefinition user) {
                UserName = user.UserName;
                LoginProvider = (from l in user.LoginInfoList select l.LoginProvider).FirstOrDefault();
                HavePassword = user.PasswordHash != null;
            }

        }

        [HttpGet]
        public ActionResult UserPassword() {

            IAuthenticationManager authManager = Manager.CurrentRequest.GetOwinContext().Authentication;
            if (!Manager.CurrentRequest.IsAuthenticated)
                throw new Error(this.__ResStr("noUser", "There is no logged on user"));
            string userName = User.Identity.Name;

            UserManager<UserDefinition> userManager = Managers.GetUserManager();
            UserDefinition user = userManager.FindByName(userName);
            if (user == null)
                throw new Error(this.__ResStr("notFound", "User \"{0}\" not found."), userName);

            EditModel model = new EditModel { };
            model.SetData(user);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserPassword_Partial(EditModel model) {
            // get current user we're changing
            IAuthenticationManager authManager = Manager.CurrentRequest.GetOwinContext().Authentication;
            if (!Manager.CurrentRequest.IsAuthenticated)
                throw new Error(this.__ResStr("noUser", "There is no logged on user"));
            string userName = User.Identity.Name;

            UserManager<UserDefinition> userManager = Managers.GetUserManager();
            UserDefinition user = userManager.FindByName(userName);
            if (user == null)
                throw new Error(this.__ResStr("notFound", "User \"{0}\" not found."), userName);

            model.SetData(user);
            if (!ModelState.IsValid)
                return PartialView(model);

            // change the password
            IdentityResult result = userManager.PasswordValidator.ValidateAsync(model.NewPassword).Result;
            if (!result.Succeeded) {
                foreach (string err in result.Errors)
                    ModelState.AddModelError("NewPassword", err);
                return PartialView(model);
            }
            if (model.LoginProvider != null && !model.HavePassword) {
                // first time setting the password after logging in using an external login provider
                user.PasswordHash = userManager.PasswordHasher.HashPassword(model.NewPassword);
            } else {
                result = userManager.ChangePassword(user.Id, model.OldPassword??"", model.NewPassword);
                if (!result.Succeeded) {
                    foreach (string err in result.Errors)
                        ModelState.AddModelError("OldPassword", err);
                    return PartialView(model);
                }
                user.PasswordHash = userManager.PasswordHasher.HashPassword(model.NewPassword);
            }

            // update user info
            LoginConfigData config = LoginConfigDataProvider.GetConfig();
            user.LastPasswordChangedDate = DateTime.UtcNow;
            user.PasswordChangeIP = Manager.UserHostAddress;
            if (config.SavePlainTextPassword)
                user.PasswordPlainText = model.NewPassword;

            // finally update the user definition
            result = userManager.Update(user);
            if (!result.Succeeded) {
                foreach (string err in result.Errors)
                    ModelState.AddModelError("", err);
                return PartialView(model);
            }
            return FormProcessed(model, this.__ResStr("okSaved", "Your new password has been saved"));
        }
    }
}