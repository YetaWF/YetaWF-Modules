/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;
using YetaWF.Core.Addons;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.Controllers;
using YetaWF.Modules.Blog.DataProvider;
using YetaWF.Modules.Blog.Scheduler;

namespace YetaWF.Modules.Blog.Endpoints {

    public class CategoriesBrowseModuleEndpoints : YetaWFEndpoints {

        internal const string Remove = "Remove";
        internal const string CreateNewsSiteMap = "CreateNewsSiteMap";
        internal const string RemoveNewsSiteMap = "RemoveNewsSiteMap";
        internal const string DownloadNewsSiteMap = "DownloadNewsSiteMap";

        private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(CategoriesBrowseModuleEndpoints), name, defaultValue, parms); }

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageRoute(package, typeof(CategoriesBrowseModuleEndpoints)))
                .RequireAuthorization()
                .AntiForgeryToken();

            group.MapPost(GridSupport.BrowseGridData, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
                ModuleDefinition module = await GetModuleAsync(gridPvData.__ModuleGuid);
                if (!module.IsAuthorized()) return Results.Unauthorized();
                return await GridSupport.GetGridPartialAsync(context, module, CategoriesBrowseModuleController.GetGridModel(module), gridPvData);
            });

            group.MapPost(Remove, async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromQuery] int blogCategory) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized("RemoveItems")) return Results.Unauthorized();
                using (BlogCategoryDataProvider dataProvider = new BlogCategoryDataProvider()) {
                    await dataProvider.RemoveItemAsync(blogCategory);
                    return Reload(ReloadEnum.ModuleParts);
                }
            })
                .ExcludeDemoMode();

            group.MapPost(CreateNewsSiteMap, async (HttpContext context, [FromQuery] Guid __ModuleGuid) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized("NewsSiteMap")) return Results.Unauthorized();
                NewsSiteMap sm = new NewsSiteMap();
                await sm.CreateAsync();
                return Reload(ReloadEnum.ModuleParts, PopupText: __ResStr("screDone", "The news site map has been successfully created"));
            })
                .ExcludeDemoMode();

            group.MapPost(RemoveNewsSiteMap, async (HttpContext context, [FromQuery] Guid __ModuleGuid) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized("NewsSiteMap")) return Results.Unauthorized();
                NewsSiteMap sm = new NewsSiteMap();
                await sm.RemoveAsync();
                return Reload(ReloadEnum.ModuleParts, PopupText: __ResStr("sremDone", "The news site map has been removed"));
            })
                .ExcludeDemoMode();

            group.MapGet(DownloadNewsSiteMap, async (HttpContext context, [FromQuery] Guid __ModuleGuid, long cookieToReturn) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized("NewsSiteMap")) return Results.Unauthorized();
                NewsSiteMap sm = new NewsSiteMap();
                string filename = sm.GetNewsSiteMapFileName();
                if (!await FileSystem.FileSystemProvider.FileExistsAsync(filename))
                    throw new Error(__ResStr("sitemapNotFound", "News site map not found - File '{0}' cannot be located", filename));
                context.Response.Headers.Remove("Cookie");
                context.Response.Cookies.Append(Basics.CookieDone, cookieToReturn.ToString(), new Microsoft.AspNetCore.Http.CookieOptions { HttpOnly = false, Path = "/" });
                return Results.File(filename, null, Path.GetFileName(filename));
            });
        }
    }
}
