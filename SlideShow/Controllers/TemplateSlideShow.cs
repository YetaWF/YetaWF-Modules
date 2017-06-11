/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SlideShow#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.SlideShow.Models;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.SlideShow.Controllers {

    public class TemplateSlideShowModuleController : ControllerImpl<YetaWF.Modules.SlideShow.Modules.TemplateSlideShowModule> {

        // TODO: Captions definitely need some work

        public TemplateSlideShowModuleController() { }

        [Trim]
        public class Model {

            [Caption("Slide Show (R/O)"), Description("SlideShow Test")]
            [UIHint("YetaWF_SlideShow_SlideShow"), ReadOnly]
            public SlideShowInfo SlideShow { get; set; }

            [Caption("Slide Show"), Description("SlideShow Test")]
            [UIHint("YetaWF_SlideShow_SlideShow")]
            public SlideShowInfo SlideShowEdit { get; set; }

            public Model() {
                SlideShow = new SlideShowInfo();
                SlideShowEdit = new SlideShowInfo();
            }
        }

        [AllowGet]
        public ActionResult TemplateSlideShow() {
            Model model = new Model();
            model.SlideShow = model.SlideShowEdit = Module.SlideShow;
            return View(model);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult TemplateSlideShow_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            Module.SlideShow = model.SlideShowEdit;
            Module.Save();
            model.SlideShowEdit = model.SlideShow = Module.SlideShow;
            return FormProcessed(model);
        }
    }
}