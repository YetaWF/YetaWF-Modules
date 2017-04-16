/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/BootstrapCarousel#License */

using System;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.BootstrapCarousel.Models;
using YetaWF.Modules.BootstrapCarousel.Modules;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.BootstrapCarousel.Controllers {

    public class CarouselEditModuleController : ControllerImpl<YetaWF.Modules.BootstrapCarousel.Modules.CarouselEditModule> {

        public CarouselEditModuleController() { }

        [Trim]
        public class EditModel {

            [Caption("Carousel"), Description("Defines the settings for the Bootstrap Carousel")]
            [UIHint("YetaWF_BootstrapCarousel_SlideShow")]
            public CarouselInfo SlideShow { get; set; }

            public CarouselInfo SlideShowPreview { get; set; }

            public Guid CarouselGuid { get; set; }

            public EditModel() { }
        }

        [HttpGet]
        public ActionResult CarouselEdit(Guid carousel) {
            CarouselDisplayModule carouselDispMod = (CarouselDisplayModule)ModuleDefinition.Load(carousel, AllowNone: true);
            if (carouselDispMod == null)
                throw new Error(this.__ResStr("notFound", "Bootstrap Carousel module \"{0}\" not found"), carousel.ToString());
            EditModel model = new EditModel {
                SlideShow = carouselDispMod.SlideShow,
                SlideShowPreview = carouselDispMod.SlideShow,
                CarouselGuid = carousel
            };
            return View(model);
        }

        [HttpPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult CarouselEdit_Partial(EditModel model) {
            CarouselDisplayModule carouselDispMod = (CarouselDisplayModule)ModuleDefinition.Load(model.CarouselGuid, AllowNone: true);
            if (carouselDispMod == null)
                throw new Error(this.__ResStr("alreadyDeleted", "Bootstrap Carousel module \"{0}\" has been removed and can no longer be updated"), model.CarouselGuid.ToString());

            model.SlideShowPreview = carouselDispMod.SlideShow;

            if (!ModelState.IsValid)
                return PartialView(model);

            carouselDispMod.SlideShow = model.SlideShow;
            carouselDispMod.Save();
            model.SlideShowPreview = carouselDispMod.SlideShow;

            return FormProcessed(model, this.__ResStr("okSaved", "Bootstrap Carousel saved"), OnPopupClose: OnPopupCloseEnum.ReloadParentPage);
        }
    }
}
