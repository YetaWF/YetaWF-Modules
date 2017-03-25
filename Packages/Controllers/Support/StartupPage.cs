/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Packages#License */

using YetaWF.Core.Controllers;
using System.Threading;
using System;
using YetaWF.Core.Support;
using YetaWF.Modules.Packages.DataProvider;
using YetaWF.Core.Site;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web;
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Packages.Controllers {

    // Standard MVC Controller
    // Standard MVC Controller
    // Standard MVC Controller

    public class StartupPageController : YetaWFController {

        private static Thread StartupThread = null;

        [HttpGet]
        //[Permission("xxxx")] //$$ There is no checking during initial site startup - this is to prevent access outside of initial startup
        public ActionResult Show() {

            if (SiteDefinition.INITIAL_INSTALL) {
                if (StartupThread == null) {
                    StartupThread = new Thread(() => Execute(Manager.CurrentSite));
                    StartupThread.Start();
                }
            }
#if MVC6
            return File("/Maintenance/StartupPage6.html", "text/html");
#else
            return File("/Maintenance/StartupPage5.html", "text/html");
#endif
        }

        private void Execute(SiteDefinition site) {

            // get a manager for the thread
            YetaWFManager.MakeInitialThreadInstance(site);

            PackagesDataProvider packagesDP = new PackagesDataProvider();
            QueryHelper qh = new QueryHelper();
            qh.Add("From", "Data");
            qh.Add("Sleep", "Sleep");// add extra time at end before restarting
            packagesDP.InitAll(qh);
        }
    }
}