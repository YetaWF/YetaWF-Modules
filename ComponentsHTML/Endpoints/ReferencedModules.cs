/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Packages;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.ComponentsHTML.Endpoints {

    /// <summary>
    /// Implements endpoints used by the ReferencedModules component.
    /// </summary>
    public class ReferencedModulesEndpoints : YetaWFEndpoints {

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(ReferencedModulesEndpoints)))
                .RequireAuthorization()
                .AntiForgeryToken();

            group.MapPost(GridSupport.DisplaySortFilter, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
                return await GridSupport.GetGridPartialAsync<ReferencedModulesDisplayComponent.Entry>(context, null, ReferencedModulesDisplayComponent.GetGridModel(false), gridPvData);
            });

            group.MapPost(GridSupport.EditSortFilter, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
                return await GridSupport.GetGridPartialAsync<ReferencedModulesEditComponent.Entry>(context, null, ReferencedModulesEditComponent.GetGridModel(false), gridPvData);
            });
        }
    }
}
