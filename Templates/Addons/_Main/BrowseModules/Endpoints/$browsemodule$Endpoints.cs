using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;
using YetaWF.Core.Addons;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using $companynamespace$.Modules.$projectnamespace$.DataProvider;
using $companynamespace$.Modules.$projectnamespace$.Modules;


namespace $companynamespace$.Modules.$projectnamespace$.Endpoints;

public class $browsemodule$ModuleEndpoints : YetaWFEndpoints {

    internal const string Remove = "Remove";

    private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof($browsemodule$ModuleEndpoints), name, defaultValue, parms); }

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof($browsemodule$ModuleEndpoints)))
            .RequireAuthorization()
            .AntiForgeryToken();

        group.MapPost(GridSupport.BrowseGridData, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
            $browsemodule$Module module = await GetModuleAsync<$browsemodule$Module>(gridPvData.__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();
            return await GridSupport.GetGridPartialAsync(context, module, module.GetGridModel(), gridPvData);
        });

        group.MapPost(Remove, async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromQuery] $modelkey$ $modelkeynamelower$) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("RemoveItems")) return Results.Unauthorized();
            using ($modelname$DataProvider $modelname$DP = new $modelname$DataProvider()) {
                if (!await $modelname$DP.RemoveItemAsync($modelkeynamelower$))
                    throw new Error(__ResStr("cantRemove", "Couldn't remove {0}", $modelkeynamelower$));
                return Reload(ReloadEnum.ModuleParts);
            }
        })
            .ExcludeDemoMode();
    }
}
