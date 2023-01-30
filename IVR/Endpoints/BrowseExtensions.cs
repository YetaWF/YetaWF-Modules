/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Softelvdm.Modules.IVR.Controllers;
using Softelvdm.Modules.IVR.DataProvider;
using System;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace Softelvdm.Modules.IVR.Endpoints {

    public class BrowseExtensionsModuleEndpoints : YetaWFEndpoints {

        internal const string Remove = "Remove";

        private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(BrowseExtensionsModuleEndpoints), name, defaultValue, parms); }

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(BrowseExtensionsModuleEndpoints)))
                .RequireAuthorization()
                .AntiForgeryToken();

            group.MapPost(GridSupport.BrowseGridData, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
                ModuleDefinition module = await GetModuleAsync(gridPvData.__ModuleGuid);
                if (!module.IsAuthorized()) return Results.Unauthorized();
                return await GridSupport.GetGridPartialAsync(context, module, BrowseExtensionsModuleController.GetGridModel(module), gridPvData);
            });

            group.MapPost($"{Remove}/{{id}}", async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromRoute] int id) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized("RemoveItems")) return Results.Unauthorized();
                using (ExtensionEntryDataProvider dataProvider = new ExtensionEntryDataProvider()) {
                    if (!await dataProvider.RemoveItemByIdentityAsync(id))
                        throw new Error(__ResStr("cantRemove", "Couldn't remove extension with id {0}", id));
                    return Reload(ReloadEnum.ModuleParts);
                }
            });
        }
    }
}
