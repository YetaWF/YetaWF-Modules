/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Lightbox#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;

namespace YetaWF.Modules.Lightbox.Controllers {

    public class SkinLightboxModuleController : ControllerImpl<YetaWF.Modules.Lightbox.Modules.SkinLightboxModule> {

        public SkinLightboxModuleController() { }

        public class Model { }

        [HttpGet]
        public ActionResult SkinLightbox(string url) {
            Model model = new Model();
            return View(model);
        }
    }
}