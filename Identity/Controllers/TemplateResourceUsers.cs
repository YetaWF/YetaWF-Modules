using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using YetaWF.Core.Identity;

#nullable enable

namespace YetaWF.Modules.Identity.Controllers {

    public class TemplateResourceUsersModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.TemplateResourceUsersModule> {

        public TemplateResourceUsersModuleController() { }

        [Trim]
        public class Model {

            [Caption("ResourceUsers (Required)"), Description("ResourceUsers (Required)")]
            [UIHint("YetaWF_Identity_ResourceUsers"), Required, Trim]
            public List<User> Prop1Req { get; set; }

            [Caption("ResourceUsers"), Description("ResourceUsers")]
            [UIHint("YetaWF_Identity_ResourceUsers"), Trim]
            public List<User> Prop1 { get; set; }

            [Caption("ResourceUsers (Read/Only)"), Description("ResourceUsers (read/only)")]
            [UIHint("YetaWF_Identity_ResourceUsers"), ReadOnly]
            public List<User> Prop1RO { get; set; }

            public Model() {
                Prop1Req = new List<User>();
                Prop1 = new List<User>();
                Prop1RO = new List<User>();
            }
        }

        [AllowGet]
        public ActionResult TemplateResourceUsers() {
            Model model = new Model { };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplateResourceUsers_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            model.Prop1RO = model.Prop1Req;
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }
    }
}
