/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Modules.Visitors.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Visitors.Controllers {

    public class TinyVisitorsModuleController : ControllerImpl<YetaWF.Modules.Visitors.Modules.TinyVisitorsModule> {

        public TinyVisitorsModuleController() { }

        public class DisplayModel {

            public int TodaysAnonymous { get; set; }
            public int TodaysUsers { get; set; }
            public int YesterdaysAnonymous { get; set; }
            public int YesterdaysUsers { get; set; }

            public string VisitorsUrl { get; set; }
            public string ImageUrl { get; set; }
            public string Tooltip { get; set; }

            public void SetData(VisitorEntry data) {
                ObjectSupport.CopyData(data, this);
            }
        }

        [AllowGet]
        public async Task<ActionResult> TinyVisitors() {
            using (VisitorEntryDataProvider visitorDP = new VisitorEntryDataProvider()) {
                if (visitorDP.Usable) {
                    string addonUrl = VersionManager.GetAddOnPackageUrl(AreaRegistration.CurrentPackage.AreaName);
                    VisitorEntryDataProvider.Info info = await visitorDP.GetStatsAsync();
                    DisplayModel model = new DisplayModel { };
                    model.TodaysAnonymous = info.TodaysAnonymous;
                    model.TodaysUsers = info.TodaysUsers;
                    model.YesterdaysAnonymous = info.YesterdaysAnonymous;
                    model.YesterdaysUsers = info.YesterdaysUsers;
                    model.Tooltip = this.__ResStr("tooltip", "Today: {0} Users, {1} Anonymous - Yesterday: {2}/{3}", model.TodaysUsers, model.TodaysAnonymous, model.YesterdaysUsers, model.YesterdaysAnonymous);
                    model.ImageUrl = addonUrl + "Icons/People.png";
                    model.VisitorsUrl = Module.VisitorsUrl;
                    return View(model);
                }
                return new EmptyResult();
            }
        }
    }
}