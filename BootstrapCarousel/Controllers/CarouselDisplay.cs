/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/BootstrapCarousel#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Modules.BootstrapCarousel.Models;

namespace YetaWF.Modules.BootstrapCarousel.Controllers {

    public class CarouselDisplayModuleController : ControllerImpl<YetaWF.Modules.BootstrapCarousel.Modules.CarouselDisplayModule> {

        public CarouselDisplayModuleController() { }

        public class DisplayModel {
            public CarouselInfo SlideShow { get; set; }
        }

        [HttpGet]
        public ActionResult CarouselDisplay() {
            if (Module.SlideShow.Slides.Count == 0) return new EmptyResult();
            DisplayModel model = new DisplayModel {
                SlideShow = Module.SlideShow
            };
            return View(model);
        }
    }
}
