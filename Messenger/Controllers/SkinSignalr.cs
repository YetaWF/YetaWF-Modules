/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System.Threading.Tasks;
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
        public async Task<ActionResult> SkinSignalr() {
            await Signalr.UseAsync();
            return new EmptyResult();
        }
    }
}
