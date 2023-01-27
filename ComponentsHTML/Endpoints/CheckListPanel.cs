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
    /// Implements endpoints used by the CheckListPanel component.
    /// </summary>
    public class CheckListPanelEndpoints : YetaWFEndpoints {

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageRoute(package, typeof(CheckListPanelEndpoints)))
                .AntiForgeryToken();

            group.MapPost(GridSupport.DisplaySortFilter, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
                return await GridSupport.GetGridPartialAsync<CheckListPanelDisplayComponent.Entry>(context, null, CheckListPanelDisplayComponent.GetGridModel(false), gridPvData);
            });
            group.MapPost(GridSupport.EditSortFilter, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
                return await GridSupport.GetGridPartialAsync<CheckListPanelEditComponent.Entry>(context, null, CheckListPanelEditComponent.GetGridModel(false), gridPvData);
            });
        }
    }
}
