/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ActiveUser#License */

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.Messenger.DataProvider;

namespace YetaWF.Modules.Messenger.Controllers {

    public class BrowseActiveUsersModuleController : ControllerImpl<YetaWF.Modules.Messenger.Modules.BrowseActiveUsersModule> {

        public class BrowseItem {

            //[Caption("Actions"), Description("The available actions")]
            //[UIHint("ActionIcons"), ReadOnly]
            //public MenuList Commands {
            //    get {
            //        MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };
            //        return actions;
            //    }
            //}

            [UIHint("Hidden")]
            public int Key { get; set; }

            [Caption("Connection Id"), Description("The connection id used to identify the active user")]
            [UIHint("String"), ReadOnly]
            public string ConnectionId { get; set; } = null!;

            [Caption("Created"), Description("The date and time the visitor connected to the site")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Created { get; set; }

            [Caption("User"), Description("The user's email address (if available)")]
            [UIHint("YetaWF_Identity_UserId"), ReadOnly]
            public int UserId { get; set; }

            [Caption("IP Address"), Description("The IP address of the site visitor")]
            [UIHint("IPAddress"), ReadOnly]
            public string IPAddress { get; set; } = null!;

            public BrowseItem(ActiveUser user) {
                ObjectSupport.CopyData(user, this);
            }
        }

        public class BrowseModel {
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; } = null!;
        }
        private GridDefinition GetGridModel() {
            return new GridDefinition {
                ModuleGuid = Module.ModuleGuid,
                SettingsModuleGuid = Module.PermanentGuid,
                InitialPageSize = 20,
                RecordType = typeof(BrowseItem),
                AjaxUrl = GetActionUrl(nameof(BrowseActiveUsers_GridData)),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                    using (ActiveUsersDataProvider userDP = new ActiveUsersDataProvider()) {
                        DataProviderGetRecords<ActiveUser> browseItems = await userDP.GetItemsAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from s in browseItems.Data select new BrowseItem(s)).ToList<object>(),
                            Total = browseItems.Total
                        };
                    }
                },
            };
        }

        [AllowGet]
        public ActionResult BrowseActiveUsers() {
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel()
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> BrowseActiveUsers_GridData(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync(GetGridModel(), gridPVData);
        }
    }
}