/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.Modules;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class Need2FADisplayModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.Need2FADisplayModule> {

        public Need2FADisplayModuleController() { }

        public class DisplayModel {
            public ModuleAction SetupAction { get; set; }
        }

        [HttpGet]
        public ActionResult Need2FADisplay() {
            if (!Manager.Need2FA) return new EmptyResult();
            if (Manager.EditMode) return new EmptyResult();
            if (Manager.IsInPopup) return new EmptyResult();

            SelectTwoStepSetupModule mod2FA = (SelectTwoStepSetupModule)ModuleDefinition.Load(ModuleDefinition.GetPermanentGuid(typeof(SelectTwoStepSetupModule)));
            if (mod2FA == null)
                throw new InternalError("Two-step authentication setup module not found");

            ModuleAction action2FA = mod2FA.GetAction_SelectTwoStepSetup(null);
            if (action2FA == null)
                throw new InternalError("Two-step authentication setup action not found");

            DisplayModel model = new DisplayModel {
                SetupAction = action2FA,
            };
            return View(model);
        }
    }
}
