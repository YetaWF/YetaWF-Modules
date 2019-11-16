/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.Pages.DataProvider;
using YetaWF.Modules.Pages.Modules;
using YetaWF.Core.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Pages.Controllers {

    public class UnifiedSetsBrowseModuleController : ControllerImpl<YetaWF.Modules.Pages.Modules.UnifiedSetsBrowseModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };

                    UnifiedSetEditModule editMod = new UnifiedSetEditModule();
                    actions.New(editMod.GetAction_Edit(Module.EditUrl, UnifiedSetGuid), ModuleAction.ActionLocationEnum.GridLinks);

                    actions.New(Module.GetAction_Remove(UnifiedSetGuid, Name), ModuleAction.ActionLocationEnum.GridLinks);
                    return actions;
                }
            }

            [Caption("Name"), Description("The name of this Unified Page Set")]
            [UIHint("String"), ReadOnly]
            public string Name { get; set; }

            [Caption("Disabled"), Description("Defines whether the Unified Page Set is disabled")]
            [UIHint("Boolean"), ReadOnly]
            public bool Disabled { get; set; }

            [Caption("Mode"), Description("Defines how page content is combined")]
            [UIHint("Enum"), ReadOnly]
            public PageDefinition.UnifiedModeEnum UnifiedMode { get; set; }

            [Caption("Description"), Description("The description for this Unified Page Set")]
            [UIHint("String"), ReadOnly]
            public string Description { get; set; }

            [Caption("Created"), Description("The date/time this set was created")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Created { get; set; }
            [Caption("Updated"), Description("The date/time this set was last updated")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Updated { get; set; }

            [Caption("Master Page"), Description("Defines the master page for the Unified Page Set that defines the skin, referenced modules, authorization and all other page attributes")]
            [UIHint("PageSelection"), ReadOnly]
            public Guid MasterPageGuid { get; set; }

            [Caption("Id"), Description("The internal id used to identify this Unified Page Set")]
            [UIHint("Guid"), ReadOnly]
            public Guid UnifiedSetGuid { get; set; }

            private UnifiedSetsBrowseModule Module { get; set; }

            public BrowseItem(UnifiedSetsBrowseModule module, UnifiedSetData unifiedSet) {
                Module = module;
                ObjectSupport.CopyData(unifiedSet, this);
            }
        }

        [Header("A Unified Page Set combines multiple, separately designed pages into one page. " +
            "When the user navigates between the pages in the set, only the required modules are replaced. Depending on the mode selected, no server access is required as all required portions of the pages have been preloaded. " +
            "Only modules within designated panes are exchanged, minimizing data transfer.")]
        public class BrowseModel {
            [Caption(""), Description("")] // empty entries required so property is shown in property list (but with a suppressed label)
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; }
        }
        private GridDefinition GetGridModel() {
            return new GridDefinition {
                ModuleGuid = Module.ModuleGuid,
                SettingsModuleGuid = Module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = GetActionUrl(nameof(UnifiedSetsBrowse_GridData)),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                    using (UnifiedSetDataProvider unifiedSetDP = new UnifiedSetDataProvider()) {
                        DataProviderGetRecords<UnifiedSetData> browseItems = await unifiedSetDP.GetItemsAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from s in browseItems.Data select new BrowseItem(Module, s)).ToList<object>(),
                            Total = browseItems.Total
                        };
                    }
                },
            };
        }

        [AllowGet]
        public ActionResult UnifiedSetsBrowse() {
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel()
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> UnifiedSetsBrowse_GridData(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync(GetGridModel(), gridPVData);
        }

        [AllowPost]
        [Permission("RemoveItems")]
        [ExcludeDemoMode]
        public async Task<ActionResult> Remove(Guid unifiedSetGuid) {
            using (UnifiedSetDataProvider unifiedSet = new UnifiedSetDataProvider()) {
                if (!await unifiedSet.RemoveItemAsync(unifiedSetGuid))
                    throw new Error(this.__ResStr("cantRemove", "Couldn't remove {0}", unifiedSetGuid));
                return Reload(null, Reload: ReloadEnum.ModuleParts);
            }
        }
    }
}
