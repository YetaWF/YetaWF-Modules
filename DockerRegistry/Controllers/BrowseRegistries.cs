/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DockerRegistry#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Modules.DockerRegistry.DataProvider;
using YetaWF.Modules.DockerRegistry.Modules;
using YetaWF.Core.Support;
using YetaWF.Core.Localize;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DockerRegistry.Controllers {

    public class BrowseRegistriesModuleController : ControllerImpl<YetaWF.Modules.DockerRegistry.Modules.BrowseRegistriesModule> {

        public class BrowseItem {

            public class ExtraData {
                public int RegistryId { get; set; }
            }

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };
                    actions.New(DisplayModule.GetAction_Display(BrowseModule.DisplayUrl, RegistryId, RegistryName));
                    return actions;
                }
            }

            [Caption("Registry"), Description("Shows the name of the registry")]
            [UIHint("String"), ReadOnly]
            public string RegistryName { get; set; }

            public int RegistryId { get; set; }
            public BrowseRegistriesModule BrowseModule { get; set; }
            public DisplayTagsModule DisplayModule { get; set; }

            public BrowseItem(DockerRegistryEntry data, int registryId, BrowseRegistriesModule browseMod, DisplayTagsModule displayMod) {
                RegistryId = registryId;
                BrowseModule = browseMod;
                DisplayModule = displayMod;
                ObjectSupport.CopyData(data, this);
            }
            public BrowseItem() { }
        }
        private GridDefinition GetGridModel(DataSourceResult ds, int registryId) {
            return new GridDefinition {
                InitialPageSize = 20,
                ModuleGuid = Module.ModuleGuid,
                SettingsModuleGuid = Module.PermanentGuid,
                SizeStyle = GridDefinition.SizeStyleEnum.SizeToFit,
                RecordType = typeof(BrowseItem),
                ExtraData = new BrowseItem.ExtraData { RegistryId = registryId },
                AjaxUrl = GetActionUrl(nameof(TemplateGrid_SortFilter)),
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                    DataProviderGetRecords<BrowseItem> recs = DataProviderImpl<BrowseItem>.GetRecords(data, skip, take, sorts, filters);
                    DisplayTagsModule dispMod = new DisplayTagsModule();
                    foreach (BrowseItem b in recs.Data) {
                        b.RegistryId = registryId;
                        b.BrowseModule = Module;
                        b.DisplayModule = dispMod;
                    }
                    return new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total,
                    };
                },
                DirectDataAsync = (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                    return Task.FromResult(ds);
                },
            };
        }

        public class BrowseModel {

            [Caption("Registry Server"), Description("Defines the registry server for which entries are shown")]
            [UIHint("YetaWF_DockerRegistry_RegistryId")]
            public int RegistryId { get; set; }

            [Caption(""), Description("")]
            [UIHint("StringAlert"), ReadOnly]
            [SuppressEmpty]
            public string Error { get; set; }

            [Caption(""), Description("")] // empty entries required so property is shown in property list (but with a suppressed label)
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; }
        }

        [AllowGet]
        public async Task<ActionResult> BrowseRegistries(int? registryId) {
            DataSourceResult ds = new DataSourceResult {
                Data = new List<object>(),
                Total = 0,
            };
            string error = null;
            int regId = (registryId != null) ? (int)registryId : 0;
            if (regId != 0) {
                using (RegistryEntryDataProvider regDP = new RegistryEntryDataProvider()) {
                    RegistryEntry reg = await regDP.GetItemAsync(regId);
                    if (reg == null) {
                        regId = 0;
                    } else {
                        // get all data now so we can report any errors in the ui rather than handling an exception later
                        using (DockerRegistryDataProvider dockerRegDP = new DockerRegistryDataProvider()) {
                            DisplayTagsModule dispMod = new DisplayTagsModule();
                            DataProviderGetRecords<DockerRegistryEntry> browseItems = null;
                            try {
                                browseItems = await dockerRegDP.GetRegistriesAsync(reg.RegistryURL, reg.UserName, reg.Password, null, null);
                                ds = new DataSourceResult {
                                    Data = (from s in browseItems.Data select new BrowseItem(s, regId, Module, dispMod)).ToList<object>(),
                                    Total = browseItems.Total
                                };
                            } catch (System.Exception exc) {
                                error = ErrorHandling.FormatExceptionMessage(exc);
                            }
                        }
                    }
                }
            }
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel(ds, regId),
                RegistryId = regId,
                Error = error,

            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> TemplateGrid_SortFilter(GridPartialViewData gridPVData, int registryId) {
            return await GridPartialViewAsync<BrowseItem>(GetGridModel(null, registryId), gridPVData);
        }
    }
}
