/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

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

    public class NeedNewPasswordDisplayModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.NeedNewPasswordDisplayModule> {

        public NeedNewPasswordDisplayModuleController() { }

        public class DisplayModel {
            public ModuleAction ChangePasswordAction { get; set; }
        }

        [AllowGet]
        public async Task<ActionResult> NeedNewPasswordDisplay() {
            if (!Manager.NeedNewPassword) return new EmptyResult();
            if (Manager.EditMode) return new EmptyResult();
            if (Manager.IsInPopup) return new EmptyResult();

            UserPasswordModule modNewPassword = (UserPasswordModule) await ModuleDefinition.LoadAsync(ModuleDefinition.GetPermanentGuid(typeof(UserPasswordModule)));
            if (modNewPassword == null)
                throw new InternalError("UserPasswordModule module not found");

            ModuleAction actionNewPassword = modNewPassword.GetAction_UserPassword(null);
            if (actionNewPassword == null)
                throw new InternalError("Change password action not found");

            DisplayModel model = new DisplayModel {
                ChangePasswordAction = actionNewPassword,
            };
            return View(model);
        }
    }
}
