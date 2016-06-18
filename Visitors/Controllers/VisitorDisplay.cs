/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Visitors#License */

using System;
using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Visitors.DataProvider;

namespace YetaWF.Modules.Visitors.Controllers {

    public class VisitorDisplayModuleController : ControllerImpl<YetaWF.Modules.Visitors.Modules.VisitorDisplayModule> {

        public VisitorDisplayModuleController() { }

        public class DisplayModel {

            [UIHint("Hidden")]
            public int Key { get; set; }

            [Caption("Session Id"), Description("The session id used to identify the visitor")]
            [UIHint("LongValue"), ReadOnly]
            public long SessionKey { get; set; }

            [Caption("Accessed"), Description("The date and time the visitor visited the site")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime AccessDateTime { get; set; }

            [Caption("User Id"), Description("The user's email address (if available)")]
            [UIHint("YetaWF_Identity_UserId"), ReadOnly]
            public int UserId { get; set; }

            [Caption("IP Address"), Description("The IP address of the site visitor")]
            [UIHint("IPAddress"), ReadOnly]
            public string IPAddress { get; set; }
            [Caption("Url"), Description("The Url accessed by the site visitor")]
            [UIHint("Url"), ReadOnly]
            public string Url { get; set; }
            [Caption("Referrer"), Description("The Url where the site visitor came from")]
            [UIHint("Url"), ReadOnly]
            public string Referrer { get; set; }
            [Caption("User Agent"), Description("The web browser's user agent")]
            [UIHint("String"), ReadOnly]
            public string UserAgent { get; set; }
            [Caption("Error"), Description("Shows any error that may have occurred")]
            [UIHint("String"), ReadOnly]
            public string Error { get; set; }

            public void SetData(VisitorEntry data) {
                ObjectSupport.CopyData(data, this);
            }
        }

        [HttpGet]
        public ActionResult VisitorDisplay(int key) {
            using (VisitorEntryDataProvider visitorDP = new VisitorEntryDataProvider()) {
                VisitorEntry data = visitorDP.GetItem(key);
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "Visitor Entry {0} not found."), key);
                DisplayModel model = new DisplayModel();
                model.SetData(data);
                Module.Title = this.__ResStr("modTitle", "Visitor Entry {0}", key);
                return View(model);
            }
        }
    }
}