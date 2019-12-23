/* Copyright �2020 Softel vdm, Inc.. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using Softelvdm.Modules.IVR.DataProvider;
using Softelvdm.Modules.IVR.Modules;
using YetaWF.Core;
using YetaWF.Core.Extensions;
using System;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace Softelvdm.Modules.IVR.Controllers {

    public class BrowseBlockedNumbersModuleController : ControllerImpl<Softelvdm.Modules.IVR.Modules.BrowseBlockedNumbersModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };

                    EditBlockedNumberModule editMod = new EditBlockedNumberModule();
                    actions.New(editMod.GetAction_Edit(Module.EditUrl, Number), ModuleAction.ActionLocationEnum.GridLinks);
                    actions.New(Module.GetAction_Remove(Id), ModuleAction.ActionLocationEnum.GridLinks);
                    return actions;
                }
            }

            [Caption("Blocked Number"), Description("Shows the blocked phone number")]
            [UIHint("Softelvdm_IVR_PhoneNumber"), StringLength(Globals.MaxPhoneNumber), ReadOnly]
            public string Number { get; set; }

            [Caption("Description"), Description("The description of the blocked number")]
            [UIHint("String"), StringLength(BlockedNumberEntry.MaxDescription), ReadOnly]
            public string Description { get; set; }

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
                data.Description = data.Description.Truncate(200);
            }
        }
        private GridDefinition GetGridModel() {
            return new GridDefinition {
                ModuleGuid = Module.ModuleGuid,
                SettingsModuleGuid = Module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = GetActionUrl(nameof(BrowseBlockedNumbers_GridData)),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                    using (BlockedNumberDataProvider dataProvider = new BlockedNumberDataProvider()) {
                        DataProviderGetRecords<BlockedNumberEntry> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from s in browseItems.Data select new BrowseItem(Module, s)).ToList<object>(),
                            Total = browseItems.Total
                        };
                    }
                },
            };
        }

        public class BrowseModel {
            [Caption(""), Description("")] // empty entries required so property is shown in property list (but with a suppressed label)
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; }
        }

        [AllowGet]
        public ActionResult BrowseBlockedNumbers() {
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel()
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> BrowseBlockedNumbers_GridData(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync(GetGridModel(), gridPVData);
        }

        [AllowPost]
        [Permission("RemoveItems")]
        [ExcludeDemoMode]
        public async Task<ActionResult> Remove(int id) {
            using (BlockedNumberDataProvider dataProvider = new BlockedNumberDataProvider()) {
                if (!await dataProvider.RemoveItemByIdentityAsync(id))
                    throw new Error(this.__ResStr("cantRemove", "Couldn't remove entry with id {0}", id));
                return Reload(null, Reload: ReloadEnum.ModuleParts);
            }
        }
    }
}
