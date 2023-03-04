/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Modules.Identity.Components;

namespace YetaWF.Modules.Identity.Endpoints;

public class RolesSelectorEndpoints : YetaWFEndpoints {

    internal const string AddUserName = "AddUserName";

    private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(RolesSelectorEndpoints), name, defaultValue, parms); }

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(RolesSelectorEndpoints)))
            .RequireAuthorization()
            .AntiForgeryToken();

        group.MapPost(GridSupport.DisplaySortFilter, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
            return await GridSupport.GetGridPartialAsync<RolesSelectorDisplayComponent.Entry>(context, null,RolesSelectorDisplayComponent.GetGridModel(false, false), gridPvData);
        });

        group.MapPost(GridSupport.EditSortFilter, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
            return await GridSupport.GetGridPartialAsync<RolesSelectorEditComponent.Entry>(context, null, RolesSelectorEditComponent.GetGridModel(false, false), gridPvData);
        });
    }
}
