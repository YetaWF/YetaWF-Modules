/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using Softelvdm.Modules.IVR.DataProvider;
using Softelvdm.Modules.IVR.Modules;
using System;
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
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace Softelvdm.Modules.IVR.Controllers {

    public class BrowseHolidaysModuleController : ControllerImpl<Softelvdm.Modules.IVR.Modules.BrowseHolidaysModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };
                    actions.New(Module.GetAction_Remove(Id), ModuleAction.ActionLocationEnum.GridLinks);
                    return actions;
                }
            }

            [Caption("Date"), Description("The date the holiday occurs")]
            [UIHint("Date"), ReadOnly]
            public DateTime HolidayDate { get; set; }

            [Caption("Description"), Description("The description of the holiday")]
            [UIHint("String"), StringLength(HolidayEntry.MaxDescription), ReadOnly]
            public string Description { get; set; }

            public int Id { get; set; }

            private BrowseHolidaysModule Module { get; set; }

            public BrowseItem(BrowseHolidaysModule module, HolidayEntry data) {
                Module = module;
                ObjectSupport.CopyData(data, this);
            }
        }
        private GridDefinition GetGridModel() {
            return new GridDefinition {
                SizeStyle = GridDefinition.SizeStyleEnum.SizeToFit,
                ModuleGuid = Module.ModuleGuid,
                //SettingsModuleGuid = Module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = GetActionUrl(nameof(BrowseHolidays_GridData)),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                    using (HolidayEntryDataProvider dataProvider = new HolidayEntryDataProvider()) {
                        DataProviderGetRecords<HolidayEntry> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
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
        public ActionResult BrowseHolidays() {
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel()
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> BrowseHolidays_GridData(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync(GetGridModel(), gridPVData);
        }

        [AllowPost]
        [Permission("RemoveItems")]
        [ExcludeDemoMode]
        public async Task<ActionResult> Remove(int id) {
            using (HolidayEntryDataProvider dataProvider = new HolidayEntryDataProvider()) {
                if (!await dataProvider.RemoveItemByIdentityAsync(id))
                    throw new Error(this.__ResStr("cantRemove", "Couldn't remove holiday with id {0}"));
                return Reload(null, Reload: ReloadEnum.ModuleParts);
            }
        }
    }
}
