/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Scheduler;
using YetaWF.Core.Support;
using YetaWF.Modules.Scheduler.DataProvider;

namespace YetaWF.Modules.Scheduler.Controllers {

    public class SchedulerDisplayModuleController : ControllerImpl<YetaWF.Modules.Scheduler.Modules.SchedulerDisplayModule> {

        public SchedulerDisplayModuleController() { }

        [Trim]
        public class SchedulerDisplayModel {

            [Caption("Name"), Description("The name of this scheduler item - the name is used to identify a scheduler item")]
            [UIHint("String"), StringLength(SchedulerItemData.MaxName), ReadOnly]
            public string Name { get; set; }

            [Caption("Description"), Description("The description of this scheduler item")]
            [UIHint("TextArea"), AdditionalMetadata("SourceOnly", true), StringLength(SchedulerItemData.MaxDescription), ReadOnly]
            [AllowHtml]
            public string Description { get; set; }

            [Caption("Enabled"), Description("Defines whether the scheduler item is enabled")]
            [UIHint("Boolean"), ReadOnly]
            public bool Enabled { get; set; }

            [Caption("Enable On Startup"), Description("Defines whether the scheduler item is enabled every time the website is restarted")]
            [UIHint("Boolean"), ReadOnly]
            public bool EnableOnStartup { get; set; }

            [Caption("Run Once"), Description("Defines whether the scheduler item is run just once - once it completes, it is disabled")]
            [UIHint("Boolean"), ReadOnly]
            public bool RunOnce { get; set; }

            [Caption("Startup"), Description("Defines whether the scheduler item runs at website startup")]
            [UIHint("Boolean"), ReadOnly]
            public bool Startup { get; set; }

            [Caption("Site Specific"), Description("Defines whether the scheduler item runs for each site")]
            [UIHint("Boolean"), ReadOnly]
            public bool SiteSpecific { get; set; }

            [Caption("Interval"), Description("The scheduler item's frequency")]
            [UIHint("YetaWF_Scheduler_Frequency"), ReadOnly]
            public SchedulerFrequency Frequency { get; set; }

            [Caption("Last"), Description("The last time this item ran")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Last { get; set; }

            [Caption("Next"), Description("The time this item is scheduled to run next")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Next { get; set; }

            [Caption("RunTime"), Description("The duration of the last occurrence of this scheduler item (hh:mm:ss)")]
            [UIHint("YetaWF_Scheduler_TimeSpan"), ReadOnly]
            public TimeSpan RunTime { get; set; }

            [Caption("Errors"), Description("The errors that occurred during the scheduler item's last run")]
            [UIHint("TextArea"), AdditionalMetadata("SourceOnly", true), ReadOnly]
            public string Errors { get; set; }

            [Caption("Event"), Description("The event running at the scheduled time")]
            [UIHint("YetaWF_Scheduler_Event"), ReadOnly]
            public SchedulerEvent Event { get; set; }

            public void SetEvent(SchedulerItemData evnt)
            {
                ObjectSupport.CopyData(evnt, this);
            }
        }

        [HttpGet]
        public ActionResult SchedulerDisplay(string eventName) {
            using (SchedulerDataProvider dataProvider = new SchedulerDataProvider()) {
                SchedulerDisplayModel model = new SchedulerDisplayModel { };
                SchedulerItemData evnt = dataProvider.GetItem(eventName);
                if (evnt == null)
                    throw new Error(this.__ResStr("notFound", "Scheduler item \"{0}\" not found."), eventName);
                model.SetEvent(evnt);
                Module.Title = this.__ResStr("modTitle", "Scheduler Item \"{0}\"", eventName);
                return View(model);
            }
        }
    }
}