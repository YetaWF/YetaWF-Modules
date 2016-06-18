/* Copyright � 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplateTimeZoneModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplateTimeZoneModule> {

        public TemplateTimeZoneModuleController() { }

        [Trim]
        public class Model {

            [Caption("TimeZone (Required)"), Description("TimeZone (Required)")]
            [UIHint("TimeZone"), Required, Trim]
            public string TimeZoneReq { get; set; }

            [Caption("TimeZone (Required w/Select)"), Description("TimeZone (Required, with (select))")]
            [UIHint("TimeZone"), AdditionalMetadata("ShowDefault", false), Required, Trim]
            public string TimeZoneReqSel { get; set; }

            [Caption("TimeZone"), Description("TimeZone (optional)")]
            [UIHint("TimeZone"), AdditionalMetadata("ShowDefault", false), Trim]
            public string TimeZone { get; set; }

            [Caption("TimeZone (Read/Only)"), Description("TimeZone (read/only)")]
            [UIHint("TimeZone"), ReadOnly]
            public string TimeZoneRO { get; set; }

            public Model() {
                TimeZoneRO = TimeZoneInfo.Local.Id;
            }
        }

        [HttpGet]
        public ActionResult TemplateTimeZone() {
            Model model = new Model { };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TemplateTimeZone_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }
    }
}
