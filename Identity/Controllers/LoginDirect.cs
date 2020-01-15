/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
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
        public async Task<ActionResult> LoginAs(int userId) {
            await Resource.ResourceAccess.LoginAsAsync(userId);
            string url = Manager.CurrentSite.HomePageUrl;
            url = QueryHelper.AddRando(url); // to defeat client-side caching
            return Redirect(url);
        }

        /// <summary>
        /// Log off
        /// </summary>
        public async Task<ActionResult> Logoff(string nextUrl) {
            Manager.SetSuperUserRole(false);// explicit logoff clears superuser state
            await LoginModuleController.UserLogoffAsync();
            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            string url = nextUrl;
            if (string.IsNullOrWhiteSpace(url))
                url = config.LoggedOffUrl;
            if (string.IsNullOrWhiteSpace(url))
                url = Manager.CurrentSite.HomePageUrl;
            // Because this is a plain MVC controller, this Redirect really redirects (no Unified Page Set handling) which is the desired behavior
            url = QueryHelper.AddRando(url); // to defeat client-side caching
            return Redirect(url);
        }
        /// <summary>
        /// Log off current user.
        /// </summary>
        [AllowPost]
        public async Task<ActionResult> LogoffDirect() {
            Manager.SetSuperUserRole(false);// explicit logoff clears superuser state
            await LoginModuleController.UserLogoffAsync();
            return new EmptyResult();
        }
    }
}
