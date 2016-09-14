/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Web.Mvc;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;

namespace YetaWF.Modules.Identity.Controllers {

    public class UsersDisplayModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.UsersDisplayModule> {

        public UsersDisplayModuleController() { }

        public class DisplayModel {

            [Caption("Name"), Description("The name of the user")]
            [UIHint("String"), ReadOnly]
            public string UserName { get; set; }

            [Caption("Email Address"), Description("The email address of this user")]
            [UIHint("Email"), ReadOnly]
            public string Email { get; set; }

            [Caption("User Id"), Description("The internal id of this user")]
            [UIHint("IntValue"), ReadOnly]
            public int UserId { get; set; }

            [Caption("Status"), Description("The user's current account status")]
            [UIHint("Enum"), ReadOnly]
            public UserStatusEnum UserStatus { get; set; }

            [Caption("Roles"), Description("The user's roles")]
            [UIHint("YetaWF_Identity_UserRoles"), ReadOnly]
            public SerializableList<Role> RolesList { get; set; }

            [Caption("Comment"), Description("Comments")]
            [UIHint("TextArea"), ReadOnly]
            public string Comment { get; set; }

            [Caption("Verification Code"), Description("The verification code to verify the user")]
            [UIHint("String"), ReadOnly]
            public string VerificationCode { get; set; }

            [Caption("IP Address"), Description("The IP address from which the user registered on this site")]
            [UIHint("IPAddress"), StringLength(Globals.MaxIP), ReadOnly]
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

            public void SetData(UserDefinition user) {
                ObjectSupport.CopyData(user, this);
            }
        }

        [HttpGet]
        public ActionResult UsersDisplay(string userName) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                UserDefinition data = dataProvider.GetItem(userName);
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "User \"{0}\" not found."), userName);
                DisplayModel model = new DisplayModel();
                model.SetData(data);
                Module.Title = this.__ResStr("modDisplayTitle", "User {0}", userName);
                return View(model);
            }
        }
    }
}