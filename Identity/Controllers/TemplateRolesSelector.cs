/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Identity;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Serializers;
#if MVC6
using Microsoft.AspNetCore.Mvc;
using YetaWF.Core.Support;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class TemplateRolesSelectorModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.TemplateRolesSelectorModule> {

        public TemplateRolesSelectorModuleController() { }

        [Trim]
        public class Model {

            [Caption("RolesSelector (Required)"), Description("RolesSelector (Required)")]
            [UIHint("YetaWF_Identity_RolesSelector"), AdditionalMetadata("ExcludeUser2FA", false), AdditionalMetadata("ShowFilter", true), Required]
            public SerializableList<Role> Prop1Req { get; set; }

            [Caption("RolesSelector"), Description("RolesSelector")]
            [UIHint("YetaWF_Identity_RolesSelector"), AdditionalMetadata("ExcludeUser2FA", true), Trim]
            public SerializableList<Role> Prop1 { get; set; }

            [Caption("RolesSelector (Read/Only)"), Description("RolesSelector (read/only)")]
            [UIHint("YetaWF_Identity_RolesSelector"), ReadOnly]
            public SerializableList<Role> Prop1RO { get; set; }

            public Model() {
                Prop1Req = new SerializableList<Role>();
                Prop1 = new SerializableList<Role>();
                List<RoleInfo> allRoles = Resource.ResourceAccess.GetDefaultRoleList();
                Prop1RO = new SerializableList<Role>((from RoleInfo a in allRoles select new Role { RoleId = a.RoleId }).ToList());
            }
        }

        [AllowGet]
        public ActionResult TemplateRolesSelector() {
            Model model = new Model { };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplateRolesSelector_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }
    }
}
