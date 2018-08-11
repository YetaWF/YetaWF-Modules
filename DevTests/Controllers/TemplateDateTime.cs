/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

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

            [Category("Date/Time"), Caption("Date/Time (Required)"), Description("Date/Time (Required)")]
            [UIHint("DateTime"), Required]
            public DateTime DateTimeReq { get; set; }

            [Category("Date/Time"), Caption("Date/Time"), Description("Date/Time")]
            [UIHint("DateTime")]
            public DateTime? DateTimeOpt { get; set; }

            [Category("Date/Time"), Caption("Date/Time (Read/Only)"), Description("Date/Time (Read/only)")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime DateTimeRO { get; set; }

            [Category("Date/Time"), Caption("Date (Required)"), Description("Date (Required)")]
            [UIHint("Date"), Required]
            public DateTime DateReq { get; set; }

            [Category("Date/Time"), Caption("Date"), Description("Date/Time")]
            [UIHint("Date")]
            public DateTime? DateOpt { get; set; }

            [Category("Date/Time"), Caption("Date (Read/Only)"), Description("Date/Time (Read/only)")]
            [UIHint("Date"), ReadOnly]
            public DateTime DateRO { get; set; }

            [Category("Date/Time"), Caption("Time (Required)"), Description("Time (Required)")]
            [UIHint("Time"), Required]
            public DateTime TimeReq { get; set; }

            [Category("Date/Time"), Caption("Time"), Description("Time")]
            [UIHint("Time")]
            public DateTime? TimeOpt { get; set; }

            [Category("Date/Time"), Caption("Time (Read/Only)"), Description("Time (Read/only)")]
            [UIHint("Time"), ReadOnly]
            public DateTime TimeRO { get; set; }

            [Category("Date/Time"), Caption("Time Of Day (Required)"), Description("Time Of Day (Required)")]
            [UIHint("TimeOfDay"), Required]
            public TimeOfDay TimeOfDayReq { get; set; }

            [Category("Date/Time"), Caption("Time Of Day (Required)"), Description("Time Of Day")]
            [UIHint("TimeOfDay")]
            public TimeOfDay TimeOfDay { get; set; }

            [Category("Date/Time"), Caption("Time Of Day (Read/Only)"), Description("Time Of Day (Read/Only)")]
            [UIHint("TimeOfDay"), ReadOnly]
            public TimeOfDay TimeOfDayRO { get; set; }

            [Category("TimeSpan"), Caption("Timespan (Required)"), Description("Timespan (Required)")]
            [UIHint("TimeSpan"), Required]
            public TimeSpan TimeSpanReq { get; set; }

            [Category("TimeSpan"), Caption("Timespan"), Description("Timespan")]
            [UIHint("TimeSpan")]
            public TimeSpan TimeSpanOpt { get; set; }

            [Category("TimeSpan"), Caption("Timespan (Read/Only)"), Description("Timespan (Read/only)")]
            [UIHint("TimeSpan"), ReadOnly]
            public TimeSpan TimeSpanRO { get; set; }

            [Category("TimeSpan"), Caption("Timespan (Read/Only)"), Description("Timespan (Read/only)")]
            [UIHint("TimeSpan"), ReadOnly]
            public TimeSpan TimeSpanHMSRO { get; set; }

            [Category("Hours"), Caption("Mondays"), Description("Select the hours")]
            [UIHint("DayTimeRange"), Required]
            public DayTimeRange Mondays { get { return OpeningHours.Days[(int)DayOfWeek.Monday]; } set { OpeningHours.Days[(int)DayOfWeek.Monday] = value; } }
            [Category("Hours"), Caption("Tuesdays"), Description("Select the hours")]
            [UIHint("DayTimeRange"), Required]
            public DayTimeRange Tuesdays { get { return OpeningHours.Days[(int)DayOfWeek.Tuesday]; } set { OpeningHours.Days[(int)DayOfWeek.Tuesday] = value; } }
            [Category("Hours"), Caption("Wednesdays"), Description("Select the hours")]
            [UIHint("DayTimeRange"), Required]
            public DayTimeRange Wednesdays { get { return OpeningHours.Days[(int)DayOfWeek.Wednesday]; } set { OpeningHours.Days[(int)DayOfWeek.Wednesday] = value; } }
            [Category("Hours"), Caption("Thursdays"), Description("Select the hours")]
            [UIHint("DayTimeRange"), Required]
            public DayTimeRange Thursdays { get { return OpeningHours.Days[(int)DayOfWeek.Thursday]; } set { OpeningHours.Days[(int)DayOfWeek.Thursday] = value; } }
            [Category("Hours"), Caption("Fridays"), Description("Select the hours")]
            [UIHint("DayTimeRange"), Required]
            public DayTimeRange Fridays { get { return OpeningHours.Days[(int)DayOfWeek.Friday]; } set { OpeningHours.Days[(int)DayOfWeek.Friday] = value; } }
            [Category("Hours"), Caption("Saturdays"), Description("Select the hours")]
            [UIHint("DayTimeRange"), Required]
            public DayTimeRange Saturdays { get { return OpeningHours.Days[(int)DayOfWeek.Saturday]; } set { OpeningHours.Days[(int)DayOfWeek.Saturday] = value; } }
            [Category("Hours"), Caption("Sundays"), Description("Select the hours")]
            [UIHint("DayTimeRange"), Required]
            public DayTimeRange Sundays { get { return OpeningHours.Days[(int)DayOfWeek.Sunday]; } set { OpeningHours.Days[(int)DayOfWeek.Sunday] = value; } }

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
