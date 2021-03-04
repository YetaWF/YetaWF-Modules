using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using Microsoft.AspNetCore.Mvc;

#nullable enable

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplateMaskedEditModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplateMaskedEditModule> {

        public TemplateMaskedEditModuleController() { }

        [Trim]
        public class Model {

            public enum ControlStatusEnum { Normal, Disabled, }

            [Caption("SSN (Required)"), Description("Social Security Number (Required)")]
            [UIHint("SSN"), SSNValidation, Required, Trim]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? Prop1Req { get; set; }

            [Caption("SSN"), Description("Social Security Number")]
            [UIHint("SSN"), SSNValidation, Trim]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? Prop1 { get; set; }

            [Caption("SSN (Read/Only)"), Description("Social Security Number (read/only)")]
            [UIHint("SSN"), ReadOnly]
            public string? Prop1RO { get; set; }

            [Caption("Control Status"), Description("Defines the processing status of the controls")]
            [UIHint("Enum")]
            public ControlStatusEnum ControlStatus { get; set; }

            public Model() { }
        }

        [AllowGet]
        public ActionResult TemplateMaskedEdit() {
            Model model = new Model { };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplateMaskedEdit_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            model.Prop1RO = model.Prop1;
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }
    }
}
