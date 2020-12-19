/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using Microsoft.AspNetCore.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Support;
using YetaWF.Modules.Visitors.DataProvider;

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
                TrackClickUrl = Utility.UrlFor(GetType(), nameof(TrackClick))
            };
            return View(model);
        }

        [AllowPost]
        [ValidateAntiForgeryToken] // always force antiforgery to avoid cross-site attacks exploiting flooding click tracking
        public ActionResult TrackClick(string url) {
            VisitorEntryDataProvider.AddVisitEntryUrlAsync(url, true); // no await, as in fire and forget
            return new EmptyResult();
        }
    }
}
