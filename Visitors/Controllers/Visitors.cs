/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Visitors#License */

using System;
using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Menus;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Visitors.DataProvider;
using YetaWF.Modules.Visitors.Modules;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Visitors.Controllers {

    public class VisitorsModuleController : ControllerImpl<YetaWF.Modules.Visitors.Modules.VisitorsModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };

                    VisitorDisplayModule dispMod = new VisitorDisplayModule();
                    actions.New(dispMod.GetAction_Display(Module.DisplayUrl, Key), ModuleAction.ActionLocationEnum.GridLinks);

                    return actions;
                }
            }

            [UIHint("Hidden")]
            public int Key { get; set; }

            [Caption("Session Id"), Description("The session id used to identify the visitor")]
            [UIHint("LongValue"), ReadOnly]
            public long SessionKey { get; set; }

            [Caption("Accessed"), Description("The date and time the visitor visited the site")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime AccessDateTime { get; set; }

            [Caption("User"), Description("The user's email address (if available)")]
            [UIHint("YetaWF_Identity_UserId"), ReadOnly]
            public int UserId { get; set; }

            [Caption("IP Address"), Description("The IP address of the site visitor")]
            [UIHint("IPAddress"), ReadOnly]
            public string IPAddress { get; set; }

            [Caption("Continent"), Description("The continent where the visitor is located, based on IP address (if available)")]
            [UIHint("String"), ReadOnly]
            public string ContinentCode { get; set; }
            [Caption("Country"), Description("The country where the visitor is located, based on IP address (if available)")]
            [UIHint("String"), ReadOnly]
            public string CountryCode { get; set; }
            [Caption("Region"), Description("The region where the visitor is located, based on IP address (if available)")]
            [UIHint("Region"), ReadOnly]
            public string RegionCode { get; set; }
            [Caption("City"), Description("The city where the visitor is located, based on IP address (if available)")]
            [UIHint("String"), ReadOnly]
            public string City { get; set; }

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

            private VisitorsModule Module { get; set; }

            public BrowseItem(VisitorsModule module, VisitorEntry data) {
                Module = module;
                ObjectSupport.CopyData(data, this);
            }
        }

        public class BrowseModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }

        [HttpGet]
        public ActionResult Visitors() {
            using (VisitorEntryDataProvider visitorDP = new VisitorEntryDataProvider()) {
                if (!visitorDP.Usable) return View("VisitorsUnavailable");
            }
            BrowseModel model = new BrowseModel { };
            model.GridDef = new GridDefinition {
                AjaxUrl = GetActionUrl("Visitors_GridData"),
                ModuleGuid = Module.ModuleGuid,
                RecordType = typeof(BrowseItem),
                SettingsModuleGuid = Module.PermanentGuid,
            };
            return View(model);
        }

        [HttpPost]
        [ConditionalAntiForgeryToken]
        public ActionResult Visitors_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, Guid settingsModuleGuid) {
            using (VisitorEntryDataProvider dataProvider = new VisitorEntryDataProvider()) {
                int total;
                List<VisitorEntry> browseItems = dataProvider.GetItems(skip, take, sort, filters, out total);
                GridHelper.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
                return GridPartialView(new DataSourceResult {
                    Data = (from s in browseItems select new BrowseItem(Module, s)).ToList<object>(),
                    Total = total
                });
            }
        }
    }
}