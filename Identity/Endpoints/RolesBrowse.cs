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
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Modules;

namespace YetaWF.Modules.Identity.Endpoints;

public class RolesBrowseModuleEndpoints : YetaWFEndpoints {

    internal const string Remove = "Remove";

    private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(RolesBrowseModuleEndpoints), name, defaultValue, parms); }

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(RolesBrowseModuleEndpoints)))
            .RequireAuthorization()
            .AntiForgeryToken();

        group.MapPost(GridSupport.BrowseGridData, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
            RolesBrowseModule module = await GetModuleAsync<RolesBrowseModule>(gridPvData.__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();
            return await GridSupport.GetGridPartialAsync(context, module, module.GetGridModel(), gridPvData);
        });

        group.MapPost(Remove, async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromQuery] string name) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("RemoveRoles")) return Results.Unauthorized();
            if (string.IsNullOrWhiteSpace(name))
                throw new Error(__ResStr("noItem", "No role name specified"));
            using (RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider()) {
                await dataProvider.RemoveItemAsync(name);
                return Reload(ReloadEnum.ModuleParts);
            }
        })
            .ExcludeDemoMode();
    }
}
