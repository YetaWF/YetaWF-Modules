/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.Panels.Models;
using Microsoft.AspNetCore.Mvc;

namespace YetaWF.Modules.Panels.Controllers {

    public class ModulePanelModuleController : ControllerImpl<YetaWF.Modules.Panels.Modules.ModulePanelModule> {

        public ModulePanelModuleController() { }

        [Trim]
        public class Model {

            [UIHint("YetaWF_Panels_PanelInfo")]
            public PanelInfo PanelInfo { get; set; }

            public Model() {
                PanelInfo = new PanelInfo();
            }
        }
        [AllowGet]
        public ActionResult ModulePanel() {
            Model model = new Model { };
            model.PanelInfo = Module.PanelInfo;
            return View(model);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> ModulePanel_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            Module.PanelInfo = model.PanelInfo;
            await Module.SaveAsync();
            model.PanelInfo = Module.PanelInfo;
            return FormProcessed(model, OnClose: OnCloseEnum.UpdateInPlace, OnPopupClose: OnPopupCloseEnum.UpdateInPlace);
        }
    }
}
