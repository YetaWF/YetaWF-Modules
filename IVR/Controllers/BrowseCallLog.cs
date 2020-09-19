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

    public class BrowseCallLogModuleController : ControllerImpl<Softelvdm.Modules.IVR.Modules.BrowseCallLogModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };

                    DisplayCallLogModule dispMod = new DisplayCallLogModule();
                    actions.New(dispMod.GetAction_Display(Module.DisplayUrl, Id), ModuleAction.ActionLocationEnum.GridLinks);
                    actions.New(Module.GetAction_Remove(Id), ModuleAction.ActionLocationEnum.GridLinks);
                    return actions;
                }
            }

            [Caption("Id"), Description("The internal id")]
            [UIHint("IntValue"), ReadOnly]
            public int Id { get; set; }

            [Caption("Created"), Description("The date/time the call was received")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Created { get; set; }

            [Caption("From"), Description("The caller's phone number")]
            [UIHint("Softelvdm_IVR_PhoneNumber"), ReadOnly]
            [ExcludeDemoMode]
            public string Caller { get; set; }
            [Caption("From City"), Description("The caller's city (if available)")]
            [UIHint("String"), ReadOnly]
            public string CallerCity { get; set; }
            [Caption("From State"), Description("The caller's state (if available)")]
            [UIHint("String"), ReadOnly]
            public string CallerState { get; set; }
            [Caption("From Zip Code"), Description("The caller's ZIP code (if available)")]
            [UIHint("String"), ReadOnly]
            public string CallerZip { get; set; }
            [Caption("From Country"), Description("The caller's country (if available)")]
            [UIHint("String"), ReadOnly]
            public string CallerCountry { get; set; }

            [Caption("Phone Number"), Description("The phone number called")]
            [UIHint("Softelvdm_IVR_PhoneNumber"), ReadOnly]
            public string To { get; set; }

            private BrowseCallLogModule Module { get; set; }

            public BrowseItem(BrowseCallLogModule module, CallLogEntry data) {
                Module = module;
                ObjectSupport.CopyData(data, this);
            }
        }
        private GridDefinition GetGridModel() {
            return new GridDefinition {
                SizeStyle = GridDefinition.SizeStyleEnum.SizeToFit,
                ModuleGuid = Module.ModuleGuid,
                SettingsModuleGuid = Module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = GetActionUrl(nameof(BrowseCallLog_GridData)),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                    using (CallLogDataProvider dataProvider = new CallLogDataProvider()) {
                        DataProviderGetRecords<CallLogEntry> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
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
        public ActionResult BrowseCallLog() {
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel()
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> BrowseCallLog_GridData(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync(GetGridModel(), gridPVData);
        }

        [AllowPost]
        [Permission("RemoveItems")]
        [ExcludeDemoMode]
        public async Task<ActionResult> Remove(int id) {
            using (CallLogDataProvider dataProvider = new CallLogDataProvider()) {
                if (!await dataProvider.RemoveItemByIdentityAsync(id))
                    throw new Error(this.__ResStr("cantRemove", "Couldn't remove item with id {0}", id));
                return Reload(null, Reload: ReloadEnum.ModuleParts);
            }
        }
    }
}
