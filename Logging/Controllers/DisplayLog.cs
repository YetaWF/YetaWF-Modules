/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Logging#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.LoggingDataProvider.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Logging.Controllers {

    public class DisplayLogModuleController : ControllerImpl<YetaWF.Modules.Logging.Modules.DisplayLogModule> {

        public DisplayLogModuleController() { }

        public class DisplayModel {

            [UIHint("Hidden")]
            public int Key { get; set; }

            [Caption("Date/Time"), Description("The time this record was logged")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime TimeStamp { get; set; }

            [Caption("Session Id"), Description("The session id used to identify the visitor")]
            [UIHint("String"), ReadOnly]
            public string SessionId { get; set; }

            [Caption("Level"), Description("The error level of this log record")]
            [UIHint("Enum"), ReadOnly]
            public Core.Log.Logging.LevelEnum Level { get; set; }

            [Caption("Info"), Description("The information logged in this record")]
            [UIHint("String"), ReadOnly]
            public string Info { get; set; }

            [Caption("Site Id"), Description("The site which logged this record")]
            [UIHint("IntValue"), ReadOnly]
            public int SiteIdentity { get; set; }

            [Caption("IP Address"), Description("The IP address associated with this log entry")]
            [UIHint("IPAddress"), ReadOnly]
            public string IPAddress { get; set; }
            [Caption("Url"), Description("The requested Url")]
            [UIHint("Url"), ReadOnly]
            public string RequestedUrl { get; set; }
            [UIHint("Url"), ReadOnly]
            [Caption("Referrer"), Description("The referring Url associated with this log entry")]
            public string ReferrerUrl { get; set; }

            [Caption("User"), Description("The user's name/email address (if available)")]
            [UIHint("YetaWF_Identity_UserId"), ReadOnly]
            public int UserId { get; set; }

            [Caption("Module Name"), Description("The module logging this record")]
            [UIHint("String"), ReadOnly]
            public string ModuleName { get; set; }
            [Caption("Class"), Description("The class logging this record")]
            [UIHint("String"), ReadOnly]
            public string Class { get; set; }
            [Caption("Method"), Description("The method logging this record")]
            [UIHint("String"), ReadOnly]
            public string Method { get; set; }
            [Caption("Namespace"), Description("The namespace logging this record")]
            [UIHint("String"), ReadOnly]
            public string Namespace { get; set; }

            public void SetData(LogRecord data) {
                ObjectSupport.CopyData(data, this);
            }
        }

        [AllowGet]
        public async Task<ActionResult> DisplayLog(int key) {
            using (LogRecordDataProvider dataProvider = LogRecordDataProvider.GetLogRecordDataProvider()) {
                LogRecord data = await dataProvider.GetItemAsync(key);
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "Record \"{0}\" not found."), key);
                DisplayModel model = new DisplayModel();
                model.SetData(data);
                return View(model);
            }
        }
    }
}