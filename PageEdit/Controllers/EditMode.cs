/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using YetaWF.Core.Controllers;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.PageEdit.Controllers {

    public class EditModeModuleController : ControllerImpl<YetaWF.Modules.PageEdit.Modules.EditModeModule> {

        public EditModeModuleController() { }

        public class Model {
            public Model() { }
        }

        [AllowGet]
        public ActionResult EditMode() {
            return View(new Model());
        }
    }
}