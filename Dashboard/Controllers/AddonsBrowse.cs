/* Copyright �2020 Softel vdm, Inc.. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Addons;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Modules.Dashboard.Modules;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using System.Threading.Tasks;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Dashboard.Controllers {

    public class AddonsBrowseModuleController : ControllerImpl<YetaWF.Modules.Dashboard.Modules.AddonsBrowseModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };

                    AddonDisplayModule dispMod = new AddonDisplayModule();
                    actions.New(dispMod.GetAction_Display(Module.DisplayUrl, AddonKey), ModuleAction.ActionLocationEnum.GridLinks);

                    return actions;
                }
            }

            [Caption("Type"), Description("The AddOn type")]
            [UIHint("Enum"), ReadOnly]
            public VersionManager.AddOnType Type { get; set; }
            [Caption("Domain"), Description("The domain owning this AddOn")]
            [UIHint("String"), ReadOnly]
            public string Domain { get; set; }
            [Caption("Product"), Description("The AddOn's product name")]
            [UIHint("String"), ReadOnly]
            public string Product { get; set; }
            [Caption("Version"), Description("The AddOn's version")]
            [UIHint("String"), ReadOnly]
            public string Version { get; set; }
            [Caption("Name"), Description("The AddOn's internal name")]
            [UIHint("String"), ReadOnly]
            public string Name { get; set; }
            [Caption("Url"), Description("The AddOn's Url where its files are located")]
            [UIHint("String"), ReadOnly]
            public string Url { get; set; }

            public AddonsBrowseModule Module { get; set; }

            [UIHint("Hidden"), ReadOnly]
            public string AddonKey { get; set; }

            public BrowseItem(AddonsBrowseModule module, VersionManager.AddOnProduct data) {
                Module = module;
                ObjectSupport.CopyData(data, this);
            }
            public BrowseItem() { }
        }

        public class BrowseModel {

            [Caption("AddOns Url"), Description("The Url containing all AddOns")]
            [UIHint("String"), ReadOnly]
            public string AddOnsUrl { get; set; }

            [Caption("Custom AddOns Url"), Description("The Url containing all customized AddOns (if any)")]
            [UIHint("String"), ReadOnly]
            public string AddOnsCustomUrl { get; set; }

            [Caption("NPM Url"), Description("The Url containing npm modules")]
            [UIHint("String"), ReadOnly]
            public string NodeModulesUrl { get; set; }

            [Caption("Bower Url"), Description("The Url containing Bower components")]
            [UIHint("String"), ReadOnly]
            public string BowerComponentsUrl { get; set; }

            [Caption("Installed AddOns"), Description("Displays all installed AddOns")]
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; }
        }
        private GridDefinition GetGridModel() {
            return new GridDefinition {
                InitialPageSize = 20,
                ModuleGuid = Module.ModuleGuid,
                SettingsModuleGuid = Module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = GetActionUrl(nameof(AddonsBrowse_GridData)),
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                    DataProviderGetRecords<BrowseItem> recs = DataProviderImpl<BrowseItem>.GetRecords(data, skip, take, sorts, filters);
                    foreach (BrowseItem r in recs.Data)
                        r.Module = Module;
                    return new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total,
                    };
                },
                DirectDataAsync = (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                    List<VersionManager.AddOnProduct> list = VersionManager.GetAvailableAddOns();
                    DataSourceResult data = new DataSourceResult {
                        Data = (from l in list select new BrowseItem(Module, l)).ToList<object>(),
                        Total = list.Count,
                    };
                    return Task.FromResult(data);
                },
            };
        }

        [AllowGet]
        public ActionResult AddonsBrowse() {
            BrowseModel model = new BrowseModel {
                AddOnsUrl = VersionManager.AddOnsUrl,
                AddOnsCustomUrl = VersionManager.AddOnsCustomUrl,
                NodeModulesUrl = Globals.NodeModulesUrl,
                BowerComponentsUrl = Globals.BowerComponentsUrl,
                GridDef = GetGridModel()
            };
            return View(model);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> AddonsBrowse_GridData(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync<BrowseItem>(GetGridModel(), gridPVData);
        }
    }
}
