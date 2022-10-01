/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using Microsoft.AspNetCore.Mvc;
using YetaWF.Core.Controllers;

namespace YetaWF.Modules.PageEdit.Controllers {

    public class EditModeModuleController : ControllerImpl<YetaWF.Modules.PageEdit.Modules.EditModeModule> {

        public EditModeModuleController() { }

        public class Model {
            public Model() { }
        }

        [AllowGet]
        public ActionResult EditMode() {

            if (Manager.IsInPopup) return new EmptyResult();
            //if (Manager.CurrentPage == null || Manager.CurrentPage.Temporary) return new EmptyResult();

            if (!Manager.CurrentPage.IsAuthorized_Edit())
                return new EmptyResult();

            return View(new Model());
        }
    }
}