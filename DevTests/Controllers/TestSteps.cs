/* Copyright � 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DevTests.Controllers {

    public class TestStepsModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TestStepsModule> {

        public TestStepsModuleController() { }

        [Trim]
        [Header(@"This test page is used to test the DisplaySteps module, which uses client-side code to activate/enable individual steps (using notifications usually sent by other modules on the same page). " +
            "The individual steps can be edited in the DisplaySteps module by switching to Site Edit mode.")]
        public class Model { }

        [AllowGet]
        public ActionResult TestSteps() {
            Model model = new Model { };
            return View(model);
        }
    }
}
