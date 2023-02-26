/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Modules.Sites.Modules;

namespace YetaWF.Modules.Sites.Endpoints;

public class SitesBrowseModuleEndpoints : YetaWFEndpoints {

    private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(SitesBrowseModuleEndpoints), name, defaultValue, parms); }

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(SitesBrowseModuleEndpoints)))
            .RequireAuthorization()
            .AntiForgeryToken();

        group.MapPost(GridSupport.BrowseGridData, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
            SitesBrowseModule module = await GetModuleAsync<SitesBrowseModule>(gridPvData.__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();
            return await GridSupport.GetGridPartialAsync(context, module, module.GetGridModel(), gridPvData);
        });
    }
}
