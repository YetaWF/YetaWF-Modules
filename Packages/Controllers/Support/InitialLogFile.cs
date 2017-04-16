/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using System;
using System.Collections.Generic;
using YetaWF.Core.Controllers;
using YetaWF.Core.Support;
using YetaWF.Modules.Packages.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.SessionState;
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Packages.Controllers {

    // Standard MVC Controller
    // Standard MVC Controller
    // Standard MVC Controller

#if MVC6
#else
    [SessionState(SessionStateBehavior.Disabled)]
#endif
    public class InitialLogFileController : YetaWFController {

        [HttpPost]
        public ActionResult GetInitialInstallLogRecords(int offset) {
            bool ended;
            List<string> records = PackagesDataProvider.RetrieveInitialInstallLog(out ended);
            if (ended) {
#if MVC6
                records.AddRange(new List<string> {
                    "*** This site has to be restarted now so the new settings can be activated ***",
                    "*** DONE. PLEASE CLOSE YOUR BROWSER AND RESTART YOUR SITE FROM VISUAL STUDIO ***",
                    "+++DONE",
                });
#else
                records.AddRange(new List<string> {
                    "*** DONE. THE SITE IS NOW RESTARTING ***",
                    "+++DONE",
                });
#endif
            } else {
                records.RemoveRange(0, Math.Min(offset, records.Count));
            }
            return new YJsonResult() { Data = records };
        }
    }
}