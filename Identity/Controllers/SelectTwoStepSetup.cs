/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        [Header("Please set up a two-step authentication method to protect your account from being hijacked.")]
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
        [ExcludeDemoMode]
        public async Task<ActionResult> SelectTwoStepSetup() {
            EditModel model = new EditModel();
            Manager.NeedUser();
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                UserDefinition user = await userDP.GetItemByUserIdAsync(Manager.UserId);
                if (user == null)
                    throw new InternalError("User with id {0} not found", Manager.UserId);
                using (UserLoginInfoDataProvider logInfoDP = new UserLoginInfoDataProvider()) {
                    string ext = await logInfoDP.GetExternalLoginProviderAsync(Manager.UserId);
                    if (ext != null)
                        return View("ShowMessage", this.__ResStr("extUser", "Your account uses a {0} account - Two-step authentication must be set up using your {0} account.", ext), UseAreaViewName: false);
                }
                TwoStepAuth twoStep = new TwoStepAuth();
                List<ITwoStepAuth> list = await twoStep.GetTwoStepAuthProcessorsAsync();
                List<string> procs = (from p in list select p.Name).ToList();
                List<string> enabledTwoStepAuths = (from e in user.EnabledTwoStepAuthentications select e.Name).ToList();
                foreach (string proc in procs) {
                    ITwoStepAuth auth = await twoStep.GetTwoStepAuthProcessorByNameAsync(proc);
                    if (auth != null) {
                        ModuleAction action = await auth.GetSetupActionAsync();
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
