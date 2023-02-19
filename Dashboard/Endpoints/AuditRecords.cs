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
using YetaWF.Modules.Dashboard.DataProvider;
using YetaWF.Modules.Dashboard.Modules;

namespace YetaWF.Modules.Dashboard.Endpoints;

public class AuditRecordsModuleEndpoints : YetaWFEndpoints {

    internal const string Remove = "Remove";

    private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(AuditRecordsModuleEndpoints), name, defaultValue, parms); }

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(AuditRecordsModuleEndpoints)))
            .RequireAuthorization()
            .AntiForgeryToken();

        group.MapPost(GridSupport.BrowseGridData, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
            AuditRecordsModule module = await GetModuleAsync<AuditRecordsModule>(gridPvData.__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();
            return await GridSupport.GetGridPartialAsync(context, module, module.GetGridModel(), gridPvData);
        });

        group.MapPost($"{Remove}/{{id}}", async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromRoute] int id) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("RemoveItems")) return Results.Unauthorized();
            using (AuditInfoDataProvider dataProvider = new AuditInfoDataProvider()) {
                if (!await dataProvider.RemoveItemAsync(id))
                    throw new Error(__ResStr("cantRemove", "Couldn't remove {0}", id));
                return Reload(ReloadEnum.ModuleParts);
            }
        });
    }
}
