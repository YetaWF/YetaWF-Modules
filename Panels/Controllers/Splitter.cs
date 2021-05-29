/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using Microsoft.AspNetCore.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.Panels.Models;

namespace YetaWF.Modules.Panels.Controllers {

    public class SplitterModuleController : ControllerImpl<YetaWF.Modules.Panels.Modules.SplitterModule> {

        public SplitterModuleController() { }

        [Trim]
        public class Model {
            public Model() { }
            [UIHint("YetaWF_Panels_SplitterInfo")]
            public SplitterInfo SplitterInfo { get; set; } = null!;
        }

        [AllowGet]
        public ActionResult Splitter() {
            Model model = new Model {
                SplitterInfo = new SplitterInfo {
                    TitleText = Module.TitleText,
                    TitleTooltip = Module.TitleTooltip,
                    CollapseText = Module.CollapseText,
                    CollapseToolTip = Module.CollapseToolTip,
                    ExpandToolTip = Module.ExpandToolTip,

                    Height = Module.Height,
                    ModuleLeft = Module.ModuleLeft,
                    MinWidth = Module.MinWidth,
                    Width = Module.Width,
                    ModuleRight = Module.ModuleRight,
                }
            };
            return View(model);
        }
    }
}
