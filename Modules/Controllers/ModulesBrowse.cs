/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Modules#License */

using System;
using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Menus;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.Modules.Modules;
using System.Threading.Tasks;
using YetaWF.Core.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

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

            [Caption("CSS Class"), Description("The optional CSS classes to be added to the module's <div> tag for further customization through stylesheets")]
            [UIHint("String"), ReadOnly]
            public string CssClass { get; set; }

            [Caption("Search Keywords"), Description("Defines whether this module's contents should be added to the site's search keywords")]
            [UIHint("Boolean"), ReadOnly]
            public bool WantSearch { get; set; }

            [Caption("Wants Input Focus"), Description("Defines whether input fields in this module should receive the input focus if it's first on the page")]
            [UIHint("Boolean"), ReadOnly]
            public bool WantFocus { get; set; }

            [Caption("Module Guid"), Description("The id uniquely identifying this module")]
            [UIHint("Guid"), ReadOnly]
            public Guid ModuleGuid { get; set; }

            private ModulesBrowseModule Module { get; set; }
            private ModuleDefinition ModSettings { get; set; }

            public BrowseItem(ModulesBrowseModule browseMod,  ModuleDefinition modSettings, ModuleDefinition mod, int useCount) {
                Module = browseMod;
                ModSettings = modSettings;
                ObjectSupport.CopyData(mod, this);
                ModuleGuid = mod.ModuleGuid;
                Description = mod.Description;
                UseCount = useCount;
                Anonymous = mod.IsAuthorized_View_Anonymous();
                Users = mod.IsAuthorized_View_AnyUser();
            }
        }

        public class BrowseModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }

        [AllowGet]
        public ActionResult ModulesBrowse() {
            BrowseModel model = new BrowseModel { };
            model.GridDef = new GridDefinition {
                AjaxUrl = GetActionUrl("ModulesBrowse_GridData"),
                ModuleGuid = Module.ModuleGuid,
                RecordType = typeof(BrowseItem),
                SettingsModuleGuid = Module.PermanentGuid,
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> ModulesBrowse_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, Guid settingsModuleGuid) {
            // module settings services
            ModuleDefinition modSettings = await ModuleDefinition.LoadAsync(Manager.CurrentSite.ModuleEditingServices, AllowNone: true);
            //if (modSettings == null)
            //    throw new InternalError("No module edit settings services available - no module has been defined");

            ModuleDefinition.ModuleBrowseInfo info = new ModuleDefinition.ModuleBrowseInfo() {
                Skip = skip,
                Take = take,
                Sort = sort,
                Filters = filters,
            };
            await ModuleDefinition.GetModulesAsync(info);
            Grid.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
            List<BrowseItem> list = new List<BrowseItem>();
            foreach (ModuleDefinition s in info.Modules) {
                int useCount = (await s.__GetPagesAsync()).Count;
                list.Add(new BrowseItem(Module, modSettings, s, useCount));
            }
            return await GridPartialViewAsync(new DataSourceResult {
                Data = list.ToList<object>(),
                Total = info.Total
            });
        }

        [AllowPost]
        [Permission("RemoveItems")]
        [ExcludeDemoMode]
        public async Task<ActionResult> Remove(Guid moduleGuid) {
            if (!await ModuleDefinition.RemoveModuleDefinitionAsync(moduleGuid))
                throw new Error(this.__ResStr("errRemove", "The module could not be removed - It may already have been deleted"));
            return Reload(null, Reload: ReloadEnum.ModuleParts);
        }
        [AllowPost]
        [Permission("RestoreAuthorization")]
        [ExcludeDemoMode]
        public async Task<ActionResult> RestoreAuthorization() {
            ModuleDefinition.ModuleBrowseInfo info = new ModuleDefinition.ModuleBrowseInfo();
            await ModuleDefinition.GetModulesAsync(info);
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