/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using YetaWF.Modules.Pages.DataProvider;
using YetaWF.Modules.Pages.Modules;
using YetaWF.Modules.Pages.Scheduler;
using YetaWF.Core.IO;
using System.Text;
using YetaWF.Core.Components;
using YetaWF.Core.Serializers;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web;
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Pages.Controllers {

    public class PagesBrowseModuleController : ControllerImpl<YetaWF.Modules.Pages.Modules.PagesBrowseModule> {

        public class PageItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands { get; set; }

            public async Task<MenuList> __GetCommandsAsync() {
                MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };

                actions.New(Module.GetAction_ShowPage(EvaluatedCanonicalUrl), ModuleAction.ActionLocationEnum.GridLinks);

                if (PageEditModule != null)
                    actions.New(await PageEditModule.GetModuleActionAsync("Edit", null, PageGuid), ModuleAction.ActionLocationEnum.GridLinks);
#if DEBUG
                actions.New(await Module.GetAction_SetSuperuserAsync(PageGuid), ModuleAction.ActionLocationEnum.GridLinks);
                actions.New(await Module.GetAction_SetAdminAsync(PageGuid), ModuleAction.ActionLocationEnum.GridLinks);
                actions.New(await Module.GetAction_SetUserAsync(PageGuid), ModuleAction.ActionLocationEnum.GridLinks);
                actions.New(await Module.GetAction_SetAnonymousAsync(PageGuid), ModuleAction.ActionLocationEnum.GridLinks);
#endif
                actions.New(Module.GetAction_RemoveLink(Url), ModuleAction.ActionLocationEnum.GridLinks);

                return actions;
            }

            [Caption("Url"), Description("The Url used to identify this page - This is the name of the designed page and may not include necessary query string arguments to display the page")]
            [UIHint("Url"), ReadOnly]
            public string Url { get; set; }

            [Caption("Canonical Url"), Description("The Canonical Url used to identify this page")]
            [UIHint("Url"), ReadOnly]
            public string CanonicalUrl { get; set; }

            [Caption("Title"), Description("The page title which will appear as title in the browser window")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString Title { get; set; }

            [Caption("Description"), Description("The page description (not usually visible, entered by page designer, used for search keywords)")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString Description { get; set; }

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

            [Caption("Page Skin"), Description("The skin used to display the page")]
            [UIHint("PageSkin"), ReadOnly]
            public SkinDefinition SelectedSkin { get; set; }

            [Caption("Popup Skin"), Description("The skin used when the page is displayed in a popup window")]
            [UIHint("PopupSkin"), ReadOnly]
            public SkinDefinition SelectedPopupSkin { get; set; }

            [Caption("Kendo UI Skin"), Description("The skin for Kendo UI elements (buttons, modal dialogs, etc.) used on this page")]
            [UIHint("KendoUISkin"), ReadOnly]
            public string KendoUISkin { get; set; }

            [Description("The optional CSS classes to be added to the page's <body> tag for further customization through stylesheets")]
            [UIHint("String"), ReadOnly]
            public string CssClass { get; set; }

            [Caption("Mobile Page Url"), Description("If this page is accessed by a mobile device, it is redirected to the Url defined here as mobile page Url - Redirection is not active in Site Edit Mode")]
            [UIHint("Url"), ReadOnly]
            public string MobilePageUrl { get; set; }

            [Caption("Redirect To Page"), Description("If this page is accessed, it is redirected to the Url defined here - Redirection is not active in Site Edit Mode")]
            [UIHint("Url"), ReadOnly]
            public string RedirectToPageUrl { get; set; }

            [Caption("Guid"), Description("The id uniquely identifying this page")]
            [UIHint("Guid"), ReadOnly]
            public Guid PageGuid { get; set; }

            public string EvaluatedCanonicalUrl { get; set; }
            private PagesBrowseModule Module { get; set; }
            public ModuleDefinition PageEditModule { get; set; }

            public PageItem(PagesBrowseModule module, PageDefinition page, ModuleDefinition pageSettings) {
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
            public GridDefinition GridDef { get; set; }
        }
        private GridDefinition GetGridModel() {
            return new GridDefinition {
                ModuleGuid = Module.ModuleGuid,
                SettingsModuleGuid = Module.PermanentGuid,
                InitialPageSize = 20,
                RecordType = typeof(PageItem),
                AjaxUrl = GetActionUrl(nameof(PagesBrowse_GridData)),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                    // page editing services
                    ModuleDefinition pageSettings = await ModuleDefinition.LoadAsync(Manager.CurrentSite.PageEditingServices, AllowNone: true);
                    //if (pageSettings == null)
                    //    throw new InternalError("No page edit services available - no module has been defined in Site Properties");

                    using (PageDefinitionDataProvider dataProvider = new PageDefinitionDataProvider()) {
                        DataProviderGetRecords<PageDefinition> pages = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from s in pages.Data select new PageItem(Module, s, pageSettings)).ToList<object>(),
                            Total = pages.Total
                        };
                    }
                },
            };
        }

        [AllowGet]
        public ActionResult PagesBrowse() {
            PagesBrowseModel model = new PagesBrowseModel {
                GridDef = GetGridModel()
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> PagesBrowse_GridData(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync(GetGridModel(), gridPVData);
        }

        [AllowPost]
        [Permission("RemovePages")]
        [ExcludeDemoMode]
        public async Task<ActionResult> Remove(string pageName) {
            if (string.IsNullOrWhiteSpace(pageName))
                throw new Error(this.__ResStr("noPageName", "No page name specified"));
            PageDefinition page = await PageDefinition.LoadFromUrlAsync(pageName);
            if (page == null)
                throw new Error(this.__ResStr("noPage", "Page \"{0}\" not found", pageName));

            await PageDefinition.RemovePageDefinitionAsync(page.PageGuid);
            return Reload(null, Reload: ReloadEnum.ModuleParts);
        }

        [AllowPost]
        [Permission("SetAuthorization")]
        [ExcludeDemoMode]
        public async Task<ActionResult> UpdateAdminAndEditorAuthorization() {
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                int adminRole = Resource.ResourceAccess.GetAdministratorRoleId();
                int editorRole = Resource.ResourceAccess.GetEditorRoleId();
                DataProviderGetRecords<PageDefinition> pages = await pageDP.GetItemsAsync(0, 0, null, null);
                foreach (PageDefinition genericPage in pages.Data) {
                    PageDefinition page = await PageDefinition.LoadAsync(genericPage.PageGuid);
                    if (page != null) {
                        PageDefinition.AllowedRole role;
                        while ((role = PageDefinition.AllowedRole.Find(page.AllowedRoles, adminRole)) != null)
                            page.AllowedRoles.Remove(role);
                        page.AllowedRoles.Add(new PageDefinition.AllowedRole { RoleId = adminRole, View = PageDefinition.AllowedEnum.Yes, Edit = PageDefinition.AllowedEnum.Yes, Remove = PageDefinition.AllowedEnum.Yes, });
                        while ((role = PageDefinition.AllowedRole.Find(page.AllowedRoles, editorRole)) != null)
                            page.AllowedRoles.Remove(role);
                        page.AllowedRoles.Add(new PageDefinition.AllowedRole { RoleId = editorRole, View = PageDefinition.AllowedEnum.Yes, Edit = PageDefinition.AllowedEnum.Yes, });
                        await page.SaveAsync();
                    }
                }
            }
            return Reload(null, Reload: ReloadEnum.ModuleParts);
        }

#if DEBUG
        [AllowPost]
        [Permission("SetAuthorization")]
        [ExcludeDemoMode]
        public async Task<ActionResult> SetSuperuser(Guid guid) {
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                int adminRole = Resource.ResourceAccess.GetAdministratorRoleId();
                PageDefinition page = await PageDefinition.LoadAsync(guid);
                if (page == null)
                    throw new InternalError($"Page with Guid {0} not found", guid);
                page.AllowedRoles = new SerializableList<PageDefinition.AllowedRole>();
                page.AllowedUsers = new SerializableList<PageDefinition.AllowedUser>();
                foreach (PageDefinition.ModuleEntry modEntry in page.ModuleDefinitions) {
                    ModuleDefinition module = await modEntry.GetModuleAsync();
                    if (module != null) {
                        module.AllowedRoles = new SerializableList<ModuleDefinition.AllowedRole>();
                        module.AllowedUsers = new SerializableList<ModuleDefinition.AllowedUser>();
                    }
                }
                await page.SaveAsync();
            }
            return Reload(null, Reload: ReloadEnum.ModuleParts);
        }
        [AllowPost]
        [Permission("SetAuthorization")]
        [ExcludeDemoMode]
        public async Task<ActionResult> SetAdmin(Guid guid) {
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                int adminRole = Resource.ResourceAccess.GetAdministratorRoleId();
                PageDefinition page = await PageDefinition.LoadAsync(guid);
                if (page == null)
                    throw new InternalError($"Page with Guid {0} not found", guid);
                page.AllowedRoles = new SerializableList<PageDefinition.AllowedRole>();
                page.AllowedUsers = new SerializableList<PageDefinition.AllowedUser>();
                page.AllowedRoles.Add(new PageDefinition.AllowedRole { RoleId = adminRole, View = PageDefinition.AllowedEnum.Yes });

                foreach (PageDefinition.ModuleEntry modEntry in page.ModuleDefinitions) {
                    ModuleDefinition module = await modEntry.GetModuleAsync();
                    if (module != null) {
                        module.AllowedRoles = new SerializableList<ModuleDefinition.AllowedRole>();
                        module.AllowedUsers = new SerializableList<ModuleDefinition.AllowedUser>();
                        module.AllowedRoles.Add(new ModuleDefinition.AllowedRole { RoleId = adminRole, View = ModuleDefinition.AllowedEnum.Yes });
                    }
                }
                await page.SaveAsync();
            }
            return Reload(null, Reload: ReloadEnum.ModuleParts);
        }
        [AllowPost]
        [Permission("SetAuthorization")]
        [ExcludeDemoMode]
        public async Task<ActionResult> SetUser(Guid guid) {
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                int adminRole = Resource.ResourceAccess.GetAdministratorRoleId();
                int userRole = Resource.ResourceAccess.GetUserRoleId();
                PageDefinition page = await PageDefinition.LoadAsync(guid);
                if (page == null)
                    throw new InternalError($"Page with Guid {0} not found", guid);
                page.AllowedRoles = new SerializableList<PageDefinition.AllowedRole>();
                page.AllowedUsers = new SerializableList<PageDefinition.AllowedUser>();
                page.AllowedRoles.Add(new PageDefinition.AllowedRole { RoleId = userRole, View = PageDefinition.AllowedEnum.Yes });

                foreach (PageDefinition.ModuleEntry modEntry in page.ModuleDefinitions) {
                    ModuleDefinition module = await modEntry.GetModuleAsync();
                    if (module != null) {
                        module.AllowedRoles = new SerializableList<ModuleDefinition.AllowedRole>();
                        module.AllowedUsers = new SerializableList<ModuleDefinition.AllowedUser>();
                        module.AllowedRoles.Add(new ModuleDefinition.AllowedRole { RoleId = userRole, View = ModuleDefinition.AllowedEnum.Yes });
                    }
                }
                await page.SaveAsync();
            }
            return Reload(null, Reload: ReloadEnum.ModuleParts);
        }
        [AllowPost]
        [Permission("SetAuthorization")]
        [ExcludeDemoMode]
        public async Task<ActionResult> SetAnonymous(Guid guid) {
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                int adminRole = Resource.ResourceAccess.GetAdministratorRoleId();
                int userRole = Resource.ResourceAccess.GetUserRoleId();
                int anonRole = Resource.ResourceAccess.GetAnonymousRoleId();
                PageDefinition page = await PageDefinition.LoadAsync(guid);
                if (page == null)
                    throw new InternalError($"Page with Guid {0} not found", guid);
                page.AllowedRoles = new SerializableList<PageDefinition.AllowedRole>();
                page.AllowedUsers = new SerializableList<PageDefinition.AllowedUser>();
                page.AllowedRoles.Add(new PageDefinition.AllowedRole { RoleId = userRole, View = PageDefinition.AllowedEnum.Yes });
                page.AllowedRoles.Add(new PageDefinition.AllowedRole { RoleId = anonRole, View = PageDefinition.AllowedEnum.Yes });

                foreach (PageDefinition.ModuleEntry modEntry in page.ModuleDefinitions) {
                    ModuleDefinition module = await modEntry.GetModuleAsync();
                    if (module != null) {
                        module.AllowedRoles = new SerializableList<ModuleDefinition.AllowedRole>();
                        module.AllowedUsers = new SerializableList<ModuleDefinition.AllowedUser>();
                        module.AllowedRoles.Add(new ModuleDefinition.AllowedRole { RoleId = userRole, View = ModuleDefinition.AllowedEnum.Yes });
                        module.AllowedRoles.Add(new ModuleDefinition.AllowedRole { RoleId = anonRole, View = ModuleDefinition.AllowedEnum.Yes });
                    }
                }
                await page.SaveAsync();
            }
            return Reload(null, Reload: ReloadEnum.ModuleParts);
        }
#endif

        [AllowPost]
        [Permission("SiteMaps")]
        [ExcludeDemoMode]
        public async Task<ActionResult> CreateSiteMap() {
            SiteMaps sm = new SiteMaps();
            await sm.CreateAsync();
            return Reload(null, Reload: ReloadEnum.ModuleParts, PopupText: this.__ResStr("screDone", "The site map has been successfully created"));
        }

        [AllowPost]
        [Permission("SiteMaps")]
        [ExcludeDemoMode]
        public async Task<ActionResult> RemoveSiteMap() {
            SiteMaps sm = new SiteMaps();
            await sm.RemoveAsync();
            return Reload(null, Reload: ReloadEnum.ModuleParts, PopupText: this.__ResStr("sremDone", "The site map has been removed"));
        }

        [Permission("SiteMaps")]
        public async Task<ActionResult> DownloadSiteMap(long cookieToReturn) {
            SiteMaps sm = new SiteMaps();
            string filename = sm.GetSiteMapFileName();
            if (!await FileSystem.FileSystemProvider.FileExistsAsync(filename))
                throw new Error(this.__ResStr("sitemapNotFound", "Site map not found - File '{0}' cannot be located", filename));
#if MVC6
            Response.Headers.Remove("Cookie");
            Response.Cookies.Append(Basics.CookieDone, cookieToReturn.ToString(), new Microsoft.AspNetCore.Http.CookieOptions { HttpOnly = false, Path = "/" });
#else
            HttpCookie cookie = new HttpCookie(Basics.CookieDone, cookieToReturn.ToString());
            Response.Cookies.Remove(Basics.CookieDone);
            Response.SetCookie(cookie);
#endif

            string contentType = "application/octet-stream";
#if MVC6
            return new PhysicalFileResult(filename, contentType) { FileDownloadName = Path.GetFileName(filename) };
#else
            FilePathResult result = new FilePathResult(filename, contentType);
            result.FileDownloadName = Path.GetFileName(filename);
            return result;
#endif
        }
        [Permission("SiteMaps")]
        public async Task<ActionResult> CreatePageList(long cookieToReturn) {
            PageList sm = new PageList();
            string list = await sm.CreateAsync();
#if MVC6
            Response.Headers.Remove("Cookie");
            Response.Cookies.Append(Basics.CookieDone, cookieToReturn.ToString(), new Microsoft.AspNetCore.Http.CookieOptions { HttpOnly = false, Path = "/" });
#else
            HttpCookie cookie = new HttpCookie(Basics.CookieDone, cookieToReturn.ToString());
            Response.Cookies.Remove(Basics.CookieDone);
            Response.SetCookie(cookie);
#endif

            string contentType = "application/octet-stream";

            byte[] btes = Encoding.UTF8.GetBytes(list);
            FileContentResult result = new FileContentResult(btes, contentType);
            result.FileDownloadName = "FileList.txt";

            return result;
        }
    }
}
