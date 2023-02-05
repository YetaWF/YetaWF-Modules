/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Modules#License */

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.IO;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.Modules.Endpoints;
using YetaWF.Modules.Modules.Modules;

namespace YetaWF.Modules.Modules.Controllers {

    public class ModulesBrowseModuleController : ControllerImpl<YetaWF.Modules.Modules.Modules.ModulesBrowseModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ModuleActionsGrid"), ReadOnly]
            public List<ModuleAction> Commands { get; set; } = null!;

            public async Task<List<ModuleAction>> __GetCommandsAsync() {
                List<ModuleAction> actions = new List<ModuleAction>();

                Guid guid = ModuleGuid;
                try {
                    actions.New(Module.GetAction_ShowModule(guid), ModuleAction.ActionLocationEnum.GridLinks);
                } catch (Exception) { }
                if (ModSettings != null) {
                    try {
                        actions.New(await ModSettings.GetModuleActionAsync("Settings", guid), ModuleAction.ActionLocationEnum.GridLinks);
                    } catch (Exception) { }
                }
#if DEBUG
                actions.New(await Module.GetAction_SetSuperuserAsync(guid), ModuleAction.ActionLocationEnum.GridLinks);
                actions.New(await Module.GetAction_SetAdminAsync(guid), ModuleAction.ActionLocationEnum.GridLinks);
                actions.New(await Module.GetAction_SetUserAsync(guid), ModuleAction.ActionLocationEnum.GridLinks);
                actions.New(await Module.GetAction_SetAnonymousAsync(guid), ModuleAction.ActionLocationEnum.GridLinks);
#endif
                try {
                    actions.New(Module.GetAction_Remove(guid), ModuleAction.ActionLocationEnum.GridLinks);
                } catch (Exception) { }
                return actions;
            }

            [Caption("Name"), Description("The module name, which is used to identify the module")]
            [UIHint("String"), ReadOnly]
            public string Name { get; set; } = null!;

            [Caption("Title"), Description("The module title, which appears at the top of the module as its title")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString? Title { get; set; }

            [Caption("Summary"), Description("The module description")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString? Description { get; set; }

            [Caption("Use Count"), Description("The number of pages where this module is used")]
            [UIHint("IntValue"), ReadOnly]
            public int UseCount { get; set; }

            [Caption("Anonymous"), Description("Anonymous users can view this module")]
            [UIHint("Boolean"), ReadOnly]
            public bool Anonymous { get; set; }

            [Caption("Users"), Description("Logged on users can view this module")]
            [UIHint("Boolean"), ReadOnly]
            public bool Users { get; set; }

            [Caption("Editors"), Description("Editors can view this module")]
            [UIHint("Boolean"), ReadOnly]
            public bool Editors { get; set; }

            [Caption("Administrators"), Description("Administrators can view this module")]
            [UIHint("Boolean"), ReadOnly]
            public bool Administrators { get; set; }

            [Caption("CSS Class"), Description("The optional CSS classes to be added to the module's <div> tag for further customization through stylesheets")]
            [UIHint("String"), ReadOnly]
            public string? CssClass { get; set; }

            [Caption("Search Keywords"), Description("Defines whether this module's contents should be added to the site's search keywords")]
            [UIHint("Boolean"), ReadOnly]
            public bool WantSearch { get; set; }

            [Caption("Wants Input Focus"), Description("Defines whether input fields in this module should receive the input focus if it's first on the page")]
            [UIHint("Boolean"), ReadOnly]
            public bool WantFocus { get; set; }

            [Caption("Area"), Description("The area implementing this module")]
            [UIHint("String"), ReadOnly]
            public string AreaName { get; set; } = null!;

            [Caption("Module Guid"), Description("The id uniquely identifying this module")]
            [UIHint("Guid"), ReadOnly]
            public Guid ModuleGuid { get; set; }

            private ModulesBrowseModule Module { get; set; }
            private ModuleDefinition? ModSettings { get; set; }

            public BrowseItem(ModulesBrowseModule browseMod,  ModuleDefinition? modSettings, SerializableList<DesignedModule> designedList, ModuleDefinition mod, int useCount) {

                Module = browseMod;
                ModSettings = modSettings;
                ObjectSupport.CopyData(mod, this);

                ModuleGuid = mod.ModuleGuid;
                DesignedModule? desMod = (from d in designedList where d.ModuleGuid == mod.ModuleGuid select d).FirstOrDefault();
                if (desMod != null) {
                    Description = desMod.Description;
                    AreaName = desMod.AreaName;
                }
                UseCount = useCount;
                Anonymous = mod.IsAuthorized_View_Anonymous();
                Users = mod.IsAuthorized_View_AnyUser();
                Editors = mod.IsAuthorized_View_Editor();
                Administrators = mod.IsAuthorized_View_Administrator();
            }
        }

        public class BrowseModel {
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; } = null!;
        }

        internal static GridDefinition GetGridModel(ModuleDefinition module) {
            return new GridDefinition {
                ModuleGuid = module.ModuleGuid,
                SettingsModuleGuid = module.PermanentGuid,
                InitialPageSize = 20,
                RecordType = typeof(BrowseItem),
                AjaxUrl = Utility.UrlFor<ModulesBrowseModuleEndpoints>(GridSupport.BrowseGridData),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                    // module settings services
                    ModuleDefinition? modSettings = await ModuleDefinition.LoadAsync(Manager.CurrentSite.ModuleEditingServices, AllowNone: true);
                    //if (modSettings == null)
                    //    throw new InternalError("No module edit settings services available - no module has been defined");

                    Module.ModuleBrowseInfo info = new Module.ModuleBrowseInfo() {
                        Skip = skip,
                        Take = take,
                        Sort = sort,
                        Filters = filters,
                    };
                    await YetaWF.Core.IO.Module.GetModulesAsync(info);
                    SerializableList<DesignedModule> designedList = await DesignedModules.LoadDesignedModulesAsync();
                    List<BrowseItem> list = new List<BrowseItem>();
                    if (info.Modules != null) {
                        foreach (ModuleDefinition s in info.Modules) {
                            int useCount = (await s.__GetPagesAsync()).Count;
                            list.Add(new BrowseItem((ModulesBrowseModule)module, modSettings, designedList, s, useCount));
                        }
                    }
                    return new DataSourceResult {
                        Data = list.ToList<object>(),
                        Total = info.Total
                    };
                },
            };
        }

        [AllowGet]
        public ActionResult ModulesBrowse() {
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel(Module)
            };
            return View(model);
        }
    }
}