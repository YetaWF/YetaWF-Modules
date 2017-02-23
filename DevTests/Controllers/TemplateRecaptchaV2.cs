using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Views.Shared;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplateRecaptchaV2ModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplateRecaptchaV2Module> {

        public TemplateRecaptchaV2ModuleController() { }

        [Trim]
        public class Model {

            [Caption("Captcha"), Description("Please verify that you're a human and not a spam bot")]
            [UIHint("RecaptchaV2"), RecaptchaV2("Please verify that you're a human and not a spam bot")]
            public RecaptchaV2Data Prop1 { get; set; }

            public Model() {
                Prop1 = new RecaptchaV2Data();
            }
        }

        [HttpGet]
        public ActionResult TemplateRecaptchaV2() {
            Model model = new Model { };
            return View(model);
        }

        [HttpPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplateRecaptchaV2_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }
    }
}
