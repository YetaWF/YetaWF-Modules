/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Lightbox#License */

using Microsoft.AspNetCore.Mvc;
using YetaWF.Core.Controllers;

namespace YetaWF.Modules.Lightbox.Controllers {

    public class SkinLightboxModuleController : ControllerImpl<YetaWF.Modules.Lightbox.Modules.SkinLightboxModule> {

        public SkinLightboxModuleController() { }

        public class Model { }

        [AllowGet]
        public ActionResult SkinLightbox(string url) {
            Model model = new();
            return View(model);
        }
    }
}