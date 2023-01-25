/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Packages;
using YetaWF.Modules.Pages.Addons;
using YetaWF.Modules.Pages.Components;

namespace YetaWF.Modules.Pages.Endpoints {

    public class ListOfLocalPagesEndpoints : YetaWFEndpoints {

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageRoute(package, typeof(ListOfLocalPagesEndpoints)))
                .RequireAuthorization()
                .AntiForgeryToken()
                .ResourceAuthorize(Info.Resource_AllowListOfLocalPagesAjax);

            group.MapPost(GetEndpoint(package, typeof(ListOfLocalPagesEndpoints), nameof(GridSupport.BrowseGridData)), async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPVData) => {
                return await GridSupport.GetGridPartialAsync(context, null, ListOfLocalPagesEditComponent.GetGridAllUsersModel(), gridPVData);
            });

            group.MapPost(GetEndpoint(package, typeof(ListOfLocalPagesEndpoints), nameof(GridSupport.DisplaySortFilter)), async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPVData) => {
                return await GridSupport.GetGridPartialAsync<ListOfLocalPagesDisplayComponent.Entry>(context, null, ListOfLocalPagesDisplayComponent.GetGridModel(false), gridPVData);
            });

            group.MapPost(GetEndpoint(package, typeof(ListOfLocalPagesEndpoints), nameof(GridSupport.EditSortFilter)), async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPVData) => {
                return await GridSupport.GetGridPartialAsync<ListOfLocalPagesEditComponent.Entry>(context, null, ListOfLocalPagesEditComponent.GetGridModel(false), gridPVData);
            });
        }
    }
}
