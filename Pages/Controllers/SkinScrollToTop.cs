/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using YetaWF.Core.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace YetaWF.Modules.Pages.Controllers {

    public class SkinScrollToTopModuleController : ControllerImpl<YetaWF.Modules.Pages.Modules.SkinScrollToTopModule> {

        public SkinScrollToTopModuleController() { }

        public class DisplayModel { }

        [AllowHttp("GET", "POST")]
        public ActionResult SkinScrollToTop() {
            DisplayModel model = new DisplayModel();
            return View(model);
        }
    }
}
