/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using YetaWF.Core.Addons;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.ComponentsHTML.Endpoints {

    /// <summary>
    /// Endpoints for the ModuleSelection template.
    /// </summary>
    public class ModuleSelectionEndpoints : YetaWFEndpoints {

        internal const string GetPackageModulesNew = "GetPackageModulesNew";
        internal const string GetPackageModulesDesigned = "GetPackageModulesDesigned";
        internal const string GetPackageModulesDesignedFromGuid = "GetPackageModulesDesignedFromGuid";

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageRoute(package, typeof(ModuleSelectionEndpoints)))
                .RequireAuthorization()
                .AntiForgeryToken()
                .ResourceAuthorize(CoreInfo.Resource_ModuleLists);

            // Returns data to replace a dropdownlist's data with new modules given a package name.
            group.MapPost(GetPackageModulesNew, (HttpContext context, string areaName) => {
                ScriptBuilder sb = new ScriptBuilder();
                sb.Append(ModuleSelectionModuleNewEditComponent.RenderReplacementPackageModulesNew(areaName));
                return Results.Json(sb.ToString());
            });

            group.MapPost(GetPackageModulesDesigned, async (HttpContext context, string areaName) => {
                ScriptBuilder sb = new ScriptBuilder();
                sb.Append(await ModuleSelectionPackageExistingEditComponent.RenderReplacementPackageModulesDesignedAsync(areaName));
                return Results.Json(sb.ToString());
            });

            group.MapPost(GetPackageModulesDesignedFromGuid, async (HttpContext context, Guid modGuid) => {
                ScriptBuilder sb = new ScriptBuilder();
                sb.Append(await ModuleSelectionPackageExistingEditComponent.RenderReplacementPackageModulesDesignedAsync(modGuid));
                return Results.Json(sb.ToString());
            });
        }
    }
}
