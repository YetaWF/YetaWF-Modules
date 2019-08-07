/* Copyright � 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Components;
using YetaWF.Core.Serializers;
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

            public enum ControlStatusEnum { Normal, Disabled, }

            [Category("Date/Time"), Caption("Date/Time (Required)"), Description("Date/Time (Required)")]
            [UIHint("DateTime")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public DateTime DateTimeReq { get; set; }

            [Category("Date/Time"), Caption("Date/Time"), Description("Date/Time")]
            [UIHint("DateTime")]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public DateTime? DateTimeOpt { get; set; }

            [Category("Date/Time"), Caption("Date/Time (Read/Only)"), Description("Date/Time (Read/only)")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime DateTimeRO { get; set; }

            [Category("Date/Time"), Caption("Date (Required)"), Description("Date (Required)")]
            [UIHint("Date")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public DateTime DateReq { get; set; }

            [Category("Date/Time"), Caption("Date"), Description("Date/Time")]
            [UIHint("Date")]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public DateTime? DateOpt { get; set; }

            [Category("Date/Time"), Caption("Date (Read/Only)"), Description("Date/Time (Read/only)")]
            [UIHint("Date"), ReadOnly]
            public DateTime DateRO { get; set; }

            [Category("Date/Time"), Caption("Time (Required)"), Description("Time (Required)")]
            [UIHint("Time")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public DateTime TimeReq { get; set; }

            [Category("Date/Time"), Caption("Time"), Description("Time")]
            [UIHint("Time")]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public DateTime? TimeOpt { get; set; }

            [Category("Date/Time"), Caption("Time (Read/Only)"), Description("Time (Read/only)")]
            [UIHint("Time"), ReadOnly]
            public DateTime TimeRO { get; set; }

            [Category("Date/Time"), Caption("Time Of Day (Required)"), Description("Time Of Day (Required)")]
            [UIHint("TimeOfDay")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public TimeOfDay TimeOfDayReq { get; set; }

            [Category("Date/Time"), Caption("Time Of Day (Required)"), Description("Time Of Day")]
            [UIHint("TimeOfDay")]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public TimeOfDay TimeOfDay { get; set; }

            [Category("Date/Time"), Caption("Time Of Day (Read/Only)"), Description("Time Of Day (Read/Only)")]
            [UIHint("TimeOfDay"), ReadOnly]
            public TimeOfDay TimeOfDayRO { get; set; }

            [Category("Date/Time"), Caption("Control Status"), Description("Defines the processing status of the controls")]
            [UIHint("Enum")]
            public ControlStatusEnum ControlStatus { get; set; }


            [Category("TimeSpan"), Caption("Timespan (Required)"), Description("Timespan (Required)")]
            [UIHint("TimeSpan")]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public TimeSpan TimeSpanReq { get; set; }

            [Category("TimeSpan"), Caption("Timespan"), Description("Timespan")]
            [UIHint("TimeSpan")]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public TimeSpan TimeSpanOpt { get; set; }

            [Category("TimeSpan"), Caption("Timespan (Read/Only)"), Description("Timespan (Read/only)")]
            [UIHint("TimeSpan"), ReadOnly]
            public TimeSpan TimeSpanRO { get; set; }

            [Category("TimeSpan"), Caption("Timespan (Read/Only)"), Description("Timespan (Read/only)")]
            [UIHint("TimeSpan"), ReadOnly]
            public TimeSpan TimeSpanHMSRO { get; set; }

            [TextAbove("Weekly Hours")]
            [Category("Hours"), Caption(""), Description("")]
            [UIHint("WeeklyHours")]
            public WeeklyHours OpeningHours { get; set; }

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
                TimeOfDayReq = new TimeOfDay(9, 0, 0);
                TimeOfDay = new TimeOfDay(9, 0, 0);
                TimeOfDayRO = new TimeOfDay(9, 0, 0);
                TimeSpanRO = new TimeSpan(3, 13, 25, 11, 933);
                TimeSpanHMSRO = new TimeSpan(0, 13, 25, 11, 933);
                OpeningHours = new WeeklyHours();
            }
        }

        [AllowGet]
        public ActionResult TemplateDateTime() {
            Model model = new Model { };
            model.OpeningHours = WeeklyHours.WorkWeek;
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
