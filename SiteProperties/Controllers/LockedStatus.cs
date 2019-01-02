/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SiteProperties#License */

using YetaWF.Core.Controllers;
using System.Threading.Tasks;
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
                return View(new Model());
            } else
                return new EmptyResult();
        }
    }
}
