/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using Microsoft.AspNetCore.Mvc;
using Softelvdm.Modules.IVR.DataProvider;
using Softelvdm.Modules.IVR.Endpoints;
using Softelvdm.Modules.IVR.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Extensions;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;

namespace Softelvdm.Modules.IVR.Controllers {

    public class BrowseBlockedNumbersModuleController : ControllerImpl<Softelvdm.Modules.IVR.Modules.BrowseBlockedNumbersModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ModuleActionsGrid"), ReadOnly]
            public List<ModuleAction> Commands {
                get {
                    List<ModuleAction> actions = new List<ModuleAction>();

                    EditBlockedNumberModule editMod = new EditBlockedNumberModule();
                    actions.New(editMod.GetAction_Edit(Module.EditUrl, Number), ModuleAction.ActionLocationEnum.GridLinks);
                    actions.New(Module.GetAction_Remove(Id), ModuleAction.ActionLocationEnum.GridLinks);
                    return actions;
                }
            }

            [Caption("Blocked Number"), Description("Shows the blocked phone number")]
            [UIHint("PhoneNumber"), StringLength(Globals.MaxPhoneNumber), ReadOnly]
            public string Number { get; set; } = null!;

            [Caption("Description"), Description("The description of the blocked number")]
            [UIHint("String"), StringLength(BlockedNumberEntry.MaxDescription), ReadOnly]
            public string? Description { get; set; }

            [Caption("Added"), Description("The date/time the entry was added")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Created { get; set; }
            [Caption("Updated"), Description("The date/time the entry was last updated")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime? Updated { get; set; }

            public int Id { get; set; }

            private BrowseBlockedNumbersModule Module { get; set; }

            public BrowseItem(BrowseBlockedNumbersModule module, BlockedNumberEntry data) {
                Module = module;
                ObjectSupport.CopyData(data, this);
                data.Description = data.Description?.Truncate(200);
            }
        }
        internal static GridDefinition GetGridModel(ModuleDefinition module) {
            return new GridDefinition {
                ModuleGuid = module.ModuleGuid,
                SettingsModuleGuid = module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = Utility.UrlFor<BrowseBlockedNumbersModuleEndpoints>(GridSupport.BrowseGridData),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                    using (BlockedNumberDataProvider dataProvider = new BlockedNumberDataProvider()) {
                        DataProviderGetRecords<BlockedNumberEntry> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from s in browseItems.Data select new BrowseItem((BrowseBlockedNumbersModule)module, s)).ToList<object>(),
                            Total = browseItems.Total
                        };
                    }
                },
            };
        }

        public class BrowseModel {
            [Caption(""), Description("")] // empty entries required so property is shown in property list (but with a suppressed label)
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; } = null!;
        }

        [AllowGet]
        public ActionResult BrowseBlockedNumbers() {
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel(Module)
            };
            return View(model);
        }
    }
}
