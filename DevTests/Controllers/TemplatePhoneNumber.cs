using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using YetaWF.Core;

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplatePhoneNumberModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplatePhoneNumberModule> {

        public TemplatePhoneNumberModuleController() { }

        [Trim]
        public class Model {

            [TextAbove("National or international phone number.")]
            [Caption("Natl Or Internatl (Required)"), Description("PhoneNumber (Required)")]
            [UIHint("PhoneNumber"), StringLength(Globals.MaxPhoneNumber), Required, PhoneNumberValidation, Trim]
            public string? Prop1Req { get; set; }

            [TextAbove("National phone number only.")]
            [Caption("National (Required)"), Description("PhoneNumber (Required)")]
            [UIHint("PhoneNumber"), StringLength(Globals.MaxPhoneNumber), Required, PhoneNumberNationalValidation, Trim]
            public string? Prop2Req { get; set; }

            [Caption("Natl Or Internatl (Read/Only)"), Description("PhoneNumber (read/only)")]
            [UIHint("PhoneNumber"), ReadOnly]
            public string? Prop1RO { get; set; }

            [Caption("National (Read/Only)"), Description("PhoneNumber (read/only)")]
            [UIHint("PhoneNumber"), ReadOnly]
            public string? Prop2RO { get; set; }

            public Model() { }
        }

        [AllowGet]
        public ActionResult TemplatePhoneNumber() {
            Model model = new Model { };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplatePhoneNumber_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            model.Prop1RO = model.Prop1Req;
            model.Prop2RO = model.Prop2Req;
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }
    }
}
