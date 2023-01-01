/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.Panels.Models;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Panels.Controllers {

    public class DisplayStepsModuleController : ControllerImpl<YetaWF.Modules.Panels.Modules.DisplayStepsModule> {

        public DisplayStepsModuleController() { }

        public class Model {

            [UIHint("YetaWF_Panels_StepInfo")]
            public StepInfo StepInfo { get; set; }

            public Model() {
                StepInfo = new StepInfo();
            }
        }

        [AllowGet]
        public ActionResult DisplaySteps() {
            if (Manager.IsInPopup) return new EmptyResult();
            Model model = new Model();
            model.StepInfo = Module.StepInfo;
            return View(model);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> DisplaySteps_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            Module.StepInfo = model.StepInfo;
            await Module.SaveAsync();
            model.StepInfo = Module.StepInfo;
            return FormProcessed(model, OnClose: OnCloseEnum.UpdateInPlace, OnPopupClose: OnPopupCloseEnum.UpdateInPlace);
        }
    }
}
