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
using YetaWF.Core.Localize;
using YetaWF.Core.Support;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DockerRegistry.Controllers {

    public class DisplayTagsModuleController : ControllerImpl<YetaWF.Modules.DockerRegistry.Modules.DisplayTagsModule> {

        public class BrowseItem {

            public class ExtraData {
                public int RegistryId { get; set; }
                public string Repository { get; set; }
            }

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };
                    actions.New(Module.GetAction_RemoveTag(RegistryId, Repository, Digest));
                    return actions;
                }
            }

            [Caption("Tag"), Description("Shows the tag")]
            [UIHint("String"), ReadOnly]
            public string TagName { get; set; }

            [Caption("Size"), Description("Shows the total size")]
            [UIHint("IntValue"), ReadOnly]
            public int Size { get; set; }

            [Caption("Digest"), Description("Shows the image digest")]
            [UIHint("String"), ReadOnly]
            public string Digest { get; set; }

            public int RegistryId { get; set; }
            public string Repository { get; set; }

            public DisplayTagsModule Module { get; set; }

            public BrowseItem(DockerTagEntry data, DisplayTagsModule module, int registryId, string repository) {
                Module = module;
                RegistryId = registryId;
                Repository = repository;
                ObjectSupport.CopyData(data, this);
            }
            public BrowseItem() { }
        }
        private GridDefinition GetGridModel(int registryId, string repository) {
            return new GridDefinition {
                InitialPageSize = 20,
                ModuleGuid = Module.ModuleGuid,
                SettingsModuleGuid = Module.PermanentGuid,
                SizeStyle = GridDefinition.SizeStyleEnum.SizeToFit,
                RecordType = typeof(BrowseItem),
                ExtraData = new BrowseItem.ExtraData { RegistryId = registryId, Repository = repository },
                AjaxUrl = GetActionUrl(nameof(TemplateGrid_SortFilter)),
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                    DataProviderGetRecords<BrowseItem> recs = DataProviderImpl<BrowseItem>.GetRecords(data, skip, take, sorts, filters);
                    foreach (BrowseItem b in recs.Data) {
                        b.Module = Module;
                        b.RegistryId = registryId;
                        b.Repository = repository;
                    }
                    return new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total,
                    };
                },
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                    using (RegistryEntryDataProvider regDP = new RegistryEntryDataProvider()) {
                        RegistryEntry reg = await regDP.GetItemAsync(registryId);
                        if (reg == null)
                            throw new Error(this.__ResStr("gone", "The registry with id {0} has been removed.", registryId));
                        using (DockerRegistryDataProvider dockerRegDP = new DockerRegistryDataProvider()) {
                            DataProviderGetRecords<DockerTagEntry> browseItems = await dockerRegDP.GetTagsAsync(reg.RegistryURL, repository, reg.UserName, reg.Password, sort, filters);
                            return new DataSourceResult {
                                Data = (from s in browseItems.Data select new BrowseItem(s, Module, registryId, repository)).ToList<object>(),
                                Total = browseItems.Total
                            };
                        }
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
        public async Task<ActionResult> DisplayTags(int registryId, string repository) {
            using (RegistryEntryDataProvider regDP = new RegistryEntryDataProvider()) {
                RegistryEntry reg = await regDP.GetItemAsync(registryId);
                if (reg == null)
                    throw new Error(this.__ResStr("gone", "The registry with id {0} has been removed.", registryId));
                BrowseModel model = new BrowseModel {
                    GridDef = GetGridModel(registryId, repository)
                };
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> TemplateGrid_SortFilter(GridPartialViewData gridPVData, int registryId, string repository) {
            return await GridPartialViewAsync<BrowseItem>(GetGridModel(registryId, repository), gridPVData);
        }

        [AllowPost]
        [Permission("RemoveTags")]
        [ExcludeDemoMode]
        public async Task<ActionResult> Remove(int registryId, string repository, string reference) {
            using (RegistryEntryDataProvider regDP = new RegistryEntryDataProvider()) {
                RegistryEntry reg = await regDP.GetItemAsync(registryId);
                if (reg == null)
                    throw new Error(this.__ResStr("alreadyDeleted", "The registry with id {0} has already been removed.", registryId));
                using (DockerRegistryDataProvider dockerRegDP = new DockerRegistryDataProvider()) {
                    await dockerRegDP.RemoveTagAsync(reg.RegistryURL, repository, reference, reg.UserName, reg.Password);
                    return Reload(null, Reload: ReloadEnum.Page, PopupText: this.__ResStr("removed", "Tag has been successfully removed"));
                }
            }
        }
    }
}
