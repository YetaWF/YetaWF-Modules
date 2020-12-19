/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using YetaWF.Core.Controllers;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

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
