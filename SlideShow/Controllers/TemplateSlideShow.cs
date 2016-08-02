/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/SlideShow#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.SlideShow.Models;

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

        [HttpGet]
        public ActionResult TemplateSlideShow() {
            Model model = new Model();
            model.SlideShow = model.SlideShowEdit = Module.SlideShow;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult TemplateSlideShow_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            Module.SlideShow = model.SlideShowEdit;
            Module.Save();
            model.SlideShow = model.SlideShowEdit = Module.SlideShow;
            return FormProcessed(model);
        }
    }
}