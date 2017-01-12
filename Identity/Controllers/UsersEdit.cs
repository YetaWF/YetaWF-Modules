/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Collections.Generic;
using System.Web.Mvc;
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
using YetaWF.Modules.Identity.Modules;
using YetaWF.Modules.Identity.Support;

namespace YetaWF.Modules.Identity.Controllers {

    public class UsersEditModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.UsersEditModule> {

        public UsersEditModuleController() { }

        [Trim]
        public class EditModel {

            [Caption("Name"), Description("The name of the user")]
            [UIHint("Text40"), SuppressIfEqual("RegistrationType", RegistrationTypeEnum.EmailOnly), StringLength(UserDefinition.MaxVerificationCode), UserNameValidation, Required, Trim]
            public string UserName { get; set; }

            [Caption("Email Address"), Description("The email address of this user")]
            [UIHint("Email"), SuppressIfEqual("RegistrationType", RegistrationTypeEnum.NameOnly), StringLength(Globals.MaxEmail), EmailValidation, Required, Trim]
            public string Email { get; set; }

            [Caption("User Id"), Description("The internal id of this user")]
            [UIHint("IntValue"), ReadOnly]
            public int UserId { get; set; }

            [Caption("Status"), Description("The user's current account status")]
            [UIHint("Enum"), Required]
            public UserStatusEnum UserStatus { get; set; }

            [Caption("Roles"), Description("The user's roles")]
            [UIHint("YetaWF_Identity_UserRoles")]
            public SerializableList<Role> RolesList { get; set; }

            [Caption("Comment"), Description("Comments")]
            [UIHint("TextArea"), AdditionalMetadata("SourceOnly", true), StringLength(UserDefinition.MaxComment), AllowHtml]
            public string Comment { get; set; }

            [Caption("Verification Code"), Description("The verification code to verify the user")]
            [UIHint("String"), ReadOnly]
            public string VerificationCode { get; set; }

            [Caption("IP Address"), Description("The IP address from which the user registered on this site")]
            [UIHint("IPAddress"), ReadOnly]
            public string RegistrationIP { get; set; }

            [Caption("Created"), Description("The date/time the user account was created")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Created { get; set; }

            [Caption("Last Login"), Description("The last time the user logged into his/her account")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime? LastLoginDate { get; set; }
            [Caption("Last Login IP"), Description("The IP address from which the user last logged on this site")]
            [UIHint("IPAddress"), StringLength(Globals.MaxIP), ReadOnly]
            public string LastLoginIP { get; set; }
            [Caption("Last Password Change"), Description("The last time the user changed his/her password")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime? LastPasswordChangedDate { get; set; }
            [Caption("Password Change IP"), Description("The IP address from which the user last changed the password on this site")]
            [UIHint("IPAddress"), StringLength(Globals.MaxIP), ReadOnly]
            public string PasswordChangeIP { get; set; }
            [Caption("Last Activity"), Description("The last time the user did anything on this site")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime? LastActivityDate { get; set; }
            [Caption("Last Activity IP"), Description("The IP address from which the user did anything on this site")]
            [UIHint("IPAddress"), StringLength(Globals.MaxIP), ReadOnly]
            public string LastActivityIP { get; set; }

            [UIHint("Hidden")]
            public string OriginalUserName { get; set; }

            [UIHint("Hidden")]
            public RegistrationTypeEnum RegistrationType { get; set; }

            public UserDefinition GetData(UserDefinition user) {
                ObjectSupport.CopyData(this, user);
                return user;
            }

            public void SetData(UserDefinition user) {
                ObjectSupport.CopyData(user, this);
                OriginalUserName = user.UserName;
            }
        }

        [HttpGet]
        public ActionResult UsersEdit(string userName) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                LoginConfigData config = LoginConfigDataProvider.GetConfig();
                EditModel model = new EditModel {
                    RegistrationType = config.RegistrationType,
                };
                UserDefinition data = dataProvider.GetItem(userName);
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "User \"{0}\" not found."), userName);
                model.SetData(data);
                Module.Title = this.__ResStr("modEditTitle", "User {0}", userName);
                return View(model);
            }
        }

        [HttpPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult UsersEdit_Partial(EditModel model) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                string originalUserName = model.OriginalUserName;
                UserDefinition user = dataProvider.GetItem(originalUserName);
                if (user == null)
                    ModelState.AddModelError("UserName", this.__ResStr("alreadyDeleted", "The user named \"{0}\" has been removed and can no longer be updated.", originalUserName));
                if (!ModelState.IsValid)
                    return PartialView(model);

                LoginConfigData config = LoginConfigDataProvider.GetConfig();
                switch (config.RegistrationType) {
                    case RegistrationTypeEnum.NameAndEmail: {
                        List<DataProviderFilterInfo> filters = DataProviderFilterInfo.Join(null, new DataProviderFilterInfo { Field = "Email", Operator = "==", Value = model.Email, });
                        UserDefinition userExists = dataProvider.GetItem(filters);
                        if (userExists != null && user.UserName != userExists.UserName) {
                            ModelState.AddModelError("Email", this.__ResStr("emailUsed", "An account using email address {0} already exists.", model.Email));
                            return PartialView(model);
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

                user = model.GetData(user); // merge new data into original
                model.SetData(user); // and all the data back into model for final display

                switch (dataProvider.UpdateItem(originalUserName, user)) {
                    default:
                    case UpdateStatusEnum.RecordDeleted:
                        ModelState.AddModelError("Name", this.__ResStr("alreadyDeleted", "The user named \"{0}\" has been removed and can no longer be updated.", originalUserName));
                        return PartialView(model);
                    case UpdateStatusEnum.NewKeyExists:
                        ModelState.AddModelError("Name", this.__ResStr("alreadyExists", "A user named \"{0}\" already exists.", model.UserName));
                        return PartialView(model);
                    case UpdateStatusEnum.OK:
                        break;
                }
                return FormProcessed(model, this.__ResStr("okSaved", "User updated"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
            }
        }
    }
}