/* Copyright �2020 Softel vdm, Inc.. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using YetaWF.Core.Controllers;
using System.Threading.Tasks;
using YetaWF.Core.Identity;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class RedirectLoggedOnUsersModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.RedirectLoggedOnUsersModule> {

        [AllowGet]
        public async Task<ActionResult> RedirectLoggedOnUsers() {

            if (Manager.HaveUser && !Manager.EditMode) {
                string nextUrl = await Resource.ResourceAccess.GetUserPostLoginUrlAsync(Manager.UserRoles);
                if (string.IsNullOrWhiteSpace(nextUrl))
                    nextUrl = Manager.CurrentSite.PostLoginUrl;
                if (!string.IsNullOrWhiteSpace(nextUrl))
                    return RedirectToUrl(nextUrl);
            }
            return new EmptyResult();
        }
    }
}
