/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Packages;
using YetaWF.Modules.Messenger.Modules;

namespace YetaWF.Modules.Messenger.Endpoints;

public class BrowseActiveUsersModuleEndpoints : YetaWFEndpoints {

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(BrowseActiveUsersModuleEndpoints)))
            .RequireAuthorization()
            .AntiForgeryToken();

        group.MapPost(GridSupport.BrowseGridData, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
            BrowseActiveUsersModule module = await GetModuleAsync<BrowseActiveUsersModule>(gridPvData.__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();
            return await GridSupport.GetGridPartialAsync(context, module, module.GetGridModel(), gridPvData);
        });
    }
}