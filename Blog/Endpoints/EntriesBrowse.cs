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
using YetaWF.Modules.Blog.Controllers;
using YetaWF.Modules.Blog.DataProvider;

namespace YetaWF.Modules.Blog.Endpoints {

    public class EntriesBrowseModuleEndpoints : YetaWFEndpoints {

        internal const string Remove = "Remove";

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageRoute(package, typeof(EntriesBrowseModuleEndpoints)))
                .RequireAuthorization()
                .AntiForgeryToken();

            group.MapPost(GridSupport.BrowseGridData, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData, int blogCategory) => {
                ModuleDefinition module = await GetModuleAsync(gridPvData.__ModuleGuid);
                if (!module.IsAuthorized()) return Results.Unauthorized();
                return await GridSupport.GetGridPartialAsync(context, module, EntriesBrowseModuleController.GetGridModel(module, blogCategory), gridPvData);
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
}
