/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

using YetaWF.Core.Controllers;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Basics.Controllers {

    public class ShowMessageModuleController : ControllerImpl<YetaWF.Modules.Basics.Modules.ShowMessageModule> {

        public ShowMessageModuleController() { }

        [AllowGet]
        public ActionResult ShowMessage(string message, int? code = 0) {
            if (code != null)
                Manager.CurrentResponse.StatusCode = (int)code;
            return View("ShowMessage", (object) message, UseAreaViewName: false);
        }
    }
}