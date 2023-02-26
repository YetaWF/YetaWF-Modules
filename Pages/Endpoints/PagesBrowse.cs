/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;
using System.Text;
using YetaWF.Core.Addons;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.Pages.Addons;
using YetaWF.Modules.Pages.DataProvider;
using YetaWF.Modules.Pages.Modules;
using YetaWF.Modules.Pages.Scheduler;
using YetaWF.Modules.Pages.Support;

namespace YetaWF.Modules.Pages.Endpoints;

public class PagesBrowseModuleEndpoints : YetaWFEndpoints {

    internal const string Remove = "Remove";
    internal const string CreateSiteMap = "CreateSiteMap";
    internal const string RemoveSiteMap = "RemoveSiteMap";
    internal const string DownloadSiteMap = "DownloadSiteMap";
    internal const string CreatePageList = "CreatePageList";
    internal const string UpdateAdminAndEditorAuthorization = "UpdateAdminAndEditorAuthorization";
    internal const string SetSuperuser = "SetSuperuser";
    internal const string SetAdmin = "SetAdmin";
    internal const string SetUser = "SetUser";
    internal const string SetAnonymous = "SetAnonymous";

    private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(PagesBrowseModuleEndpoints), name, defaultValue, parms); }

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(PagesBrowseModuleEndpoints)))
            .RequireAuthorization()
            .ResourceAuthorize(Info.Resource_AllowListOfLocalPagesAjax);

        group.MapPost(GridSupport.BrowseGridData, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
            PagesBrowseModule module = await GetModuleAsync<PagesBrowseModule>(gridPvData.__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();
            return await GridSupport.GetGridPartialAsync(context, module, module.GetGridModel(), gridPvData);
        })
            .AntiForgeryToken();

        group.MapPost(Remove, async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromQuery] string pageName) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("RemovePages")) return Results.Unauthorized();
            PageDefinition? page = await PageDefinition.LoadFromUrlAsync(pageName);
            if (page == null)
                throw new Error(__ResStr("noPage", "Page \"{0}\" not found", pageName));
            await PageDefinition.RemovePageDefinitionAsync(page.PageGuid);
            return Reload(ReloadEnum.ModuleParts);
        })
            .ExcludeDemoMode();

        group.MapPost(CreateSiteMap, async (HttpContext context, [FromQuery] Guid __ModuleGuid) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("SiteMaps")) return Results.Unauthorized();
            SiteMaps sm = new SiteMaps();
            await sm.CreateAsync();
            return Reload(ReloadEnum.ModuleParts, PopupText: __ResStr("screDone", "The site map has been successfully created"));
        })
            .ExcludeDemoMode();

        group.MapPost(RemoveSiteMap, async (HttpContext context, [FromQuery] Guid __ModuleGuid) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("SiteMaps")) return Results.Unauthorized();
            SiteMaps sm = new SiteMaps();
            await sm.RemoveAsync();
            return Reload(ReloadEnum.ModuleParts, PopupText: __ResStr("sremDone", "The site map has been removed"));
        })
            .ExcludeDemoMode();

        group.MapGet(DownloadSiteMap, async (HttpContext context, [FromQuery] Guid __ModuleGuid, long cookieToReturn) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("SiteMaps")) return Results.Unauthorized();
            SiteMaps sm = new SiteMaps();
            string filename = sm.GetSiteMapFileName();
            if (!await FileSystem.FileSystemProvider.FileExistsAsync(filename))
                throw new Error(__ResStr("sitemapNotFound", "Site map not found - File '{0}' cannot be located", filename));

            context.Response.Headers.Remove("Cookie");
            context.Response.Cookies.Append(Basics.CookieDone, cookieToReturn.ToString(), new Microsoft.AspNetCore.Http.CookieOptions { HttpOnly = false, Path = "/" });
            return Results.File(filename, null, Path.GetFileName(filename));
        })
            .ExcludeDemoMode();

        group.MapGet(CreatePageList, async (HttpContext context, [FromQuery] Guid __ModuleGuid, long cookieToReturn) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("SiteMaps")) return Results.Unauthorized();
            PageList sm = new PageList();
            string list = await sm.CreateAsync();

            context.Response.Headers.Remove("Cookie");
            context.Response.Cookies.Append(Basics.CookieDone, cookieToReturn.ToString(), new Microsoft.AspNetCore.Http.CookieOptions { HttpOnly = false, Path = "/" });
            byte[] btes = Encoding.UTF8.GetBytes(list);
            return Results.File(btes, null, "FileList.txt");
        })
            .ExcludeDemoMode();

        group.MapPost(UpdateAdminAndEditorAuthorization, async (HttpContext context, [FromQuery] Guid __ModuleGuid) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("SetAuthorization")) return Results.Unauthorized();
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                int adminRole = Resource.ResourceAccess.GetAdministratorRoleId();
                int editorRole = Resource.ResourceAccess.GetEditorRoleId();
                DataProviderGetRecords<PageDefinition> pages = await pageDP.GetItemsAsync(0, 0, null, null);
                foreach (PageDefinition genericPage in pages.Data) {
                    PageDefinition? page = await PageDefinition.LoadAsync(genericPage.PageGuid);
                    if (page != null) {
                        PageDefinition.AllowedRole? role;
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
            return Reload(ReloadEnum.ModuleParts);
        })
            .ExcludeDemoMode();

#if DEBUG
        group.MapPost(SetSuperuser, async (HttpContext context, [FromQuery] Guid __ModuleGuid, Guid guid) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("SetAuthorization")) return Results.Unauthorized();
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                int adminRole = Resource.ResourceAccess.GetAdministratorRoleId();
                PageDefinition? page = await PageDefinition.LoadAsync(guid);
                if (page == null)
                    throw new InternalError($"Page with Guid {0} not found", guid);
                page.AllowedRoles = new SerializableList<PageDefinition.AllowedRole>();
                page.AllowedUsers = new SerializableList<PageDefinition.AllowedUser>();
                foreach (PageDefinition.ModuleEntry modEntry in page.ModuleDefinitions) {
                    ModuleDefinition? m = await modEntry.GetModuleAsync();
                    if (m != null) {
                        m.AllowedRoles = new SerializableList<ModuleDefinition.AllowedRole>();
                        m.AllowedUsers = new SerializableList<ModuleDefinition.AllowedUser>();
                    }
                }
                await page.SaveAsync();
            }
            return Reload(ReloadEnum.ModuleParts);
        })
            .ExcludeDemoMode();

        group.MapPost(SetAdmin, async (HttpContext context, [FromQuery] Guid __ModuleGuid, Guid guid) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("SetAuthorization")) return Results.Unauthorized();
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                int adminRole = Resource.ResourceAccess.GetAdministratorRoleId();
                PageDefinition? page = await PageDefinition.LoadAsync(guid);
                if (page == null)
                    throw new InternalError($"Page with Guid {0} not found", guid);
                page.AllowedRoles = new SerializableList<PageDefinition.AllowedRole>();
                page.AllowedUsers = new SerializableList<PageDefinition.AllowedUser>();
                page.AllowedRoles.Add(new PageDefinition.AllowedRole { RoleId = adminRole, View = PageDefinition.AllowedEnum.Yes });

                foreach (PageDefinition.ModuleEntry modEntry in page.ModuleDefinitions) {
                    ModuleDefinition? m = await modEntry.GetModuleAsync();
                    if (m != null) {
                        m.AllowedRoles = new SerializableList<ModuleDefinition.AllowedRole>();
                        m.AllowedUsers = new SerializableList<ModuleDefinition.AllowedUser>();
                        m.AllowedRoles.Add(new ModuleDefinition.AllowedRole { RoleId = adminRole, View = ModuleDefinition.AllowedEnum.Yes });
                    }
                }
                await page.SaveAsync();
            }
            return Reload(ReloadEnum.ModuleParts);
        })
            .ExcludeDemoMode();

        group.MapPost(SetUser, async (HttpContext context, [FromQuery] Guid __ModuleGuid, Guid guid) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("SetAuthorization")) return Results.Unauthorized();
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                int adminRole = Resource.ResourceAccess.GetAdministratorRoleId();
                int userRole = Resource.ResourceAccess.GetUserRoleId();
                PageDefinition? page = await PageDefinition.LoadAsync(guid);
                if (page == null)
                    throw new InternalError($"Page with Guid {0} not found", guid);
                page.AllowedRoles = new SerializableList<PageDefinition.AllowedRole>();
                page.AllowedUsers = new SerializableList<PageDefinition.AllowedUser>();
                page.AllowedRoles.Add(new PageDefinition.AllowedRole { RoleId = userRole, View = PageDefinition.AllowedEnum.Yes });

                foreach (PageDefinition.ModuleEntry modEntry in page.ModuleDefinitions) {
                    ModuleDefinition? m = await modEntry.GetModuleAsync();
                    if (m != null) {
                        m.AllowedRoles = new SerializableList<ModuleDefinition.AllowedRole>();
                        m.AllowedUsers = new SerializableList<ModuleDefinition.AllowedUser>();
                        m.AllowedRoles.Add(new ModuleDefinition.AllowedRole { RoleId = userRole, View = ModuleDefinition.AllowedEnum.Yes });
                    }
                }
                await page.SaveAsync();
            }
            return Reload(ReloadEnum.ModuleParts);
        })
            .ExcludeDemoMode();

        group.MapPost(SetAnonymous, async (HttpContext context, [FromQuery] Guid __ModuleGuid, Guid guid) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("SetAuthorization")) return Results.Unauthorized();
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                int adminRole = Resource.ResourceAccess.GetAdministratorRoleId();
                int userRole = Resource.ResourceAccess.GetUserRoleId();
                int anonRole = Resource.ResourceAccess.GetAnonymousRoleId();
                PageDefinition? page = await PageDefinition.LoadAsync(guid);
                if (page == null)
                    throw new InternalError($"Page with Guid {0} not found", guid);
                page.AllowedRoles = new SerializableList<PageDefinition.AllowedRole>();
                page.AllowedUsers = new SerializableList<PageDefinition.AllowedUser>();
                page.AllowedRoles.Add(new PageDefinition.AllowedRole { RoleId = userRole, View = PageDefinition.AllowedEnum.Yes });
                page.AllowedRoles.Add(new PageDefinition.AllowedRole { RoleId = anonRole, View = PageDefinition.AllowedEnum.Yes });

                foreach (PageDefinition.ModuleEntry modEntry in page.ModuleDefinitions) {
                    ModuleDefinition? m = await modEntry.GetModuleAsync();
                    if (m != null) {
                        m.AllowedRoles = new SerializableList<ModuleDefinition.AllowedRole>();
                        m.AllowedUsers = new SerializableList<ModuleDefinition.AllowedUser>();
                        m.AllowedRoles.Add(new ModuleDefinition.AllowedRole { RoleId = userRole, View = ModuleDefinition.AllowedEnum.Yes });
                        m.AllowedRoles.Add(new ModuleDefinition.AllowedRole { RoleId = anonRole, View = ModuleDefinition.AllowedEnum.Yes });
                    }
                }
                await page.SaveAsync();
            }
            return Reload(ReloadEnum.ModuleParts);
        })
            .ExcludeDemoMode();
#endif
    }
}
