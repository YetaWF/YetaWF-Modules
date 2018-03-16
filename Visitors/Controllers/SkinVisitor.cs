/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Support;
using YetaWF.Modules.Visitors.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Visitors.Controllers {

    public class SkinVisitorModuleController : ControllerImpl<YetaWF.Modules.Visitors.Modules.SkinVisitorModule> {

        public SkinVisitorModuleController() { }

        public class DisplayModel {
            public string TrackClickUrl { get; set; }
        }

        [AllowGet]
        public ActionResult SkinVisitor() {
            Module.ShowTitle = Manager.EditMode;// always show title in edit mode and never show in display mode
            // We render a form so we get antiforgery fields used for TrackClick
            DisplayModel model = new DisplayModel {
                TrackClickUrl = YetaWFManager.UrlFor(GetType(), nameof(TrackClick))
            };
            return View(model);
        }

        // Don't use ConditionalAntiForgeryToken here - We have to prevent malicious cross-site attacks as this could be
        // exploited to flood click tracking and fill up DBs, etc.
        // This means you cannot use tracking on STATIC pages.
        [AllowPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrackClick(string url) {
            string origin = Manager.CurrentRequest.Headers["Origin"];
            if (!string.IsNullOrWhiteSpace(origin))
                return new EmptyResult();
            VisitorEntryDataProvider.AddVisitEntryUrlAsync(url, true); // no await, as in fire and forget
            return new EmptyResult();
        }
    }
}
