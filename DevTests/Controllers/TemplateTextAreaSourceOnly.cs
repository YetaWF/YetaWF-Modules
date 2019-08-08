/* Copyright � 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplateTextAreaSourceOnlyModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplateTextAreaSourceOnlyModule> {

        public TemplateTextAreaSourceOnlyModuleController() { }

        [Trim]
        public class Model {

            public enum ControlStatusEnum { Normal, Disabled, }

            [Caption("TextAreaSourceOnly (Required)"), Description("TextAreaSourceOnly (Required)")]
            [UIHint("TextAreaSourceOnly"), StringLength(0), Trim]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string Prop1Req { get; set; }

            [Caption("TextAreaSourceOnly"), Description("TextAreaSourceOnly")]
            [UIHint("TextAreaSourceOnly"), StringLength(0), Trim]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string Prop1 { get; set; }

            [Caption("TextAreaSourceOnly (Read/Only)"), Description("TextAreaSourceOnly (read/only)")]
            [UIHint("TextAreaSourceOnly"), ReadOnly]
            public string Prop1RO { get; set; }

            [Caption("Control Status"), Description("Defines the processing status of the controls")]
            [UIHint("Enum")]
            public ControlStatusEnum ControlStatus { get; set; }

            public Model() { }
        }

        [AllowGet]
        public ActionResult TemplateTextAreaSourceOnly() {
            Model model = new Model { };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplateTextAreaSourceOnly_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            model.Prop1RO = model.Prop1Req;
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }
    }
}
