/* Copyright �2020 Softel vdm, Inc.. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Models;
using YetaWF.Modules.Identity.Modules;
using YetaWF.Modules.Identity.Support;
using System.Linq;
#if MVC6
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
#else
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class UserAccountModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.UserAccountModule> {

        public UserAccountModuleController() { }

        [Trim]
        public class EditModel {

            [Caption("Name"), Description("Your user name")]
            [UIHint("Text40"), StringLength(Globals.MaxUser), UserNameValidation, Trim]
            [SuppressIf(nameof(RegistrationType), RegistrationTypeEnum.EmailOnly)]
            [RequiredIfNot(nameof(RegistrationType), RegistrationTypeEnum.EmailOnly)]
            public string UserName { get; set; }

            [Caption("Email Address"), Description("Your email address used by this site  to communicate with you")]
            [UIHint("Email"), StringLength(Globals.MaxEmail), EmailValidation, Trim]
            [SuppressIf(nameof(RegistrationType), RegistrationTypeEnum.NameOnly)]
            [RequiredIfNot(nameof(RegistrationType), RegistrationTypeEnum.NameOnly)]
            public string Email { get; set; }

            [Caption("Last Login"), Description("The last time you logged into your account")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime? LastLoginDate { get; set; }

            [Caption("Last Login IP"), Description("The IP address from which the user last logged on this site")]
            [UIHint("IPAddress"), StringLength(Globals.MaxIP), ReadOnly]
            public string LastLoginIP { get; set; }

            [Caption("Last Password Change"), Description("The last time you changed your password")]
            [UIHint("DateTime"), ReadOnly]
            [SuppressEmpty]
            public DateTime? LastPasswordChangedDate { get; set; }
            [Caption("Password Change IP"), Description("The IP address from which the user last changed the password on this site")]
            [UIHint("IPAddress"), StringLength(Globals.MaxIP), ReadOnly]
            [SuppressEmpty]
            public string PasswordChangeIP { get; set; }

            [UIHint("Hidden")]
            public RegistrationTypeEnum RegistrationType { get; set; }
            [UIHint("Hidden")]
            public string OriginalUserName { get; set; }

            public UserDefinition GetData() {
                UserDefinition data = new UserDefinition();
                ObjectSupport.CopyData(this, data);
                return data;
            }

            public void SetData(UserDefinition data) {
                ObjectSupport.CopyData(data, this);
            }
        }

        [AllowGet]
        public async Task<ActionResult> UserAccount()
        {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
                EditModel model = new EditModel {
                    RegistrationType = config.RegistrationType,
                };

                // make sure this user exists
#if MVC6
                if (!Manager.CurrentContext.User.Identity.IsAuthenticated)
#else
                if (!Manager.CurrentRequest.IsAuthenticated)
#endif
                {
                    throw new Error(this.__ResStr("noUser", "There is no logged on user."));
                }
                string userName = User.Identity.Name;
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
                model.OriginalUserName = user.UserName;

                Module.Title = this.__ResStr("modEditTitle", "User Account");
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> UserAccount_Partial(EditModel model) {
            // make sure this user exists
            UserManager<UserDefinition> userManager = Managers.GetUserManager();
            UserDefinition user;
#if MVC6
            user = await userManager.FindByNameAsync(model.OriginalUserName);
#else
            user = userManager.FindByName(model.OriginalUserName);
#endif
            if (user == null)
                throw new Error(this.__ResStr("alreadyDeleted", "The user named \"{0}\" has been removed and can no longer be updated.", model.OriginalUserName));
            if (!ModelState.IsValid)
                return PartialView(model);

            // update email/user name - can't use an existing email address
            // get the registration module for some defaults
            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            switch (config.RegistrationType) {
                default:
                case RegistrationTypeEnum.NameAndEmail: {
                    using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                        List<DataProviderFilterInfo> filters = DataProviderFilterInfo.Join(null, new DataProviderFilterInfo { Field = nameof(UserDefinition.Email), Operator = "==", Value = model.Email, });
                        UserDefinition userExists = await dataProvider.GetItemAsync(filters);
                        if (userExists != null && user.UserName != userExists.UserName) {
                            ModelState.AddModelError(nameof(model.Email), this.__ResStr("emailUsed", "An account using email address {0} already exists.", model.Email));
                            return PartialView(model);
                        }
                    }
                    break;
                 }
                case RegistrationTypeEnum.EmailOnly:
                    model.UserName = model.Email;
                    break;
                case RegistrationTypeEnum.NameOnly:
                    model.Email = model.UserName;
                    break;
            }

            // save new user info
            ObjectSupport.CopyData(model, user); // merge new data into original
            model.SetData(user); // and all the data back into model for final display

            if (model.OriginalUserName != user.UserName) {
                // user name changed - change data through data provider directly
                using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                    UpdateStatusEnum status = await dataProvider.UpdateItemAsync(model.OriginalUserName, user);
                    switch (status) {
                        default:
                        case UpdateStatusEnum.RecordDeleted:
                            ModelState.AddModelError(nameof(model.UserName), this.__ResStr("alreadyDeleted", "The user named \"{0}\" has been removed and can no longer be updated.", model.OriginalUserName));
                            return PartialView(model);
                        case UpdateStatusEnum.NewKeyExists:
                            ModelState.AddModelError(nameof(model.UserName), this.__ResStr("alreadyExists", "A user named \"{0}\" already exists.", model.UserName));
                            return PartialView(model);
                        case UpdateStatusEnum.OK:
                            break;
                    }
                }
                // log the user off and back on so new name takes effect
                //IAuthenticationManager authManager = HttpContext.GetOwinContext().Authentication;
                //deleted, done in UserLogff authManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.ExternalBearer);
                await LoginModuleController.UserLogoffAsync();
                await LoginModuleController.UserLoginAsync(user);
            } else {
                IdentityResult result;
                result = await userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    throw new Error(string.Join(" - ", (from e in result.Errors select e.Description)));
            }
            return FormProcessed(model, this.__ResStr("okSaved", "Your account information has been saved"), OnClose: OnCloseEnum.ReloadPage, OnPopupClose: OnPopupCloseEnum.ReloadParentPage, ForceRedirect:true);// reload for tiny login module to refresh
        }
    }
}
