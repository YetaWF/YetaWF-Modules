/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Modules.Search.Controllers;
using YetaWF.Modules.Search.DataProvider;

namespace YetaWF.Modules.Search.Endpoints {

    public class SearchBrowseModuleEndpoints : YetaWFEndpoints {

        internal static string Remove = "Remove";
        internal static string RemoveAll = "RemoveAll";
        internal static string CollectKeywords = "CollectKeywords";

        private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(SearchBrowseModuleEndpoints), name, defaultValue, parms); }

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(SearchBrowseModuleEndpoints)))
                .RequireAuthorization()
                .AntiForgeryToken();

            group.MapPost(GridSupport.BrowseGridData, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
                ModuleDefinition module = await GetModuleAsync(gridPvData.__ModuleGuid);
                if (!module.IsAuthorized()) return Results.Unauthorized();
                return await GridSupport.GetGridPartialAsync(context, module, SearchBrowseModuleController.GetGridModel(module), gridPvData);
            });

            group.MapPost($"{Remove}/{{searchDataId}}", async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromRoute] int searchDataId) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized("RemoveItems")) return Results.Unauthorized();
                using (SearchDataProvider searchDP = new SearchDataProvider()) {
                    await searchDP.RemoveItemAsync(searchDataId);
                    return Reload(ReloadEnum.ModuleParts);
                }
            })
                .ExcludeDemoMode();

            group.MapPost(RemoveAll, async (HttpContext context, [FromQuery] Guid __ModuleGuid) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized("RemoveItems")) return Results.Unauthorized();
                using (SearchDataProvider searchDP = new SearchDataProvider()) {
                    await using (ILockObject lockObject = await YetaWF.Core.IO.Caching.LockProvider.LockResourceAsync($"{AreaRegistration.CurrentPackage.AreaName}_{nameof(SearchDataProvider)}")) {
                        await searchDP.RemoveItemsAsync(null);/* ALL */
                    }
                    return Reload(ReloadEnum.ModuleParts);
                }
            })
                .ExcludeDemoMode();

            group.MapPost(CollectKeywords, async (HttpContext context, [FromQuery] Guid __ModuleGuid) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized("RemoveItems")) return Results.Unauthorized();
                Scheduler.Search search = new Scheduler.Search();
                await search.SearchSiteAsync(false);
                return Reload(ReloadEnum.ModuleParts, __ResStr("done", "Site search keywords updated"));
            })
                .ExcludeDemoMode();
        }
    }
}
