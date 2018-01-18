/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
#if MVC6
using Microsoft.AspNetCore.Mvc;
using YetaWF.Core.Support;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplateTimeZoneModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplateTimeZoneModule> {

        public TemplateTimeZoneModuleController() { }

        [Trim]
        public class Model {

            [Caption("TimeZone (Required)"), Description("TimeZone (Required)")]
            [UIHint("TimeZone"), StringLength(Globals.MaxTimeZone), Required, Trim]
            public string TimeZoneReq { get; set; }

            [Caption("TimeZone (Required w/Select)"), Description("TimeZone (Required, with (select))")]
            [UIHint("TimeZone"), AdditionalMetadata("ShowDefault", false), StringLength(Globals.MaxTimeZone), Required, Trim]
            public string TimeZoneReqSel { get; set; }

            [Caption("TimeZone"), Description("TimeZone (optional)")]
            [UIHint("TimeZone"), AdditionalMetadata("ShowDefault", false), StringLength(Globals.MaxTimeZone), Trim]
            public string TimeZone { get; set; }

            [Caption("TimeZone (Read/Only)"), Description("TimeZone (read/only)")]
            [UIHint("TimeZone"), ReadOnly]
            public string TimeZoneRO { get; set; }

            public Model() {
                TimeZoneRO = TimeZoneInfo.Local.Id;
            }
        }

        [AllowGet]
        public ActionResult TemplateTimeZone() {
            Model model = new Model { };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplateTimeZone_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }
    }
}
