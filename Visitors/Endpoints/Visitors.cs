/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Modules.Visitors.Controllers;
using YetaWF.Modules.Visitors.Scheduler;

namespace YetaWF.Modules.Visitors.Endpoints {

    public class VisitorsModuleEndpoints : YetaWFEndpoints {

        internal const string UpdateGeoLocation = "UpdateGeoLocation";

        private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(VisitorsModuleEndpoints), name, defaultValue, parms); }

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(VisitorsModuleEndpoints)))
                .RequireAuthorization()
                .AntiForgeryToken();

            group.MapPost(GridSupport.BrowseGridData, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
                ModuleDefinition module = await GetModuleAsync(gridPvData.__ModuleGuid);
                if (!module.IsAuthorized()) return Results.Unauthorized();
                return await GridSupport.GetGridPartialAsync(context, module, VisitorsModuleController.GetGridModel(module), gridPvData);
            });

            group.MapPost(UpdateGeoLocation, async (HttpContext context, [FromQuery] Guid __ModuleGuid) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized("UpdateGeoLocation")) return Results.Unauthorized();
                AddVisitorGeoLocation geo = new AddVisitorGeoLocation();
                List<string> errorList = new List<string>();
                await geo.AddGeoLocationAsync(errorList);
                return Reload(ReloadEnum.ModuleParts);
            })
                .ExcludeDemoMode();
        }
    }
}
