/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.Messenger.DataProvider;
using YetaWF.Modules.Messenger.Endpoints;
using YetaWF.Modules.Messenger.Modules;
using YetaWF.Modules.Messenger.Views;

namespace YetaWF.Modules.Messenger.Controllers {

    public class BrowseSiteAnnouncementModuleController : ControllerImpl<YetaWF.Modules.Messenger.Modules.BrowseSiteAnnouncementModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ModuleActionsGrid"), ReadOnly]
            public List<ModuleAction> Commands {
                get {
                    List<ModuleAction> actions = new List<ModuleAction>();
                    return actions;
                }
            }

            [Caption("Date/Time Sent"), Description("The date/time the message was sent to all users")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Sent { get; set; }

            [Caption("Title"), Description("The title of the message sent to all users")]
            [UIHint("String"), ReadOnly]
            public string? Title { get; set; }

            [Caption("Message"), Description("The message that was sent to all users")]
            [UIHint("String"), ReadOnly]
            public string? Message { get; set; }

            public int Key { get; set; }

            private BrowseSiteAnnouncementModule Module { get; set; }

            public BrowseItem(BrowseSiteAnnouncementModule module, SiteAnnouncement data) {
                Module = module;
                ObjectSupport.CopyData(data, this);
            }
        }

        public class BrowseModel {
            [Caption(""), Description("")] // empty entries required so property is shown in property list (but with a suppressed label)
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; } = null!;
        }
        internal static GridDefinition GetGridModel(ModuleDefinition module) {
            return new GridDefinition {
                ModuleGuid = module.ModuleGuid,
                SettingsModuleGuid = module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = Utility.UrlFor<BrowseSiteAnnouncementModuleEndpoints>(GridSupport.BrowseGridData),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                    using (SiteAnnouncementDataProvider dataProvider = new SiteAnnouncementDataProvider()) {
                        DataProviderGetRecords<SiteAnnouncement> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from s in browseItems.Data select new BrowseItem((BrowseSiteAnnouncementModule)module, s)).ToList<object>(),
                            Total = browseItems.Total
                        };
                    }
                },
            };
        }

        [AllowGet]
        public ActionResult BrowseSiteAnnouncement() {
            using (SiteAnnouncementDataProvider sitAnncDP = new SiteAnnouncementDataProvider()) {
                if (!sitAnncDP.Usable)
                    return View(SiteAnnouncementsUnavailableView.ViewName);
            }
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel(Module)
            };
            return View(model);
        }
    }
}
