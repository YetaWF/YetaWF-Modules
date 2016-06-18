/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/PageEar#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Support;

namespace YetaWF.Modules.PageEar.Controllers {

    public class PageEarModuleController : ControllerImpl<YetaWF.Modules.PageEar.Modules.PageEarModule> {

        public PageEarModuleController() { }

        public class Model { }

        [HttpGet]
        public ActionResult PageEar() {
            if (!Manager.EditMode && (Module.AdImage_Data.Length == 0 || Module.CoverImage_Data.Length == 0 || string.IsNullOrWhiteSpace(Module.ClickUrl)))
                return new EmptyResult();
            return View(new Model());
        }
    }
}