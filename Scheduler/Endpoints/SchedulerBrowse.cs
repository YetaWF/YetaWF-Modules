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
using YetaWF.Core.Scheduler;
using YetaWF.Core.Support;
using YetaWF.Modules.Scheduler.Controllers;
using YetaWF.Modules.Scheduler.DataProvider;

namespace YetaWF.Modules.Scheduler.Endpoints {

    public class SchedulerBrowseModuleEndpoints : YetaWFEndpoints {

        internal const string RemoveItem = "RemoveItem";
        internal const string RunItem = "RunItem";
        internal const string SchedulerToggle = "SchedulerToggle";

        private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(SchedulerBrowseModuleEndpoints), name, defaultValue, parms); }

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(SchedulerBrowseModuleEndpoints)))
                .RequireAuthorization()
                .AntiForgeryToken();

            group.MapPost(GridSupport.BrowseGridData, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
                ModuleDefinition module = await GetModuleAsync(gridPvData.__ModuleGuid);
                if (!module.IsAuthorized()) return Results.Unauthorized();
                return await GridSupport.GetGridPartialAsync(context, module, SchedulerBrowseModuleController.GetGridModel(module), gridPvData);
            });

            group.MapPost(RemoveItem, async (HttpContext context, [FromQuery] Guid __ModuleGuid, string name) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized("RemoveItems")) return Results.Unauthorized();
                if (string.IsNullOrWhiteSpace(name))
                    throw new Error(__ResStr("noEvent", "No scheduler item name specified"));
                using (SchedulerDataProvider dataProvider = new SchedulerDataProvider()) {
                    await dataProvider.RemoveItemAsync(name);
                    return Reload(ReloadEnum.ModuleParts);
                }
            })
                .ExcludeDemoMode();

            group.MapPost(RunItem, async (HttpContext context, [FromQuery] Guid __ModuleGuid, string name) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized("RunItems")) return Results.Unauthorized();
                if (string.IsNullOrWhiteSpace(name))
                    throw new Error(__ResStr("noEvent", "No scheduler item name specified"));
                await SchedulerSupport.RunItemAsync(name);
                return Reload(ReloadEnum.ModuleParts);
            })
                .ExcludeDemoMode();

            group.MapPost(SchedulerToggle, async (HttpContext context, [FromQuery] Guid __ModuleGuid, bool start) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized()) return Results.Unauthorized();
                using (SchedulerDataProvider dataProvider = new SchedulerDataProvider()) {
                    await dataProvider.SetRunningAsync(start);
                }
                return Reload(ReloadEnum.Page, 
                    PopupText: start ?
                        __ResStr("okStarting", "The scheduler will be started when the site is restarted") :
                        __ResStr("okStopping", "The scheduler will be stopped when the site is restarted"));
            })
                .ExcludeDemoMode();
        }
    }
}
