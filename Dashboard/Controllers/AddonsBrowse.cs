/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

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
using YetaWF.Core.Packages;
using Microsoft.AspNetCore.Mvc;

namespace YetaWF.Modules.Dashboard.Controllers {

    public class AddonsBrowseModuleController : ControllerImpl<YetaWF.Modules.Dashboard.Modules.AddonsBrowseModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ModuleActionsGrid"), ReadOnly]
            public List<ModuleAction> Commands {
                get {
                    List<ModuleAction> actions = new List<ModuleAction>();

                    AddonDisplayModule dispMod = new AddonDisplayModule();
                    actions.New(dispMod.GetAction_Display(Module.DisplayUrl, AddonKey), ModuleAction.ActionLocationEnum.GridLinks);

                    return actions;
                }
            }

            [Caption("Type"), Description("The AddOn type")]
            [UIHint("Enum"), ReadOnly]
            public Package.AddOnType Type { get; set; }
            [Caption("Domain"), Description("The domain owning this AddOn")]
            [UIHint("String"), ReadOnly]
            public string Domain { get; set; } = null!;
            [Caption("Product"), Description("The AddOn's product name")]
            [UIHint("String"), ReadOnly]
            public string Product { get; set; } = null!;
            [Caption("Version"), Description("The AddOn's version")]
            [UIHint("String"), ReadOnly]
            public string Version { get; set; } = null!;
            [Caption("Name"), Description("The AddOn's internal name")]
            [UIHint("String"), ReadOnly]
            public string Name { get; set; } = null!;
            [Caption("Url"), Description("The AddOn's Url where its files are located")]
            [UIHint("String"), ReadOnly]
            public string Url { get; set; } = null!;

            public AddonsBrowseModule Module { get; set; }

            [UIHint("Hidden"), ReadOnly]
            public string AddonKey { get; set; } = null!;

            public BrowseItem(AddonsBrowseModule module, Package.AddOnProduct data) {
                Module = module;
                ObjectSupport.CopyData(data, this);
            }
        }

        public class BrowseModel {

            [Caption("AddOns Url"), Description("The Url containing all AddOns")]
            [UIHint("String"), ReadOnly]
            public string AddOnsUrl { get; set; } = null!;

            [Caption("Custom AddOns Url"), Description("The Url containing all customized AddOns (if any)")]
            [UIHint("String"), ReadOnly]
            public string AddOnsCustomUrl { get; set; } = null!;

            [Caption("NPM Url"), Description("The Url containing npm modules")]
            [UIHint("String"), ReadOnly]
            public string NodeModulesUrl { get; set; } = null!;

            [Caption("Installed AddOns"), Description("Displays all installed AddOns")]
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; } = null!;
        }
        private GridDefinition GetGridModel() {
            return new GridDefinition {
                InitialPageSize = 20,
                ModuleGuid = Module.ModuleGuid,
                SettingsModuleGuid = Module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = GetActionUrl(nameof(AddonsBrowse_GridData)),
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo>? sorts, List<DataProviderFilterInfo>? filters) => {
                    DataProviderGetRecords<BrowseItem> recs = DataProviderImpl<BrowseItem>.GetRecords(data, skip, take, sorts, filters);
                    foreach (BrowseItem r in recs.Data)
                        r.Module = Module;
                    return new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total,
                    };
                },
                DirectDataAsync = (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                    List<Package.AddOnProduct> list = Package.GetAvailableAddOns();
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
                AddOnsUrl = Package.AddOnsUrl,
                AddOnsCustomUrl = Package.AddOnsCustomUrl,
                NodeModulesUrl = Globals.NodeModulesUrl,
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
