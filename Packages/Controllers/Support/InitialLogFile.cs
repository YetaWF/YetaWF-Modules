/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using System;
using System.Collections.Generic;
using YetaWF.Core.Controllers;
using YetaWF.Core.Support;
using YetaWF.Modules.Packages.DataProvider;
using System.Threading.Tasks;
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

        [AllowPost]
        public async Task<ActionResult> GetInitialInstallLogRecords(int offset) {
            PackagesDataProvider.RetrieveInitialInstallLogInfo info = await PackagesDataProvider.RetrieveInitialInstallLogAsync();
            if (info.Ended) {
#if MVC6
                info.Lines.AddRange(new List<string> {
                    "*** This site has to be restarted now so the new settings can be activated ***",
                    "*** DONE. PLEASE CLOSE YOUR BROWSER AND RESTART YOUR SITE FROM VISUAL STUDIO ***",
                    "+++DONE",
                });
#else
                info.Lines.AddRange(new List<string> {
                    "*** DONE. THE SITE IS NOW RESTARTING ***",
                    "+++DONE",
                });
#endif
            } else {
                info.Lines.RemoveRange(0, Math.Min(offset, info.Lines.Count));
            }
            return new YJsonResult() { Data = info.Lines };
        }
    }
}