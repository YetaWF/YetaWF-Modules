/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/SiteProperties#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;

namespace YetaWF.Modules.SiteProperties.Controllers {

    public class LockedStatusModuleController : ControllerImpl<YetaWF.Modules.SiteProperties.Modules.LockedStatusModule> {

        public class Model { }

        public LockedStatusModuleController() { }

        [HttpGet]
        public ActionResult LockedStatus() {
            if (Manager.CurrentSite.IsLocked)
                return View(new Model());
            else
                return new EmptyResult();
        }
    }
}