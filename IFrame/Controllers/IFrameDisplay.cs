/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IFrame#License */

using YetaWF.Core.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace YetaWF.Modules.IFrame.Controllers {

    public class IFrameDisplayModuleController : ControllerImpl<YetaWF.Modules.IFrame.Modules.IFrameDisplayModule> {

        public IFrameDisplayModuleController() { }

        public class DisplayModel {
            public string Style { get; set; } = null!;
        }

        [AllowGet]
        public ActionResult IFrameDisplay() {
            if (string.IsNullOrWhiteSpace(Module.Url))
                return new EmptyResult();
            DisplayModel model = new DisplayModel() {
                Style = string.Empty
            };
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
