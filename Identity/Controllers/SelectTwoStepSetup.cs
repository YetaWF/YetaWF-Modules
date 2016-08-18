/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Core.Support.TwoStepAuthorization;
using YetaWF.Modules.Identity.DataProvider;

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

        [HttpGet]
        public ActionResult SelectTwoStepSetup() {
            EditModel model = new EditModel();
            Manager.NeedUser();
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                UserDefinition user = userDP.GetItemByUserId(Manager.UserId);
                if (user == null)
                    throw new InternalError("User with id {0} not found", Manager.UserId);
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
                                status = this.__ResStr("enabled", "(Not Enabled)");
                            model.AuthMethods.Add(new Controllers.SelectTwoStepSetupModuleController.EditModel.AuthMethod {
                                Action = action,
                                Status = status,
                                Description = auth.GetDescription()
                           });
                        }
                    }
                }
            }
            return View(model);
        }
    }
}
