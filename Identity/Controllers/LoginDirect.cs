/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Identity;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.Addons;
using YetaWF.Modules.Identity.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class LoginDirectController : YetaWFController {

        /// <summary>
        /// Log in as someone else
        /// </summary>
        [ResourceAuthorize(Info.Resource_AllowUserLogon)]
        public ActionResult LoginAs(int userId) {
            Resource.ResourceAccess.LoginAs(userId);
            string url = Manager.CurrentSite.HomePageUrl;
            return Redirect(url);
        }

        /// <summary>
        /// Log off
        /// </summary>
        public ActionResult Logoff(string nextUrl, bool resetForcedDomain = true) {
            Manager.SetSuperUserRole(false);// explicit logoff clears superuser state
            LoginConfigData config = LoginConfigDataProvider.GetConfig();
            LoginModuleController.UserLogoff();
            if (resetForcedDomain)
                YetaWFManager.SetRequestedDomain(null);
            string url = nextUrl;
            if (string.IsNullOrWhiteSpace(url))
                url = config.LoggedOffUrl;
            if (string.IsNullOrWhiteSpace(url))
                url = Manager.CurrentSite.HomePageUrl;
            return Redirect(url);
        }
    }
}