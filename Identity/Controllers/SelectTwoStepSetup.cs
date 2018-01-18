/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Core.Support.TwoStepAuthorization;
using YetaWF.Modules.Identity.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class SelectTwoStepSetupModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.SelectTwoStepSetupModule> {

        public SelectTwoStepSetupModuleController() { }

        [Trim]
        [Header("Please set up one or multiple two-step authentication methods - These will be available when you log into your account, at which point you must complete one of the enabled two-step authentications.")]
        public class EditModel {
            public class AuthMethod {
                public ModuleAction Action { get; set; }
                public string Status { get; set; }
                public string Description { get; set; }
            }
            public List<AuthMethod> AuthMethods { get; set; }

            public EditModel() {
                AuthMethods = new List<AuthMethod>();
            }
        }

        [AllowGet]
        public ActionResult SelectTwoStepSetup() {
            EditModel model = new EditModel();
            Manager.NeedUser();
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                UserDefinition user = userDP.GetItemByUserId(Manager.UserId);
                if (user == null)
                    throw new InternalError("User with id {0} not found", Manager.UserId);
                using (UserLoginInfoDataProvider logInfoDP = new UserLoginInfoDataProvider()) {
                    if (logInfoDP.IsExternalUser(Manager.UserId))
                        return View("ShowMessage", this.__ResStr("extUser", "This account uses an external login provider - Two-step authentication (if available) must be set up using the external login provider."), UseAreaViewName: false);
                }
                TwoStepAuth twoStep = new TwoStepAuth();
                List<string> procs = (from p in twoStep.GetTwoStepAuthProcessors() where p.IsAvailable() select p.Name).ToList();
                List<string> enabledTwoStepAuths = (from e in user.EnabledTwoStepAuthentications select e.Name).ToList();
                foreach (string proc in procs) {
                    ITwoStepAuth auth = twoStep.GetTwoStepAuthProcessorByName(proc);
                    if (auth != null) {
                        ModuleAction action = auth.GetSetupAction();
                        if (action != null) {
                            string status;
                            if (enabledTwoStepAuths.Contains(auth.Name))
                                status = this.__ResStr("enabled", "(Enabled)");
                            else
                                status = this.__ResStr("notEnabled", "(Not Enabled)");
                            model.AuthMethods.Add(new Controllers.SelectTwoStepSetupModuleController.EditModel.AuthMethod {
                                Action = action,
                                Status = status,
                                Description = auth.GetDescription()
                           });
                        }
                    }
                }
                model.AuthMethods = (from a in model.AuthMethods orderby a.Action.LinkText select a).ToList();
            }
            return View(model);
        }
    }
}
