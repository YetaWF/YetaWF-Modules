/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Modules#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Modules.Endpoints;

namespace YetaWF.Modules.Modules.Modules;

public class ModulesBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, ModulesBrowseModule>, IInstallableModel { }

[ModuleGuid("{276dcecf-f890-4c54-b010-679ee58f0034}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class ModulesBrowseModule : ModuleDefinition {

    public ModulesBrowseModule() {
        Title = this.__ResStr("modTitle", "Modules");
        Name = this.__ResStr("modName", "Modules");
        Description = this.__ResStr("modSummary", "Displays and manages modules and implements module removal. It is also used to display, edit and remove modules. It is accessible using Admin > Panel > Modules (standard YetaWF site).");
        DefaultViewName = StandardViews.Browse;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new ModulesBrowseModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    public override List<RoleDefinition> ExtraRoles {
        get {
            return new List<RoleDefinition>() {
                new RoleDefinition("RemoveItems",
                    this.__ResStr("roleRemItemsC", "Remove Modules"), this.__ResStr("roleRemItems", "The role has permission to remove individual modules"),
                    this.__ResStr("userRemItemsC", "Remove Modules"), this.__ResStr("userRemItems", "The user has permission to remove individual modules")),
                new RoleDefinition("SetAuthorization",
                    this.__ResStr("roleSetAuthC", "Set Global Authorization"), this.__ResStr("roleSetAuth", "The role has permission to set global authorization for all modules"),
                    this.__ResStr("userSetAuthC", "Set Global Authorization"), this.__ResStr("userSetAuth", "The user has permission to set global authorization for all modules")),
                new RoleDefinition("RestoreAuthorization",
                    this.__ResStr("roleRestAuthC", "Restore Default Authorization"), this.__ResStr("roleRestAuth", "The role has permission to restore the default authorizations for ALL modules"),
                    this.__ResStr("userRestAuthC", "Restore Default Authorization"), this.__ResStr("userRestAuth", "The user has permission to restore the default authorizations for ALL modules")),
            };
        }
    }

    public ModuleAction? GetAction_Modules(string url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Browse",
            LinkText = this.__ResStr("browseLink", "Modules"),
            MenuText = this.__ResStr("browseText", "Modules"),
            Tooltip = this.__ResStr("browseTooltip", "Display and manage modules"),
            Legend = this.__ResStr("browseLegend", "Displays and manages modules"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
        };
    }
    public ModuleAction? GetAction_ShowModule(Guid modGuid) {
        return new ModuleAction() {
            Url = ModuleDefinition.GetModulePermanentUrl(modGuid),
            Image = "#Display",
            Style = ModuleAction.ActionStyleEnum.NewWindow,
            LinkText = this.__ResStr("displayLink", "Show Module"),
            MenuText = this.__ResStr("displayMenu", "Show Module"),
            Tooltip = this.__ResStr("displayTT", "Display the module in a new window"),
            Legend = this.__ResStr("displayLegend", "Displays the module in a new window"),
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,

            DontFollow = true,
        };
    }
    public ModuleAction? GetAction_Remove(Guid moduleGuid) {
        if (!IsAuthorized("RemoveItems")) return null;
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(ModulesBrowseModuleEndpoints), ModulesBrowseModuleEndpoints.Remove),
            QueryArgs = new { ModuleGuid = moduleGuid },
            Image = "#Remove",
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("removeLink", "Remove Module"),
            MenuText = this.__ResStr("removeMenu", "Remove Module"),
            Tooltip = this.__ResStr("removeTT", "Remove the module permanently - The module and its data are PERMANENTLY deleted and can no longer be used on any pages"),
            Legend = this.__ResStr("removeLegend", "Removes the module permanently - The module and its data are PERMANENTLY deleted and can no longer be used on any pages"),
            Category = ModuleAction.ActionCategoryEnum.Delete,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove this module PERMANENTLY?"),
        };
    }
    public ModuleAction? GetAction_RestoreAllDefaultAuthorization() {
        if (!IsAuthorized("RestoreAuthorization")) return null;
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(ModulesBrowseModuleEndpoints), ModulesBrowseModuleEndpoints.RestoreAuthorization),
            QueryArgs = new { },
            Image = "",
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("restAuthLink", "Restore Default Authorization"),
            MenuText = this.__ResStr("restAuthMenu", "Restore Default Authorization"),
            Tooltip = this.__ResStr("restAuthTT", "DEVELOPMENT FEATURE - Restore the default authorization for all modules"),
            Legend = this.__ResStr("restAuthLegend", "DEVELOPMENT FEATURE - Restores the default authorization for all modules"),
            Category = ModuleAction.ActionCategoryEnum.Significant,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
            ConfirmationText = this.__ResStr("restAuthConfirm", "DEVELOPMENT FEATURE - Are you sure you want to restore the default authorizations for ALL modules - This will reset all modules to their \"factory\" authorization?"),
            PleaseWaitText = this.__ResStr("restAuthPlsWait", "Restoring default authorization for all modules..."),
        };
    }
#if DEBUG
    public async Task<ModuleAction?> GetAction_SetSuperuserAsync(Guid guid) {
        if (!IsAuthorized("SetAuthorization")) return null;
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(ModulesBrowseModuleEndpoints), ModulesBrowseModuleEndpoints.SetSuperuser),
            QueryArgs = new { Guid = guid },
            Image = await CustomIconAsync("go.png"),
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = "Superuser Authorization",
            MenuText = "Superuser Authorization",
            Tooltip = "Change module to superuser only access",
            Legend = "Changes a module to superuser only access",
            Category = ModuleAction.ActionCategoryEnum.Significant,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
        };
    }
    public async Task<ModuleAction?> GetAction_SetAdminAsync(Guid guid) {
        if (!IsAuthorized("SetAuthorization")) return null;
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(ModulesBrowseModuleEndpoints), ModulesBrowseModuleEndpoints.SetAdmin),
            QueryArgs = new { Guid = guid },
            Image = await CustomIconAsync("go.png"),
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = "Admin Authorization",
            MenuText = "Admin Authorization",
            Tooltip = "Change module to admin only access",
            Legend = "Changes a module to admin only access",
            Category = ModuleAction.ActionCategoryEnum.Significant,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
        };
    }
    public async Task<ModuleAction?> GetAction_SetUserAsync(Guid guid) {
        if (!IsAuthorized("SetAuthorization")) return null;
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(ModulesBrowseModuleEndpoints), ModulesBrowseModuleEndpoints.SetUser),
            QueryArgs = new { Guid = guid },
            Image = await CustomIconAsync("go.png"),
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = "User Authorization",
            MenuText = "User Authorization",
            Tooltip = "Change module to user only access",
            Legend = "Changes a module to user only access",
            Category = ModuleAction.ActionCategoryEnum.Significant,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
        };
    }
    public async Task<ModuleAction?> GetAction_SetAnonymousAsync(Guid guid) {
        if (!IsAuthorized("SetAuthorization")) return null;
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(ModulesBrowseModuleEndpoints), ModulesBrowseModuleEndpoints.SetAnonymous),
            QueryArgs = new { Guid = guid },
            Image = await CustomIconAsync("go.png"),
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = "Anonymous Authorization",
            MenuText = "Anonymous Authorization",
            Tooltip = "Change module to anonymous only access",
            Legend = "Changes a module to anonymous only access",
            Category = ModuleAction.ActionCategoryEnum.Significant,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
        };
    }
#endif

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

        public BrowseItem(ModulesBrowseModule browseMod, ModuleDefinition? modSettings, SerializableList<DesignedModule> designedList, ModuleDefinition mod, int useCount) {

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

    public GridDefinition GetGridModel() {
        return new GridDefinition {
            ModuleGuid = ModuleGuid,
            SettingsModuleGuid = PermanentGuid,
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
                        list.Add(new BrowseItem(this, modSettings, designedList, s, useCount));
                    }
                }
                return new DataSourceResult {
                    Data = list.ToList<object>(),
                    Total = info.Total
                };
            },
        };
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        BrowseModel model = new BrowseModel {
            GridDef = GetGridModel()
        };
        return await RenderAsync(model);
    }
}
