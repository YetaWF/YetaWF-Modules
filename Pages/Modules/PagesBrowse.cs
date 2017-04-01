/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Pages#License */

using System;
using System.Collections.Generic;
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
            Description = this.__ResStr("modSummary", "Displays and manages pages");
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

        public ModuleAction GetAction_Pages(string url) {
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
        public ModuleAction GetAction_ShowPage(string url) {
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
            };
        }
        public ModuleAction GetAction_RemoveLink(string pageName) {
            if (!IsAuthorized("RemovePages")) return null;
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(PagesBrowseModuleController), "Remove"),
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
        public ModuleAction GetAction_UpdateAdminAndEditorAuthorization() {
            if (!IsAuthorized("SetAuthorization")) return null;
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(PagesBrowseModuleController), "UpdateAdminAndEditorAuthorization"),
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
        public ModuleAction GetAction_CreateSiteMap() {
            if (!IsAuthorized("SiteMaps")) return null;
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(PagesBrowseModuleController), "CreateSiteMap"),
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
        public ModuleAction GetAction_RemoveSiteMap() {
            if (!IsAuthorized("SiteMaps")) return null;
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(PagesBrowseModuleController), "RemoveSiteMap"),
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
        public ModuleAction GetAction_DownloaSiteMap() {
            if (!IsAuthorized("SiteMaps")) return null;
            SiteMaps sm = new SiteMaps();
            string filename = sm.GetSiteMapFileName();
            if (!System.IO.File.Exists(filename))
                return null;
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(PagesBrowseModuleController), "DownloadSiteMap"),
                NeedsModuleContext = true,
                CookieAsDoneSignal = true,
                Image = "Download.png",
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
    }
}