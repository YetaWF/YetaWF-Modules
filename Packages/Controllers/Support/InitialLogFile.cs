/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Modules.Packages.DataProvider;

namespace YetaWF.Modules.Packages.Controllers;

// Standard MVC Controller
// Standard MVC Controller
// Standard MVC Controller

public class InitialLogFileController : YetaWFController {

    [AllowPost]
    public async Task<ActionResult> GetInitialInstallLogRecords(int offset) {
        PackagesDataProvider.RetrieveInitialInstallLogInfo info = await PackagesDataProvider.RetrieveInitialInstallLogAsync();
        if (info.Ended) {
            info.Lines.AddRange(new List<string> {
                "*** This site has to be restarted now so the new settings can be activated ***",
                "*** DONE. PLEASE CLOSE YOUR BROWSER AND RESTART YOUR SITE FROM VISUAL STUDIO ***",
                "+++DONE",
            });
        } else {
            info.Lines.RemoveRange(0, Math.Min(offset, info.Lines.Count));
        }
        return new YJsonResult() { Data = info.Lines };
    }
}