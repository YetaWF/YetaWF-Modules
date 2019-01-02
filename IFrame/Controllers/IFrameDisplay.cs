/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IFrame#License */

using YetaWF.Core.Controllers;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.IFrame.Controllers {

    public class IFrameDisplayModuleController : ControllerImpl<YetaWF.Modules.IFrame.Modules.IFrameDisplayModule> {

        public IFrameDisplayModuleController() { }

        public class DisplayModel {
            public string Style { get; set; }
        }

        [AllowGet]
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
