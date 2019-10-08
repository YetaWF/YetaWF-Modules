/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Serializers;
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
    public class UsersAddModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.UsersAddModule> {

        public UsersAddModuleController() { }

        [Trim]
        public class AddModel {
            [Caption("Name"), Description("The name of the user")]
            [UIHint("Text40"), SuppressIf("RegistrationType", RegistrationTypeEnum.EmailOnly), StringLength(Globals.MaxUser), UserNameValidation, Required, Trim]
            public string UserName { get; set; }

            [Caption("Email Address"), Description("The email address of this user")]
            [UIHint("Email"), SuppressIf("RegistrationType", RegistrationTypeEnum.NameOnly), StringLength(Globals.MaxEmail), EmailValidation, Required, Trim]
            public string Email { get; set; }

            [Caption("Password"), Description("The password for this user")]
            [UIHint("Password20"), StringLength(Globals.MaxPswd), Required]
            public string Password { get; set; }

            [Caption("Status"), Description("The user's current account status")]
            [UIHint("Enum"), Required]
            public UserStatusEnum UserStatus { get; set; }

            [Caption("New Password"), Description("Defines whether the user must change the password")]
            [UIHint("Boolean")]
            public bool NeedsNewPassword { get; set; }

            [Caption("Roles"), Description("The user's roles")]
            [UIHint("YetaWF_Identity_UserRoles")]
            public SerializableList<Role> RolesList { get; set; }

            [Caption("Comment"), Description("Comments")]
            [UIHint("TextAreaSourceOnly"), StringLength(UserDefinition.MaxComment)]
            public string Comment { get; set; }

            [UIHint("Hidden")]
            public RegistrationTypeEnum RegistrationType { get; set; }

            public AddModel() { }

            public UserDefinition SetData(UserDefinition data) {
                ObjectSupport.CopyData(data, this);
                return data;
            }
            public UserDefinition GetData() {
                UserDefinition data = new UserDefinition();
                ObjectSupport.CopyData(this, data);
                return data;
            }
        }

        [AllowGet]
        public async Task<ActionResult> UsersAdd() {
            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            AddModel model = new AddModel();
            model.SetData(new UserDefinition());
            model.RegistrationType = config.RegistrationType;
            return View(model);
        }


        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> UsersAdd_Partial(AddModel model) {

            if (!ModelState.IsValid)
                return PartialView(model);

            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {

                LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
                switch (config.RegistrationType) {
                    default:
                    case RegistrationTypeEnum.NameAndEmail: { // Email == model.Email
                            List<DataProviderFilterInfo> filters = DataProviderFilterInfo.Join(null, new DataProviderFilterInfo { Field = "Email", Operator = "==", Value = model.Email, });
                            UserDefinition userExists = await dataProvider.GetItemAsync(filters);
                            if (userExists != null) {
                                ModelState.AddModelError("Email", this.__ResStr("emailUsed", "An account using email address {0} already exists.", model.Email));
                                return PartialView(model);
                            }
                            break;
                        }
                    case RegistrationTypeEnum.EmailOnly:
                        model.UserName = model.Email;
                        break;
                    case RegistrationTypeEnum.NameOnly:
                        model.UserName = model.Email;
                        break;
                }

                UserDefinition user = model.GetData();

                string hashedNewPassword;
#if MVC6
                IPasswordHasher<UserDefinition> passwordHasher = (IPasswordHasher<UserDefinition>) YetaWFManager.ServiceProvider.GetService(typeof(IPasswordHasher<UserDefinition>));
                hashedNewPassword = passwordHasher.HashPassword(user, model.Password);
#else
                UserManager<UserDefinition> userManager = Managers.GetUserManager();
                hashedNewPassword = userManager.PasswordHasher.HashPassword(model.Password);
#endif
                user.PasswordPlainText = config.SavePlainTextPassword ? model.Password : null;
                user.PasswordHash = hashedNewPassword;

                if (!await dataProvider.AddItemAsync(user))
                    throw new Error(this.__ResStr("alreadyExists", "A user named \"{0}\" already exists."), model.UserName);
                return FormProcessed(model, this.__ResStr("okSaved", "New user saved"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
            }
        }
    }
}
