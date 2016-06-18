/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.ImageRepository.Views.Shared;

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

            public void Update(YetaWFManager manager, ModuleDefinition module) {
                ImageName_Info = new ImageSelectionInfo(manager, module, module.ModuleGuid, null) {
                    AllowUpload = true,
                };
                FlashImageName_Info = new FlashSelectionInfo(manager, module, module.ModuleGuid, null) {
                    AllowUpload = true,
                };
            }
        }

        [HttpGet]
        public ActionResult TemplateTest(string imageName) {
            Model model = new Model { };
            model.Update(Manager, Module);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TemplateTest_Partial(Model model) {
            model.Update(Manager, Module);
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model, this.__ResStr("okSaved", "Image saved"), OnClose: OnCloseEnum.Return, OnPopupClose: OnPopupCloseEnum.ReloadModule);
        }
    }
}