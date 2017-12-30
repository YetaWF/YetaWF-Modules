/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

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
using YetaWF.Modules.Messenger.DataProvider;
using YetaWF.Modules.Messenger.Modules;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Messenger.Controllers {

    public class BrowseSiteAnnouncementModuleController : ControllerImpl<YetaWF.Modules.Messenger.Modules.BrowseSiteAnnouncementModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };
                    return actions;
                }
            }

            [Caption("Date/Time Sent"), Description("The date/time the message was sent to all users")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Sent { get; set; }

            [Caption("Title"), Description("The title of the message sent to all users")]
            [UIHint("String"), ReadOnly]
            public string Title { get; set; }

            [Caption("Message"), Description("The message that was sent to all users")]
            [UIHint("String"), ReadOnly]
            public string Message { get; set; }

            public int Key { get; set; }

            private BrowseSiteAnnouncementModule Module { get; set; }

            public BrowseItem(BrowseSiteAnnouncementModule module, SiteAccouncement data) {
                Module = module;
                ObjectSupport.CopyData(data, this);
            }
        }

        public class BrowseModel {
            [Caption(""), Description("")] // empty entries required so property is shown in property list (but with a suppressed label)
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; }
        }

        [AllowGet]
        public ActionResult BrowseSiteAnnouncement() {
            BrowseModel model = new BrowseModel { };
            model.GridDef = new GridDefinition {
                AjaxUrl = GetActionUrl("BrowseSiteAnnouncement_GridData"),
                ModuleGuid = Module.ModuleGuid,
                RecordType = typeof(BrowseItem),
                SettingsModuleGuid = Module.PermanentGuid,
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult BrowseSiteAnnouncement_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, Guid settingsModuleGuid) {
            using (SiteAccouncementDataProvider dataProvider = new SiteAccouncementDataProvider()) {
                int total;
                List<SiteAccouncement> browseItems = dataProvider.GetItems(skip, take, sort, filters, out total);
                GridHelper.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
                return GridPartialView(new DataSourceResult {
                    Data = (from s in browseItems select new BrowseItem(Module, s)).ToList<object>(),
                    Total = total
                });
            }
        }
    }
}
