/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Modules.Visitors.DataProvider;
using YetaWF.Modules.Visitors.Modules;
using YetaWF.Modules.Visitors.Scheduler;
using YetaWF.Core.Components;
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
            [UIHint("String"), ReadOnly]
            public string SessionId { get; set; }

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
            [UIHint("String"), ReadOnly]
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
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; }
        }
        private GridDefinition GetGridModel() {
            return new GridDefinition {
                ModuleGuid = Module.ModuleGuid,
                SettingsModuleGuid = Module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = GetActionUrl(nameof(Visitors_GridData)),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                    using (VisitorEntryDataProvider dataProvider = new VisitorEntryDataProvider()) {
                        DataProviderGetRecords<VisitorEntry> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from s in browseItems.Data select new BrowseItem(Module, s)).ToList<object>(),
                            Total = browseItems.Total
                        };
                    }
                },
            };
        }

        [AllowGet]
        public ActionResult Visitors() {
            using (VisitorEntryDataProvider visitorDP = new VisitorEntryDataProvider()) {
                if (!visitorDP.Usable) return View("VisitorsUnavailable");
            }
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel()
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> Visitors_GridData(string fieldPrefix, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) {
            return await GridPartialViewAsync(GetGridModel(), fieldPrefix, skip, take, sorts, filters);
        }

        [AllowPost]
        [Permission("UpdateGeoLocation")]
        [ExcludeDemoMode]
        public async Task<ActionResult> UpdateGeoLocation() {
            AddVisitorGeoLocation geo = new AddVisitorGeoLocation();
            List<string> errorList = new List<string>();
            await geo.AddGeoLocationAsync(errorList);
            return Reload(null, Reload: ReloadEnum.ModuleParts);
        }
    }
}