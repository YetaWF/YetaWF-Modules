/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/IFrame#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;

namespace YetaWF.Modules.IFrame.Controllers {

    public class IFrameDisplayModuleController : ControllerImpl<YetaWF.Modules.IFrame.Modules.IFrameDisplayModule> {

        public IFrameDisplayModuleController() { }

        public class DisplayModel {
            public string Style { get; set; }
        }

        [HttpGet]
        public ActionResult IFrameDisplay() {
            if (string.IsNullOrWhiteSpace(Module.Url))
                return new EmptyResult();
            DisplayModel model = new DisplayModel();
            model.Style = "";
            if (!string.IsNullOrWhiteSpace(Module.Width))
                model.Style += "width:" + Module.Width;
            if (!string.IsNullOrWhiteSpace(model.Style))
                model.Style += ";";
            if (!string.IsNullOrWhiteSpace(Module.Height))
                model.Style += "height:" + Module.Height;
            return View(model);
        }
    }
}
