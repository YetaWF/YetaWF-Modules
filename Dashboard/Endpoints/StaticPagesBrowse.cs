/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Modules.Dashboard.Modules;

namespace YetaWF.Modules.Dashboard.Endpoints;

public class StaticPagesBrowseModuleEndpoints : YetaWFEndpoints {

    internal const string Remove = "Remove";
    internal const string RemoveAll = "RemoveAll";

    private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(StaticPagesBrowseModuleEndpoints), name, defaultValue, parms); }

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(StaticPagesBrowseModuleEndpoints)))
            .RequireAuthorization()
            .AntiForgeryToken();

        group.MapPost(GridSupport.BrowseGridData, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
            StaticPagesBrowseModule module = await GetModuleAsync<StaticPagesBrowseModule>(gridPvData.__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();
            return await GridSupport.GetGridPartialAsync(context, module, module.GetGridModel(), gridPvData);
        });

        group.MapPost(Remove, async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromQuery] string localUrl) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();
            await Manager.StaticPageManager.RemovePageAsync(localUrl);
            return Reload(ReloadEnum.ModuleParts);
        });

        group.MapPost(RemoveAll, async (HttpContext context, [FromQuery] Guid __ModuleGuid) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();
            await Manager.StaticPageManager.RemoveAllPagesAsync();
            return Reload(ReloadEnum.ModuleParts);
        });
    }
}
