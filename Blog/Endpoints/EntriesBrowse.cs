/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Modules.Blog.DataProvider;
using YetaWF.Modules.Blog.Modules;

namespace YetaWF.Modules.Blog.Endpoints;

public class EntriesBrowseModuleEndpoints : YetaWFEndpoints {

    internal const string Remove = "Remove";

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(EntriesBrowseModuleEndpoints)))
            .RequireAuthorization()
            .AntiForgeryToken();

        group.MapPost(GridSupport.BrowseGridData, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData, int blogCategory) => {
            EntriesBrowseModule module = await GetModuleAsync<EntriesBrowseModule>(gridPvData.__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();
            return await GridSupport.GetGridPartialAsync(context, module, module.GetGridModel(blogCategory), gridPvData);
        });

        group.MapPost(Remove, async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromQuery] int blogEntry) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("RemoveItems")) return Results.Unauthorized();
            using (BlogEntryDataProvider dataProvider = new BlogEntryDataProvider()) {
                await dataProvider.RemoveItemAsync(blogEntry);
                return Reload(ReloadEnum.ModuleParts);
            }
        });
    }
}
