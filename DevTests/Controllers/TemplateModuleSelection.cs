/* Copyright � 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
#if MVC6
using Microsoft.AspNetCore.Mvc;
using YetaWF.Core.Support;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplateModuleSelectionModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplateModuleSelectionModule> {

        public TemplateModuleSelectionModuleController() { }

        [Trim]
        public class EditModel {

            [Caption("Module Selection"), Description("Existing module")]
            [UIHint("ModuleSelection"), Required, Trim]
            public Guid Module { get; set; }

            [Caption("Module Selection (New)"), Description("New module")]
            [UIHint("ModuleSelection"), AdditionalMetadata("New", true), Required, Trim]
            public Guid ModuleNew { get; set; }

            [Caption("Module Selection (R/O)"), Description("Existing module, read/only")]
            [UIHint("ModuleSelection")]
            [ReadOnly]
            public Guid ROModule { get; set; }

            [Caption("Module Selection (New, R/O)"), Description("New module, read/only")]
            [UIHint("ModuleSelection"), AdditionalMetadata("New", true)]
            [ReadOnly]
            public Guid ROModuleNew { get; set; }

            public EditModel() { }
        }

        [AllowGet]
        public ActionResult TemplateModuleSelection() {
            EditModel model = new EditModel {
                ROModule = Module.PermanentGuid,// use this module as displayed module
                ROModuleNew = Module.PermanentGuid,
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplateModuleSelection_Partial(EditModel model) {
            model.ROModule = Module.PermanentGuid;
            model.ROModuleNew = Module.PermanentGuid;
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }
    }
}
