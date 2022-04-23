/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DevTests.Controllers {

    public class TestExprAttributesModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TestExprAttributesModule> {

        public TestExprAttributesModuleController() { }

        [Trim]
        public class EditModel {

            [Caption("On/Off ProcessIf"), Description("")]
            [UIHint("Boolean")]
            public bool OnOff { get; set; }

            [Caption("String1"), Description("")]
            [UIHint("Text80"), StringLength(80), Trim]
            [ProcessIf(nameof(OnOff), true, Disable = true)]
            public string? String1 { get; set; }

            [Caption("String2"), Description("")]
            [UIHint("Text80"), StringLength(80), Trim]
            [ProcessIf(nameof(OnOff), true)]
            public string? String2 { get; set; }

            public EditModel() { }
        }

        [AllowGet]
        public ActionResult TestExprAttributes() {
            EditModel model = new EditModel {};
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TestExprAttributes_Partial(EditModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model, this.__ResStr("okSaved", "OK"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
        }
    }
}
