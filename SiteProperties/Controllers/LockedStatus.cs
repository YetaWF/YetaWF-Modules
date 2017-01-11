/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/SiteProperties#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;

namespace YetaWF.Modules.SiteProperties.Controllers {

    public class LockedStatusModuleController : ControllerImpl<YetaWF.Modules.SiteProperties.Modules.LockedStatusModule> {

        public class Model { }

        public LockedStatusModuleController() { }

        [HttpGet]
        public ActionResult LockedStatus() {
            if (Manager.CurrentSite.IsLockedAny) {
                Manager.AddOnManager.AddAddOn(AreaRegistration.CurrentPackage.Domain, AreaRegistration.CurrentPackage.Product, "LockedStatus");
                return View(new Model());
            } else
                return new EmptyResult();
        }
    }
}