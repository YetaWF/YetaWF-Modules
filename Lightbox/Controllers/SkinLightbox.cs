/* Copyright �2020 Softel vdm, Inc.. - https://yetawf.com/Documentation/YetaWF/Lightbox#License */

using YetaWF.Core.Controllers;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Lightbox.Controllers {

    public class SkinLightboxModuleController : ControllerImpl<YetaWF.Modules.Lightbox.Modules.SkinLightboxModule> {

        public SkinLightboxModuleController() { }

        public class Model { }

        [AllowGet]
        public ActionResult SkinLightbox(string url) {
            Model model = new Model();
            return View(model);
        }
    }
}