/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Modules#License */

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.Modules.Modules;

namespace YetaWF.Modules.Modules.Controllers {

    public class ModulesBrowseModuleController : ControllerImpl<YetaWF.Modules.Modules.Modules.ModulesBrowseModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands { get; set; }

            public async Task<MenuList> __GetCommandsAsync() {
                MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };

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
            public string Name { get; set; }

            [Caption("Title"), Description("The module title, which appears at the top of the module as its title")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString Title { get; set; }

            [Caption("Summary"), Description("The module description")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString Description { get; set; }

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
            public string CssClass { get; set; }

            [Caption("Search Keywords"), Description("Defines whether this module's contents should be added to the site's search keywords")]
            [UIHint("Boolean"), ReadOnly]
            public bool WantSearch { get; set; }

            [Caption("Wants Input Focus"), Description("Defines whether input fields in this module should receive the input focus if it's first on the page")]
            [UIHint("Boolean"), ReadOnly]
            public bool WantFocus { get; set; }

            [Caption("Area"), Description("The area implementing this module")]
            [UIHint("String"), ReadOnly]
            public string AreaName { get; set; }

            [Caption("Module Guid"), Description("The id uniquely identifying this module")]
            [UIHint("Guid"), ReadOnly]
            public Guid ModuleGuid { get; set; }

            private ModulesBrowseModule Module { get; set; }
            private ModuleDefinition ModSettings { get; set; }

            public BrowseItem(ModulesBrowseModule browseMod,  ModuleDefinition modSettings, SerializableList<DesignedModule> designedList, ModuleDefinition mod, int useCount) {

                Module = browseMod;
                ModSettings = modSettings;
                ObjectSupport.CopyData(mod, this);

                ModuleGuid = mod.ModuleGuid;
                DesignedModule desMod = (from d in designedList where d.ModuleGuid == mod.ModuleGuid select d).FirstOrDefault();
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
            public GridDefinition GridDef { get; set; }
        }
        private GridDefinition GetGridModel() {
            return new GridDefinition {
                ModuleGuid = Module.ModuleGuid,
                SettingsModuleGuid = Module.PermanentGuid,
                InitialPageSize = 20,
                RecordType = typeof(BrowseItem),
                AjaxUrl = GetActionUrl(nameof(ModulesBrowse_GridData)),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                    // module settings services
                    ModuleDefinition modSettings = await ModuleDefinition.LoadAsync(Manager.CurrentSite.ModuleEditingServices, AllowNone: true);
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
                    foreach (ModuleDefinition s in info.Modules) {
                        int useCount = (await s.__GetPagesAsync()).Count;
                        list.Add(new BrowseItem(Module, modSettings, designedList, s, useCount));
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
                GridDef = GetGridModel()
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> ModulesBrowse_GridData(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync(GetGridModel(), gridPVData);
        }

        [AllowPost]
        [Permission("RemoveItems")]
        [ExcludeDemoMode]
        public async Task<ActionResult> Remove(Guid moduleGuid) {
            if (!await YetaWF.Core.IO.Module.RemoveModuleDefinitionAsync(moduleGuid))
                throw new Error(this.__ResStr("errRemove", "The module could not be removed - It may already have been deleted"));
            return Reload(null, Reload: ReloadEnum.ModuleParts);
        }
#if DEBUG
        [AllowPost]
        [Permission("SetAuthorization")]
        [ExcludeDemoMode]
        public async Task<ActionResult> SetSuperuser(Guid guid) {
            ModuleDefinition module = await ModuleDefinition.LoadAsync(guid, AllowNone: true);
            if (module == null)
                throw new InternalError($"Couldn't load module {guid}");
            int adminRole = Resource.ResourceAccess.GetAdministratorRoleId();
            int userRole = Resource.ResourceAccess.GetUserRoleId();
            int anonRole = Resource.ResourceAccess.GetAnonymousRoleId();
            module.AllowedRoles = new SerializableList<ModuleDefinition.AllowedRole>();
            await module.SaveAsync();
            return Reload(null, Reload: ReloadEnum.ModuleParts);
        }
        [AllowPost]
        [Permission("SetAuthorization")]
        [ExcludeDemoMode]
        public async Task<ActionResult> SetAdmin(Guid guid) {
            ModuleDefinition module = await ModuleDefinition.LoadAsync(guid, AllowNone: true);
            if (module == null)
                throw new InternalError($"Couldn't load module {guid}");
            int adminRole = Resource.ResourceAccess.GetAdministratorRoleId();
            int userRole = Resource.ResourceAccess.GetUserRoleId();
            int anonRole = Resource.ResourceAccess.GetAnonymousRoleId();
            module.AllowedRoles = new SerializableList<ModuleDefinition.AllowedRole>();
            module.AllowedRoles.Add(new ModuleDefinition.AllowedRole { RoleId = adminRole, View = ModuleDefinition.AllowedEnum.Yes });
            await module.SaveAsync();
            return Reload(null, Reload: ReloadEnum.ModuleParts);
        }
        [AllowPost]
        [Permission("SetAuthorization")]
        [ExcludeDemoMode]
        public async Task<ActionResult> SetUser(Guid guid) {
            ModuleDefinition module = await ModuleDefinition.LoadAsync(guid, AllowNone: true);
            if (module == null)
                throw new InternalError($"Couldn't load module {guid}");
            int adminRole = Resource.ResourceAccess.GetAdministratorRoleId();
            int userRole = Resource.ResourceAccess.GetUserRoleId();
            int anonRole = Resource.ResourceAccess.GetAnonymousRoleId();
            module.AllowedRoles = new SerializableList<ModuleDefinition.AllowedRole>();
            module.AllowedRoles.Add(new ModuleDefinition.AllowedRole { RoleId = userRole, View = ModuleDefinition.AllowedEnum.Yes });
            await module.SaveAsync();
            return Reload(null, Reload: ReloadEnum.ModuleParts);
        }
        [AllowPost]
        [Permission("SetAuthorization")]
        [ExcludeDemoMode]
        public async Task<ActionResult> SetAnonymous(Guid guid) {
            ModuleDefinition module = await ModuleDefinition.LoadAsync(guid, AllowNone: true);
            if (module == null)
                throw new InternalError($"Couldn't load module {guid}");
            int adminRole = Resource.ResourceAccess.GetAdministratorRoleId();
            int userRole = Resource.ResourceAccess.GetUserRoleId();
            int anonRole = Resource.ResourceAccess.GetAnonymousRoleId();
            module.AllowedRoles = new SerializableList<ModuleDefinition.AllowedRole>();
            module.AllowedRoles.Add(new ModuleDefinition.AllowedRole { RoleId = userRole, View = ModuleDefinition.AllowedEnum.Yes });
            module.AllowedRoles.Add(new ModuleDefinition.AllowedRole { RoleId = anonRole, View = ModuleDefinition.AllowedEnum.Yes });
            await module.SaveAsync();
            return Reload(null, Reload: ReloadEnum.ModuleParts);
        }
#endif
        [AllowPost]
        [Permission("RestoreAuthorization")]
        [ExcludeDemoMode]
        public async Task<ActionResult> RestoreAuthorization() {
            YetaWF.Core.IO.Module.ModuleBrowseInfo info = new YetaWF.Core.IO.Module.ModuleBrowseInfo();
            await YetaWF.Core.IO.Module.GetModulesAsync(info);
            foreach (ModuleDefinition genericMod in info.Modules) {
                ModuleDefinition mod = await ModuleDefinition.LoadAsync(genericMod.ModuleGuid, AllowNone: true);
                if (mod != null) {
#if MERGE // enable this to preserve anon and user settings
                    int anonRoleId = Resource.ResourceAccess.GetAnonymousRoleId();
                    int userRoleId = Resource.ResourceAccess.GetUserRoleId();
                    ModuleDefinition.AllowedRole anonRole = ModuleDefinition.AllowedRole.Find(mod.DefaultAllowedRoles, anonRoleId);
                    ModuleDefinition.AllowedRole userRole = ModuleDefinition.AllowedRole.Find(mod.DefaultAllowedRoles, userRoleId);
                    if (anonRole == null && userRole == null) {
                        // merge default roles into allowed roles to preserve current anon & user settings
                        anonRole = ModuleDefinition.AllowedRole.Find(mod.AllowedRoles, anonRoleId);
                        userRole = ModuleDefinition.AllowedRole.Find(mod.AllowedRoles, userRoleId);
                        mod.AllowedRoles = new SerializableList<ModuleDefinition.AllowedRole>(mod.DefaultAllowedRoles);
                        if (anonRole != null)
                            mod.AllowedRoles.Add(anonRole);
                        if (userRole != null)
                            mod.AllowedRoles.Add(userRole);
                    } else {
                        mod.AllowedRoles = mod.DefaultAllowedRoles;
                    }
#else
                    mod.AllowedRoles = mod.DefaultAllowedRoles;
#endif
                    //mod.AllowedUsers = // we're not touching this

                    await mod.SaveAsync();
                }
            }
            return Reload(null, Reload: ReloadEnum.ModuleParts);
        }
    }
}