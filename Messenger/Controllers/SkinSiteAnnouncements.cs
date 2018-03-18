/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Messenger.Controllers {

    public class SkinSiteAnnouncementsModuleController : ControllerImpl<YetaWF.Modules.Messenger.Modules.SkinSiteAnnouncementsModule> {

        public SkinSiteAnnouncementsModuleController() { }

        [AllowGet]
        public async Task<ActionResult> SkinSiteAnnouncements() {
            await Signalr.UseAsync();
            Package currentPackage = AreaRegistration.CurrentPackage;
            await Manager.AddOnManager.AddAddOnNamedAsync(currentPackage.Domain, currentPackage.Product, "SkinSiteAnnouncements");
            return new EmptyResult();
        }
    }
}
