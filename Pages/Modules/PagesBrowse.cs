/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Pages.Controllers;
using YetaWF.Modules.Pages.Scheduler;

namespace YetaWF.Modules.Pages.Modules {

    public class PagesBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, PagesBrowseModule>, IInstallableModel { }

    [ModuleGuid("{4AEDC6D5-4655-48fa-A3C1-A1BF2707030D}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class PagesBrowseModule : ModuleDefinition {

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
                Url = Utility.UrlFor(typeof(PagesBrowseModuleController), nameof(PagesBrowseModuleController.Remove)),
                QueryArgs = new { PageName = pageName },
                NeedsModuleContext = true,
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
                Url = Utility.UrlFor(typeof(PagesBrowseModuleController), nameof(PagesBrowseModuleController.UpdateAdminAndEditorAuthorization)),
                NeedsModuleContext = true,
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
                Url = Utility.UrlFor(typeof(PagesBrowseModuleController), nameof(PagesBrowseModuleController.SetSuperuser)),
                NeedsModuleContext = true,
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
                Url = Utility.UrlFor(typeof(PagesBrowseModuleController), nameof(PagesBrowseModuleController.SetAdmin)),
                NeedsModuleContext = true,
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
                Url = Utility.UrlFor(typeof(PagesBrowseModuleController), nameof(PagesBrowseModuleController.SetUser)),
                NeedsModuleContext = true,
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
                Url = Utility.UrlFor(typeof(PagesBrowseModuleController), nameof(PagesBrowseModuleController.SetAnonymous)),
                NeedsModuleContext = true,
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
                Url = Utility.UrlFor(typeof(PagesBrowseModuleController), nameof(PagesBrowseModuleController.CreateSiteMap)),
                NeedsModuleContext = true,
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
                Url = Utility.UrlFor(typeof(PagesBrowseModuleController), nameof(PagesBrowseModuleController.RemoveSiteMap)),
                NeedsModuleContext = true,
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
                Url = Utility.UrlFor(typeof(PagesBrowseModuleController), nameof(PagesBrowseModuleController.DownloadSiteMap)),
                NeedsModuleContext = true,
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
                Url = Utility.UrlFor(typeof(PagesBrowseModuleController), nameof(PagesBrowseModuleController.CreatePageList)),
                NeedsModuleContext = true,
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
    }
}