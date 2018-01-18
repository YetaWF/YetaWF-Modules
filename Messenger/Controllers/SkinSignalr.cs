/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Support;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Messenger.Controllers {

    public class SkinSignalrModuleController : ControllerImpl<YetaWF.Modules.Messenger.Modules.SkinSignalrModule> {

        public SkinSignalrModuleController() { }

        [AllowGet]
        public ActionResult SkinSignalr() {
            Signalr.Use();
            return new EmptyResult();
        }
    }
}
