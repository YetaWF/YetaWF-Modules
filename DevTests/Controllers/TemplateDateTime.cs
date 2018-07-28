/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplateDateTimeModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplateDateTimeModule> {

        public TemplateDateTimeModuleController() { }

        [Trim]
        public class Model {

            [Caption("Date/Time (Required)"), Description("Date/Time (Required)")]
            [UIHint("DateTime"), Required]
            public DateTime DateTimeReq { get; set; }

            [Caption("Date/Time"), Description("Date/Time")]
            [UIHint("DateTime")]
            public DateTime? DateTimeOpt { get; set; }

            [Caption("Date/Time (Read/Only)"), Description("Date/Time (Read/only)")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime DateTimeRO { get; set; }

            [Caption("Date (Required)"), Description("Date (Required)")]
            [UIHint("Date"), Required]
            public DateTime DateReq { get; set; }

            [Caption("Date"), Description("Date/Time")]
            [UIHint("Date")]
            public DateTime? DateOpt { get; set; }

            [Caption("Date (Read/Only)"), Description("Date/Time (Read/only)")]
            [UIHint("Date"), ReadOnly]
            public DateTime DateRO { get; set; }

            [Caption("Time (Required)"), Description("Time (Required)")]
            [UIHint("Time"), Required]
            public DateTime TimeReq { get; set; }

            [Caption("Time"), Description("Time")]
            [UIHint("Time")]
            public DateTime? TimeOpt { get; set; }

            [Caption("Time (Read/Only)"), Description("Time (Read/only)")]
            [UIHint("Time"), ReadOnly]
            public DateTime TimeRO { get; set; }

            [Caption("Timespan (Required)"), Description("Timespan (Required)")]
            [UIHint("TimeSpan"), Required]
            public TimeSpan TimeSpanReq { get; set; }

            [Caption("Timespan"), Description("Timespan")]
            [UIHint("TimeSpan")]
            public TimeSpan TimeSpanOpt { get; set; }

            [Caption("Timespan (Read/Only)"), Description("Timespan (Read/only)")]
            [UIHint("TimeSpan"), ReadOnly]
            public TimeSpan TimeSpanRO { get; set; }

            [Caption("Timespan (Read/Only)"), Description("Timespan (Read/only)")]
            [UIHint("TimeSpan"), ReadOnly]
            public TimeSpan TimeSpanHMSRO { get; set; }

            public Model() {
                DateTimeReq = DateTime.UtcNow;
                DateTimeOpt = DateTime.UtcNow;
                DateTimeRO = DateTime.UtcNow;
                DateReq = DateTime.UtcNow;
                DateOpt = DateTime.UtcNow;
                DateRO = DateTime.UtcNow;
                TimeReq = DateTime.UtcNow;
                TimeOpt = DateTime.UtcNow;
                TimeRO = DateTime.UtcNow;
                TimeSpanRO = new TimeSpan(3, 13, 25, 11, 933);
                TimeSpanHMSRO = new TimeSpan(0, 13, 25, 11, 933);
            }
        }

        [AllowGet]
        public ActionResult TemplateDateTime() {
            Model model = new Model { };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplateDateTime_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }
    }
}
