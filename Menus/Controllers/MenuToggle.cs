/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

using Microsoft.AspNetCore.Mvc;
using YetaWF.Core.Controllers;

namespace YetaWF.Modules.Menus.Controllers {

    public class MenuToggleModuleController : ControllerImpl<YetaWF.Modules.Menus.Modules.MenuToggleModule> {

        public class Model { }

        [AllowGet]
        public ActionResult MenuToggle() {
            return View(new Model{ });
        }
    }
}