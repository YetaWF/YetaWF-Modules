/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using System.Threading.Tasks;
using YetaWF.Modules.ImageRepository.Components;
using Microsoft.AspNetCore.Mvc;

namespace YetaWF.Modules.ImageRepository.Controllers {

    public class TemplateTestModuleController : ControllerImpl<YetaWF.Modules.ImageRepository.Modules.TemplateTestModule> {

        public TemplateTestModuleController() { }

        [Trim]
        public class Model {

            [Caption("Image Selection"), Description("Description of image selection")]
            [UIHint("YetaWF_ImageRepository_ImageSelection"), Required, Trim]
            public string? ImageName { get; set; }
            public ImageSelectionInfo? ImageName_Info { get; set; }

            public Model() { }

            public async Task UpdateAsync(ModuleDefinition module) {
                ImageName_Info = new ImageSelectionInfo(module, module.ModuleGuid, null) {
                    AllowUpload = true,
                };
                await ImageName_Info.InitAsync();
            }
        }

        [AllowGet]
        public async Task<ActionResult> TemplateTest() {
            Model model = new Model { };
            await model.UpdateAsync(Module);
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> TemplateTest_Partial(Model model) {
            await model.UpdateAsync(Module);
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model, this.__ResStr("okSaved", "Image saved"), OnClose: OnCloseEnum.Return, OnPopupClose: OnPopupCloseEnum.ReloadModule);
        }
    }
}