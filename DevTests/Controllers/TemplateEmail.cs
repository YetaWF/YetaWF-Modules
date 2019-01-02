/* Copyright � 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplateEmailModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplateEmailModule> {

        public TemplateEmailModuleController() { }

        [Trim]
        public class Model {

            [Caption("Email (Required)"), Description("Email (Required)")]
            [UIHint("Email"), StringLength(Globals.MaxEmail), Required, EmailValidation, Trim]
            public string EmailReq { get; set; }

            [Caption("Email"), Description("Email")]
            [UIHint("Email"), StringLength(Globals.MaxEmail), EmailValidation, Trim]
            public string Email { get; set; }

            [Caption("Email (Read/Only)"), Description("Email (read/only)")]
            [UIHint("Email"), ReadOnly]
            public string EmailRO { get; set; }

            public Model() {
                EmailRO = "mikevdm@mikevdm.com";
            }
        }

        [AllowGet]
        public ActionResult TemplateEmail() {
            Model model = new Model { };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplateEmail_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }
    }
}
