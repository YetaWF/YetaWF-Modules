/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
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

        [AllowGet]
        public async Task<ActionResult> Need2FADisplay() {
            if (!Manager.Need2FA) return new EmptyResult();
            if (Manager.EditMode) return new EmptyResult();
            if (Manager.IsInPopup) return new EmptyResult();

            SelectTwoStepSetupModule mod2FA = (SelectTwoStepSetupModule) await ModuleDefinition.LoadAsync(ModuleDefinition.GetPermanentGuid(typeof(SelectTwoStepSetupModule)));
            if (mod2FA == null)
                throw new InternalError("Two-step authentication setup module not found");

            ModuleAction action2FA = await mod2FA.GetAction_SelectTwoStepSetupAsync(null);
            if (action2FA == null)
                throw new InternalError("Two-step authentication setup action not found");

            DisplayModel model = new DisplayModel {
                SetupAction = action2FA,
            };
            return View(model);
        }
    }
}
