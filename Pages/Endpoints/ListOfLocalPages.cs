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

        public const string BrowseGridData = "BrowseGridData";
        public const string DisplaySortFilter = "DisplaySortFilter";
        public const string EditSortFilter = "EditSortFilter";

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            endpoints.MapPost(GetEndpoint(package, typeof(ListOfLocalPagesEndpoints), nameof(ListOfLocalPagesEndpoints.BrowseGridData)), async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPVData) => {
                return await GridSupport.GetGridPartialAsync(context, ListOfLocalPagesEditComponent.GetGridAllUsersModel(), gridPVData);
            })
                .RequireAuthorization()
                .AntiForgeryToken()
                .ResourceAuthorize(Info.Resource_AllowListOfLocalPagesAjax);

            endpoints.MapPost(GetEndpoint(package, typeof(ListOfLocalPagesEndpoints), nameof(ListOfLocalPagesEndpoints.DisplaySortFilter)), async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPVData) => {
                return await GridSupport.GetGridPartialAsync<ListOfLocalPagesDisplayComponent.Entry>(context, ListOfLocalPagesDisplayComponent.GetGridModel(false), gridPVData);
            })
                .RequireAuthorization()
                .AntiForgeryToken()
                .ResourceAuthorize(Info.Resource_AllowListOfLocalPagesAjax);

            endpoints.MapPost(GetEndpoint(package, typeof(ListOfLocalPagesEndpoints), nameof(ListOfLocalPagesEndpoints.EditSortFilter)), async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPVData) => {
                return await GridSupport.GetGridPartialAsync<ListOfLocalPagesEditComponent.Entry>(context, ListOfLocalPagesEditComponent.GetGridModel(false), gridPVData);
            })
                .RequireAuthorization()
                .AntiForgeryToken()
                .ResourceAuthorize(Info.Resource_AllowListOfLocalPagesAjax);
        }
    }
}
