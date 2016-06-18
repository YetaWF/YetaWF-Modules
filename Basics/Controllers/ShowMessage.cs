/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Basics#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;

namespace YetaWF.Modules.Basics.Controllers {

    public class ShowMessageModuleController : ControllerImpl<YetaWF.Modules.Basics.Modules.ShowMessageModule> {

        public ShowMessageModuleController() { }

        [HttpGet]
        public ActionResult ShowMessage(string message) {
            return View("ShowMessage", (object) message, UseAreaViewName: false);
        }
    }
}