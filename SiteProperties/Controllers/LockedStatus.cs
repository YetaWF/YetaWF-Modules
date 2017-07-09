/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SiteProperties#License */

using YetaWF.Core.Controllers;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.SiteProperties.Controllers {

    public class LockedStatusModuleController : ControllerImpl<YetaWF.Modules.SiteProperties.Modules.LockedStatusModule> {

        public class Model { }

        public LockedStatusModuleController() { }

        [AllowGet]
        public ActionResult LockedStatus() {
            if (Manager.CurrentSite.IsLockedAny) {
                Manager.AddOnManager.AddAddOnNamed(AreaRegistration.CurrentPackage.Domain, AreaRegistration.CurrentPackage.Product, "LockedStatus");
                return View(new Model());
            } else
                return new EmptyResult();
        }
    }
}
