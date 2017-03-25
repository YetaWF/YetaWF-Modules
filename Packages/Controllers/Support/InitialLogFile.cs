/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Packages#License */

using System;
using System.Collections.Generic;
using YetaWF.Core.Controllers;
using YetaWF.Core.Support;
using YetaWF.Modules.Packages.DataProvider;
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

    public class InitialLogFileController : YetaWFController {

        [HttpPost]
        //[Permission("xxxx")] //$$ There is no checking during initial site startup - this is to prevent access outside of initial startup
        public ActionResult GetInitialInstallLogRecords(int offset) {
            bool ended;
            List<string> records = PackagesDataProvider.RetrieveInitialInstallLog(out ended);
            if (ended) { 
#if MVC6
                records = new List<string> {
                    "*** This site has to be restarted now so the new settings can be activated ***",
                    "*** PLEASE CLOSE YOUR BROWSER AND RESTART YOUR SITE FROM VISUAL STUDIO ***",
                    "+++DONE",
                };
#else
                records = new List<string>() {
                    "*** THE SITE IS NOW RESTARTING ***",
                    "*** If your browser isn't automatically redirected within about 10-20 seconds, please close your browser and restart your site from Visual Studio ***",
                    "+++DONE",
                };
#endif
            } else {
                records.RemoveRange(0, Math.Min(offset, records.Count));
            }
            return new YJsonResult() { Data = records };
        }
    }
}