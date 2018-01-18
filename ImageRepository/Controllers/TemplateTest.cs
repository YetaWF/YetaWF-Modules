/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.ImageRepository.Views.Shared;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.ImageRepository.Controllers {

    public class TemplateTestModuleController : ControllerImpl<YetaWF.Modules.ImageRepository.Modules.TemplateTestModule> {

        public TemplateTestModuleController() { }

        [Trim]
        public class Model {

            [Caption("Image Selection"), Description("Description of image selection")]
            [UIHint("YetaWF_ImageRepository_ImageSelection"), Required, Trim]
            public string ImageName { get; set; }

            [Caption("Flash Image Selection"), Description("Description of Flash image selection")]
            [UIHint("YetaWF_ImageRepository_FlashSelection"), Required, Trim]
            public string FlashImageName { get; set; }

            public ImageSelectionInfo ImageName_Info { get; set; }
            public FlashSelectionInfo FlashImageName_Info { get; set; }

            public Model() { }

            public void Update(ModuleDefinition module) {
                ImageName_Info = new ImageSelectionInfo(module, module.ModuleGuid, null) {
                    AllowUpload = true,
                };
                FlashImageName_Info = new FlashSelectionInfo(module, module.ModuleGuid, null) {
                    AllowUpload = true,
                };
            }
        }

        [AllowGet]
        public ActionResult TemplateTest(string imageName) {
            Model model = new Model { };
            model.Update(Module);
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplateTest_Partial(Model model) {
            model.Update(Module);
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model, this.__ResStr("okSaved", "Image saved"), OnClose: OnCloseEnum.Return, OnPopupClose: OnPopupCloseEnum.ReloadModule);
        }
    }
}