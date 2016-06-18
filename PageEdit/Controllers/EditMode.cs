/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;

namespace YetaWF.Modules.PageEdit.Controllers {

    public class EditModeModuleController : ControllerImpl<YetaWF.Modules.PageEdit.Modules.EditModeModule> {

        public EditModeModuleController() { }

        public class Model {
            public Model() { }
        }

        [HttpGet]
        public ActionResult EditMode() {
            return View(new Model());
        }
    }
}