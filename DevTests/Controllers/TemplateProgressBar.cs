/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using Microsoft.AspNetCore.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplateProgressBarModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplateProgressBarModule> {

        public TemplateProgressBarModuleController() { }

        [Trim]
        public class Model {

            [Caption("ProgressBar"), Description("ProgressBar")]
            [UIHint("ProgressBar"), ReadOnly]
            public float Bar { get; set; }
            public float Bar_Min { get { return 0; }  }
            public float Bar_Max { get { return 100; } }

            [Caption("Value"), Description("The progress bar value")]
            [UIHint("IntValue4"), Range(0, 100), Required]
            public int Value { get; set; }

            public bool __applyShown { get { return false; } }

            public Model() {
                Value = 25;
                Bar = Value;
            }
        }

        [AllowGet]
        public ActionResult TemplateProgressBar() {
            Model model = new Model { };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplateProgressBar_Partial(Model model) {
            model.Bar = model.Value;
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }
    }
}
