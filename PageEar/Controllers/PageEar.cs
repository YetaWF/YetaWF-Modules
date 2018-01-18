/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEar#License */

using YetaWF.Core.Controllers;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.PageEar.Controllers {

    public class PageEarModuleController : ControllerImpl<YetaWF.Modules.PageEar.Modules.PageEarModule> {

        public PageEarModuleController() { }

        public class Model { }

        [AllowGet]
        public ActionResult PageEar() {
            if (!Manager.EditMode && (Module.AdImage_Data.Length == 0 || Module.CoverImage_Data.Length == 0 || string.IsNullOrWhiteSpace(Module.ClickUrl)))
                return new EmptyResult();
            return View(new Model());
        }
    }
}