/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.Visitors.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Visitors.Controllers {

    public class VisitorSummaryModuleController : ControllerImpl<YetaWF.Modules.Visitors.Modules.VisitorSummaryModule> {

        public VisitorSummaryModuleController() { }

        public class DisplayModel {

            [Caption("Today's Anonymous Visitors"), Description("Displays the number of anonymous visitors today (by reporting the number of distinct sessions without a logged on user)")]
            [UIHint("IntValue"), ReadOnly]
            public int TodaysAnonymous { get; set; }
            [Caption("Today's Visitors"), Description("Displays the number of logged on visitors today")]
            [UIHint("IntValue"), ReadOnly]
            public int TodaysUsers { get; set; }
            [Caption("Yesterday's Anonymous Visitors"), Description("Displays the number of anonymous visitors yesterday (by reporting the number of distinct sessions without a logged on user)")]
            [UIHint("IntValue"), ReadOnly]
            public int YesterdaysAnonymous { get; set; }
            [Caption("Yesterday's Visitors"), Description("Displays the number of logged on visitors yesterday")]
            [UIHint("IntValue"), ReadOnly]
            public int YesterdaysUsers { get; set; }
        }

        [AllowGet]
        public async Task<ActionResult> VisitorSummary() {
            using (VisitorEntryDataProvider visitorDP = new VisitorEntryDataProvider()) {
                if (visitorDP.Usable) {

                    await Manager.AddOnManager.AddAddOnNamedAsync(AreaRegistration.CurrentPackage.AreaName, Module.ModuleName); // add module specific items

                    VisitorEntryDataProvider.Info info = await visitorDP.GetStatsAsync();
                    DisplayModel model = new DisplayModel { };
                    model.TodaysAnonymous = info.TodaysAnonymous;
                    model.TodaysUsers = info.TodaysUsers;
                    model.YesterdaysAnonymous = info.YesterdaysAnonymous;
                    model.YesterdaysUsers = info.YesterdaysUsers;
                    return View(model);
                }
                return new EmptyResult();
            }
        }
    }
}
