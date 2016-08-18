/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Core.Support.TwoStepAuthorization;
using YetaWF.Modules.Identity.DataProvider;

namespace YetaWF.Modules.Identity.Controllers {

    public class SelectTwoStepAuthModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.SelectTwoStepAuthModule> {

        public SelectTwoStepAuthModuleController() { }

        [Trim]
        [Header("Please click on one of the available authentication methods to complete logging in.")]
        public class EditModel {
            [UIHint("Hidden")]
            public int UserId { get; set; }
            [UIHint("Hidden")]
            public string UserName { get; set; }
            [UIHint("Hidden")]
            public string UserEmail { get; set; }

            public List<ModuleAction> Actions { get; set; }

            public EditModel() {
                Actions = new List<ModuleAction>();
            }
        }

        [HttpGet]
        public ActionResult SelectTwoStepAuth(int userId, string userName, string userEmail) {
            EditModel model = new EditModel {
                UserId = userId,
                UserName = userName,
                UserEmail = userEmail,
            };
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                UserDefinition user = userDP.GetItemByUserId(userId);
                if (user == null)
                    throw new InternalError("User with id {0} not found", userId);
                TwoStepAuth twoStep = new TwoStepAuth();
                List<string> procs = (from p in twoStep.GetTwoStepAuthProcessors() where p.IsAvailable() select p.Name).ToList();
                List<string> enabledTwoStepAuths = (from e in user.EnabledTwoStepAuthentications select e.Name).ToList();
                procs = procs.Intersect(enabledTwoStepAuths).ToList();
                foreach (string proc in procs) {
                    ITwoStepAuth auth = twoStep.GetTwoStepAuthProcessorByName(proc);
                    if (auth != null) {
                        model.Actions.New(auth.GetLoginAction(userId, userName, userEmail));
                    }
                }
                if (model.Actions.Count == 0)
                    throw new InternalError("There are no two-step authentication methods available");
            }
            return View(model);
        }
    }
}
