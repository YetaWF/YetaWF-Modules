/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DevTests.Controllers {

    public class TestEscapesModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TestEscapesModule> {

        public TestEscapesModuleController() { }

        [Trim]
        [Header("Special characters in the header (HeaderAttribute) < > & @ {0}")]
        public class EditModel {

            [TextAbove("Special characters in the text above the field (TextAboveAttribute) < > & @ {0}")]
            [TextBelow("Special characters in the text below the field (TextAboveAttribute) < > & @ {0}")]
            [Caption("Caption < > & @ {0}"), Description("A description < > & @ {0}")]
            [UIHint("Text80"), StringLength(80), Trim]
            public string String1 { get; set; }

            public EditModel() { }
        }

        [AllowGet]
        public ActionResult TestEscapes() {
            EditModel model = new EditModel {};
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TestEscapes_Partial(EditModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);

            return FormProcessed(model, this.__ResStr("okSaved", "Test Done - Here are some special characters in a message: < > & @ {0}"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
        }
    }
}
