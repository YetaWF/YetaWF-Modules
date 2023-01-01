/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/BootstrapCarousel#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Modules.BootstrapCarousel.Models;
using YetaWF.Core.Models.Attributes;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.BootstrapCarousel.Controllers {

    public class CarouselDisplayModuleController : ControllerImpl<YetaWF.Modules.BootstrapCarousel.Modules.CarouselDisplayModule> {

        public CarouselDisplayModuleController() { }

        public class Model {
            [UIHint("YetaWF_BootstrapCarousel_SlideShow")]
            public CarouselInfo SlideShow { get; set; }

            public Model() {
                SlideShow = new CarouselInfo();
            }
        }

        [AllowGet]
        public ActionResult CarouselDisplay() {
            if (!Manager.EditMode && Module.SlideShow.Slides.Count == 0) return new EmptyResult();
            Model model = new Model {
                SlideShow = Module.SlideShow
            };
            return View(model);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> CarouselDisplay_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            Module.SlideShow = model.SlideShow;
            await Module.SaveAsync();
            model.SlideShow = Module.SlideShow;
            return FormProcessed(model, OnClose: OnCloseEnum.UpdateInPlace, OnPopupClose: OnPopupCloseEnum.UpdateInPlace);
        }
    }
}
