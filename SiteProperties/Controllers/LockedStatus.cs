/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SiteProperties#License */

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
        public async Task<ActionResult> LockedStatus() {
            if (Manager.CurrentSite.IsLockedAny) {
                await Manager.AddOnManager.AddAddOnNamedAsync(AreaRegistration.CurrentPackage.Domain, AreaRegistration.CurrentPackage.Product, "LockedStatus");
                return View(new Model());
            } else
                return new EmptyResult();
        }
    }
}
