/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.Visitors.DataProvider;
using YetaWF.Modules.Visitors.Endpoints;
using YetaWF.Modules.Visitors.Modules;

namespace YetaWF.Modules.Visitors.Controllers {

    public class VisitorsModuleController : ControllerImpl<YetaWF.Modules.Visitors.Modules.VisitorsModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ModuleActionsGrid"), ReadOnly]
            public List<ModuleAction> Commands {
                get {
                    List<ModuleAction> actions = new List<ModuleAction>();

                    VisitorDisplayModule dispMod = new VisitorDisplayModule();
                    actions.New(dispMod.GetAction_Display(Module.DisplayUrl, Key), ModuleAction.ActionLocationEnum.GridLinks);

                    return actions;
                }
            }

            [UIHint("Hidden")]
            public int Key { get; set; }

            [Caption("Session Id"), Description("The session id used to identify the visitor")]
            [UIHint("String"), ReadOnly]
            public string? SessionId { get; set; }

            [Caption("Accessed"), Description("The date and time the visitor visited the site")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime AccessDateTime { get; set; }

            [Caption("User"), Description("The user's email address (if available)")]
            [UIHint("YetaWF_Identity_UserId"), ReadOnly]
            public int UserId { get; set; }

            [Caption("IP Address"), Description("The IP address of the site visitor")]
            [UIHint("IPAddress"), ReadOnly]
            public string? IPAddress { get; set; }

            [Caption("Continent"), Description("The continent where the visitor is located, based on IP address (if available)")]
            [UIHint("String"), ReadOnly]
            public string? ContinentCode { get; set; }
            [Caption("Country"), Description("The country where the visitor is located, based on IP address (if available)")]
            [UIHint("String"), ReadOnly]
            public string? CountryCode { get; set; }
            [Caption("Region"), Description("The region where the visitor is located, based on IP address (if available)")]
            [UIHint("String"), ReadOnly]
            public string? RegionCode { get; set; }
            [Caption("City"), Description("The city where the visitor is located, based on IP address (if available)")]
            [UIHint("String"), ReadOnly]
            public string? City { get; set; }

            [Caption("Url"), Description("The Url accessed by the site visitor")]
            [UIHint("Url"), ReadOnly]
            public string? Url { get; set; }
            [Caption("Referrer"), Description("The Url where the site visitor came from")]
            [UIHint("Url"), ReadOnly]
            public string? Referrer { get; set; }
            [Caption("User Agent"), Description("The web browser's user agent")]
            [UIHint("String"), ReadOnly]
            public string? UserAgent { get; set; }
            [Caption("Error"), Description("Shows any error that may have occurred")]
            [UIHint("String"), ReadOnly]
            public string? Error { get; set; }

            private VisitorsModule Module { get; set; }

            public BrowseItem(VisitorsModule module, VisitorEntry data) {
                Module = module;
                ObjectSupport.CopyData(data, this);
            }
        }

        public class BrowseModel {
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; } = null!;
        }
        internal static GridDefinition GetGridModel(ModuleDefinition module) {
            return new GridDefinition {
                ModuleGuid = module.ModuleGuid,
                SettingsModuleGuid = module.PermanentGuid,
                InitialPageSize = 20,
                RecordType = typeof(BrowseItem),
                AjaxUrl = Utility.UrlFor<VisitorsModuleEndpoints>(GridSupport.BrowseGridData),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                    using (VisitorEntryDataProvider dataProvider = new VisitorEntryDataProvider()) {
                        DataProviderGetRecords<VisitorEntry> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from s in browseItems.Data select new BrowseItem((VisitorsModule)module, s)).ToList<object>(),
                            Total = browseItems.Total
                        };
                    }
                },
            };
        }

        [AllowGet]
        public ActionResult Visitors() {
            using (VisitorEntryDataProvider visitorDP = new VisitorEntryDataProvider()) {
                if (!visitorDP.Usable)
                    throw new Error(this.__ResStr("noInfo", "Visitor information is not available - See https://yetawf.com/Documentation/YetaWF/Visitors"));
            }
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel(Module)
            };
            return View(model);
        }
    }
}