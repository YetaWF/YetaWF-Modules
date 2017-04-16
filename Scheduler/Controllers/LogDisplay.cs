/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Scheduler.DataProvider;
using System;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Scheduler.Controllers {

    public class LogDisplayModuleController : ControllerImpl<YetaWF.Modules.Scheduler.Modules.LogDisplayModule> {

        public LogDisplayModuleController() { }

        public class DisplayModel {

            [Caption("Created"), Description("The date/time this log record was created")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime TimeStamp { get; set; }

            [Caption("Id"), Description("The id of the scheduler item run")]
            [UIHint("LongValue"), ReadOnly]
            public long RunId { get; set; }

            [Caption("Name"), Description("The name of the running scheduler item")]
            [UIHint("String"), ReadOnly]
            public string Name { get; set; }

            [Caption("Level"), Description("The message level")]
            [UIHint("Enum"), ReadOnly]
            public Core.Log.Logging.LevelEnum Level { get; set; }

            [Caption("Site Id"), Description("The site which was affected by the scheduler item")]
            [UIHint("SiteId"), ReadOnly]
            public int SiteIdentity { get; set; }

            [Caption("Message"), Description("The message")]
            [UIHint("String"), ReadOnly]
            public string Info { get; set; }

            public void SetData(LogData data) {
                ObjectSupport.CopyData(data, this);
            }
        }

        [HttpGet]
        public ActionResult LogDisplay(int logEntry) {
            using (LogDataProvider logDP = new LogDataProvider()) {
                LogData data = logDP.GetItem(logEntry);
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "Scheduler log entry with id {0} not found"), logEntry);
                DisplayModel model = new DisplayModel();
                model.SetData(data);
                return View(model);
            }
        }
    }
}
