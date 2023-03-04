/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

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
using YetaWF.Modules.Pages.DataProvider;
using YetaWF.Modules.Pages.Endpoints;
using YetaWF.Modules.Pages.Scheduler;

namespace YetaWF.Modules.Pages.Modules;

public class PagesBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, PagesBrowseModule>, IInstallableModel { }

[ModuleGuid("{4AEDC6D5-4655-48fa-A3C1-A1BF2707030D}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class PagesBrowseModule : ModuleDefinition2 {

    public PagesBrowseModule() : base() {
        Title = this.__ResStr("modTitle", "Pages");
        Name = this.__ResStr("modName", "Pages");
        Description = this.__ResStr("modSummary", "Displays and manages pages and implements page removal. It is accessible using Admin > Panel > Pages (standard YetaWF site). It is used to display, edit settings and remove pages. The Pages Module also provides Site Map creation services.When removing pages, the modules on the page are NOT removed.");
        DefaultViewName = StandardViews.Browse;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new PagesBrowseModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    public override List<RoleDefinition> ExtraRoles {
        get {
            return new List<RoleDefinition>() {
                new RoleDefinition("RemovePages",
                    this.__ResStr("roleRemItemsC", "Remove Pages"), this.__ResStr("roleRemItems", "The role has permission to remove pages"),
                    this.__ResStr("userRemItemsC", "Remove Pages"), this.__ResStr("userRemItems", "The user has permission to remove pages")),
                new RoleDefinition("SetAuthorization",
                    this.__ResStr("roleSetAuthC", "Set Global Authorization"), this.__ResStr("roleSetAuth", "The role has permission to set global authorization for all pages"),
                    this.__ResStr("userSetAuthC", "Set Global Authorization"), this.__ResStr("userSetAuth", "The user has permission to set global authorization for all pages")),
                new RoleDefinition("SiteMaps",
                    this.__ResStr("roleSiteMapsC", "Manage Site Maps"), this.__ResStr("roleSiteMaps", "The role has permission to manage site maps"),
                    this.__ResStr("userSiteMapsC", "Manage Site Maps"), this.__ResStr("userSiteMaps", "The user has permission to manage site maps")),
            };
        }
    }

    public ModuleAction? GetAction_Pages(string? url) {
        return new ModuleAction(this) {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Browse",
            LinkText = this.__ResStr("browseLink", "Pages"),
            MenuText = this.__ResStr("browseText", "Pages"),
            Tooltip = this.__ResStr("browseTooltip", "Display and manage pages"),
            Legend = this.__ResStr("browseLegend", "Displays and manages pages"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
        };
    }
    public ModuleAction? GetAction_ShowPage(string? url) {
        return new ModuleAction(this) {
            Url = url,
            Image = "#Display",
            Style = ModuleAction.ActionStyleEnum.NewWindow,
            LinkText = this.__ResStr("displayLink", "Show Page"),
            MenuText = this.__ResStr("displayMenu", "Show Page"),
            Tooltip = this.__ResStr("displayTT", "Display the page in a new window"),
            Legend = this.__ResStr("displayLegend", "Displays the page"),
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            SaveReturnUrl = true,
            DontFollow = true,
        };
    }
    public ModuleAction? GetAction_RemoveLink(string pageName) {
        if (!IsAuthorized("RemovePages")) return null;
        return new ModuleAction(this) {
            Url = Utility.UrlFor(typeof(PagesBrowseModuleEndpoints), PagesBrowseModuleEndpoints.Remove),
            QueryArgs = new { PageName = pageName },
            Image = "#Remove",
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("removeLink", "Remove Item"),
            MenuText = this.__ResStr("removeMenu", "Remove Item"),
            Legend = this.__ResStr("removeLegend", "Remove the page from the site"),
            Tooltip = this.__ResStr("removeTT", "Removes the page from the site"),
            Category = ModuleAction.ActionCategoryEnum.Delete,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove page \"{0}\"?", pageName),
            SaveReturnUrl = true,
        };
    }
    public ModuleAction? GetAction_UpdateAdminAndEditorAuthorization() {
        if (!IsAuthorized("SetAuthorization")) return null;
        return new ModuleAction(this) {
            Url = Utility.UrlFor(typeof(PagesBrowseModuleEndpoints), nameof(PagesBrowseModuleEndpoints.UpdateAdminAndEditorAuthorization)),
            QueryArgs = new { },
            Image = "",
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("restAuthLink", "Admin & Editor Authorization"),
            MenuText = this.__ResStr("restAuthMenu", "Admin & Editor Authorization"),
            Tooltip = this.__ResStr("restAuthTT", "DEVELOPMENT FEATURE - Add permission to all pages for the Administrator to have full control and the Editor can View & Edit"),
            Legend = this.__ResStr("restAuthLegend", "DEVELOPMENT FEATURE - Add permission to all pages for the Administrator to have full control and the Editor can View & Edit"),
            Category = ModuleAction.ActionCategoryEnum.Significant,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
            ConfirmationText = this.__ResStr("restAuthConfirm", "DEVELOPMENT FEATURE - Are you sure you want to add permission to all pages for the Administrator to have full control and the Editor can View & Edit?"),
            PleaseWaitText = this.__ResStr("restAuthPlsWait", "Updating all pages so the Administrator has full control and the Editor can View & Edit..."),
        };
    }
#if DEBUG
    public async Task<ModuleAction?> GetAction_SetSuperuserAsync(Guid guid) {
        if (!IsAuthorized("SetAuthorization")) return null;
        return new ModuleAction(this) {
            Url = Utility.UrlFor(typeof(PagesBrowseModuleEndpoints), nameof(PagesBrowseModuleEndpoints.SetSuperuser)),
            QueryArgs = new { Guid = guid },
            Image = await CustomIconAsync("go.png"),
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = "Superuser Authorization",
            MenuText = "Superuser Authorization",
            Tooltip = "Change page and its module to superuser only access",
            Legend = "Changes a page and its module to superuser only access",
            Category = ModuleAction.ActionCategoryEnum.Significant,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
        };
    }
    public async Task<ModuleAction?> GetAction_SetAdminAsync(Guid guid) {
        if (!IsAuthorized("SetAuthorization")) return null;
        return new ModuleAction(this) {
            Url = Utility.UrlFor(typeof(PagesBrowseModuleEndpoints), nameof(PagesBrowseModuleEndpoints.SetAdmin)),
            QueryArgs = new { Guid = guid },
            Image = await CustomIconAsync("go.png"),
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = "Admin Authorization",
            MenuText = "Admin Authorization",
            Tooltip = "Change page and its module to admin only access",
            Legend = "Changes a page and its module to admin only access",
            Category = ModuleAction.ActionCategoryEnum.Significant,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
        };
    }
    public async Task<ModuleAction?> GetAction_SetUserAsync(Guid guid) {
        if (!IsAuthorized("SetAuthorization")) return null;
        return new ModuleAction(this) {
            Url = Utility.UrlFor(typeof(PagesBrowseModuleEndpoints), nameof(PagesBrowseModuleEndpoints.SetUser)),
            QueryArgs = new { Guid = guid },
            Image = await CustomIconAsync("go.png"),
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = "User Authorization",
            MenuText = "User Authorization",
            Tooltip = "Change page and its module to user only access",
            Legend = "Changes a page and its module to user only access",
            Category = ModuleAction.ActionCategoryEnum.Significant,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
        };
    }
    public async Task<ModuleAction?> GetAction_SetAnonymousAsync(Guid guid) {
        if (!IsAuthorized("SetAuthorization")) return null;
        return new ModuleAction(this) {
            Url = Utility.UrlFor(typeof(PagesBrowseModuleEndpoints), nameof(PagesBrowseModuleEndpoints.SetAnonymous)),
            QueryArgs = new { Guid = guid },
            Image = await CustomIconAsync("go.png"),
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = "Anonymous Authorization",
            MenuText = "Anonymous Authorization",
            Tooltip = "Change page and its module to anonymous only access",
            Legend = "Changes a page and its module to anonymous only access",
            Category = ModuleAction.ActionCategoryEnum.Significant,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
        };
    }
#endif
    public ModuleAction? GetAction_CreateSiteMap() {
        if (!IsAuthorized("SiteMaps")) return null;
        return new ModuleAction(this) {
            Url = Utility.UrlFor(typeof(PagesBrowseModuleEndpoints), nameof(PagesBrowseModuleEndpoints.CreateSiteMap)),
            QueryArgs = new { },
            Image = "#Add",
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("screAuthLink", "Create Site Map"),
            MenuText = this.__ResStr("screAuthMenu", "Create Site Map"),
            Tooltip = this.__ResStr("screAuthTT", "Create a site map"),
            Legend = this.__ResStr("screAuthLegend", "Creates a site map"),
            Category = ModuleAction.ActionCategoryEnum.Significant,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
            ConfirmationText = this.__ResStr("screAuthConfirm", "Are you sure you want to create a new site map?"),
            PleaseWaitText = this.__ResStr("screAuthPlsWait", "Creating site map..."),
        };
    }
    public ModuleAction? GetAction_RemoveSiteMap() {
        if (!IsAuthorized("SiteMaps")) return null;
        return new ModuleAction(this) {
            Url = Utility.UrlFor(typeof(PagesBrowseModuleEndpoints), nameof(PagesBrowseModuleEndpoints.RemoveSiteMap)),
            QueryArgs = new { },
            Image = "#Remove",
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("sremAuthLink", "Remove Site Map"),
            MenuText = this.__ResStr("sremAuthMenu", "Remove Site Map"),
            Tooltip = this.__ResStr("sremAuthTT", "Remove current site map"),
            Legend = this.__ResStr("sremAuthLegend", "Removes the current site map"),
            Category = ModuleAction.ActionCategoryEnum.Significant,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
            ConfirmationText = this.__ResStr("sremAuthConfirm", "Are you sure you want to remove the current site map?"),
        };
    }
    public async Task<ModuleAction?> GetAction_DownloadSiteMapAsync() {
        if (!IsAuthorized("SiteMaps")) return null;
        SiteMaps sm = new SiteMaps();
        string filename = sm.GetSiteMapFileName();
        if (!await FileSystem.FileSystemProvider.FileExistsAsync(filename))
            return null;
        return new ModuleAction(this) {
            Url = Utility.UrlFor(typeof(PagesBrowseModuleEndpoints), nameof(PagesBrowseModuleEndpoints.DownloadSiteMap)),
            CookieAsDoneSignal = true,
            Image = await CustomIconAsync("Download.png"),
            LinkText = this.__ResStr("downloadLink", "Download Site Map"),
            MenuText = this.__ResStr("downloadMenu", "Download Site Map"),
            Tooltip = this.__ResStr("downloadTT", "Download the site map file"),
            Legend = this.__ResStr("downloadLegend", "Downloads the site map file"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
        };
    }
    public async Task<ModuleAction?> GetAction_CreatePageList() {
        if (!IsAuthorized("SiteMaps")) return null;
        return new ModuleAction(this) {
            Url = Utility.UrlFor(typeof(PagesBrowseModuleEndpoints), nameof(PagesBrowseModuleEndpoints.CreatePageList)),
            CookieAsDoneSignal = true,
            Image = await CustomIconAsync("Download.png"),
            LinkText = this.__ResStr("pagelistLink", "Download Page List"),
            MenuText = this.__ResStr("pagelistMenu", "Download Page List"),
            Tooltip = this.__ResStr("pagelistTT", "Download the list of designed pages"),
            Legend = this.__ResStr("pagelistLegend", "Downloads the list of designed pages"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
        };
    }
    public class PageItem {

        [Caption("Actions"), Description("The available actions")]
        [UIHint("ModuleActionsGrid"), ReadOnly]
        public List<ModuleAction> Commands { get; set; } = null!;

        public async Task<List<ModuleAction>> __GetCommandsAsync() {
            List<ModuleAction> actions = new List<ModuleAction>();

            actions.New(Module.GetAction_ShowPage(EvaluatedCanonicalUrl), ModuleAction.ActionLocationEnum.GridLinks);

            if (PageEditModule != null)
                actions.New(await PageEditModule.GetModuleActionAsync("Edit", null, PageGuid), ModuleAction.ActionLocationEnum.GridLinks);
            //#if DEBUG
            //                actions.New(await Module.GetAction_SetSuperuserAsync(PageGuid), ModuleAction.ActionLocationEnum.GridLinks);
            //                actions.New(await Module.GetAction_SetAdminAsync(PageGuid), ModuleAction.ActionLocationEnum.GridLinks);
            //                actions.New(await Module.GetAction_SetUserAsync(PageGuid), ModuleAction.ActionLocationEnum.GridLinks);
            //                actions.New(await Module.GetAction_SetAnonymousAsync(PageGuid), ModuleAction.ActionLocationEnum.GridLinks);
            //#endif
            actions.New(Module.GetAction_RemoveLink(Url), ModuleAction.ActionLocationEnum.GridLinks);

            return actions;
        }

        [Caption("Url"), Description("The URL used to identify this page - This is the name of the designed page and may not include necessary query string arguments to display the page")]
        [UIHint("Url"), ReadOnly]
        public string Url { get; set; } = null!;

        [Caption("Canonical URL"), Description("The Canonical URL used to identify this page")]
        [UIHint("Url"), ReadOnly]
        public string? CanonicalUrl { get; set; }

        [Caption("Title"), Description("The page title which will appear as title in the browser window")]
        [UIHint("MultiString"), ReadOnly]
        public MultiString Title { get; set; } = null!;

        [Caption("Description"), Description("The page description (not usually visible, entered by page designer, used for search keywords)")]
        [UIHint("MultiString"), ReadOnly]
        public MultiString Description { get; set; } = null!;

        [Caption("Static Page"), Description("Defines whether the page is rendered as a static page (for anonymous users only) - A page whose content doesn't change can be marked as a static page, which results in faster page load for the end-user - Site Settings can be used to enable/disable the use of static pages globally using the StaticPages property - Static pages are only used with deployed sites")]
        [UIHint("Enum"), ReadOnly]
        public PageDefinition.StaticPageEnum StaticPage { get; set; }

        [Caption("Anonymous"), Description("Anonymous users can view this page")]
        [UIHint("Boolean"), ReadOnly]
        public bool Anonymous { get; set; }

        [Caption("Users"), Description("Logged on users can view this page")]
        [UIHint("Boolean"), ReadOnly]
        public bool Users { get; set; }

        [Caption("Editors"), Description("Editors can view this page")]
        [UIHint("Boolean"), ReadOnly]
        public bool Editors { get; set; }

        [Caption("Administrators"), Description("Administrators can view this page")]
        [UIHint("Boolean"), ReadOnly]
        public bool Administrators { get; set; }

        [Caption("SiteMap Priority"), Description("Defines the page priority used for the site map")]
        [UIHint("Enum"), ReadOnly]
        public PageDefinition.SiteMapPriorityEnum SiteMapPriority { get; set; }

        [Caption("Date Created"), Description("The date the page was created")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime Created { get; set; }
        [Caption("Date Updated"), Description("The date the page was updated")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime Updated { get; set; }

        [Description("The optional CSS classes to be added to the page's <body> tag for further customization through stylesheets")]
        [UIHint("String"), ReadOnly]
        public string? CssClass { get; set; }

        [Caption("Mobile Page URL"), Description("If this page is accessed by a mobile device, it is redirected to the URL defined here as mobile page URL - Redirection is not active in Site Edit Mode")]
        [UIHint("Url"), ReadOnly]
        public string? MobilePageUrl { get; set; }

        [Caption("Redirect To Page"), Description("If this page is accessed, it is redirected to the URL defined here - Redirection is not active in Site Edit Mode")]
        [UIHint("Url"), ReadOnly]
        public string? RedirectToPageUrl { get; set; }

        [Caption("Guid"), Description("The id uniquely identifying this page")]
        [UIHint("Guid"), ReadOnly]
        public Guid PageGuid { get; set; }

        public string EvaluatedCanonicalUrl { get; set; } = null!;
        private PagesBrowseModule Module { get; set; }
        public ModuleDefinition? PageEditModule { get; set; }

        public PageItem(PagesBrowseModule module, PageDefinition page, ModuleDefinition? pageSettings) {
            Module = module;
            PageEditModule = pageSettings;
            ObjectSupport.CopyData(page, this);
            Anonymous = page.IsAuthorized_View_Anonymous();
            Users = page.IsAuthorized_View_AnyUser();
            Editors = page.IsAuthorized_View_Editor();
            Administrators = page.IsAuthorized_View_Administrator();
        }
    }

    public class PagesBrowseModel {
        [UIHint("Grid"), ReadOnly]
        public GridDefinition GridDef { get; set; } = null!;
    }

    public GridDefinition GetGridModel() {
        return new GridDefinition {
            ModuleGuid = ModuleGuid,
            SettingsModuleGuid = PermanentGuid,
            InitialPageSize = 20,
            RecordType = typeof(PageItem),
            AjaxUrl = Utility.UrlFor(typeof(PagesBrowseModuleEndpoints), GridSupport.BrowseGridData),
            DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                // page editing services
                ModuleDefinition? pageSettings = await ModuleDefinition.LoadAsync(Manager.CurrentSite.PageEditingServices, AllowNone: true);
                //if (pageSettings == null)
                //    throw new InternalError("No page edit services available - no module has been defined in Site Properties");

                using (PageDefinitionDataProvider dataProvider = new PageDefinitionDataProvider()) {
                    DataProviderGetRecords<PageDefinition> pages = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                    return new DataSourceResult {
                        Data = (from s in pages.Data select new PageItem(this, s, pageSettings)).ToList<object>(),
                        Total = pages.Total
                    };
                }
            },
        };
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        PagesBrowseModel model = new PagesBrowseModel {
            GridDef = GetGridModel()
        };
        return await RenderAsync(model);
    }
}