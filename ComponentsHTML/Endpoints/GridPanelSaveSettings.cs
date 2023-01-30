/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Packages;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.ComponentsHTML.Endpoints {

    /// <summary>
    /// Endpoints for the Grid template.
    /// </summary>
    public class GridPanelSaveSettingsEndpoints : YetaWFEndpoints {

        internal const string SaveExpandCollapse = "SaveExpandCollapse";

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(GridPanelSaveSettingsEndpoints)));

            // Saves a grid's expand/collapse status (panel header only).
            group.MapPost(SaveExpandCollapse, (HttpContext context, [FromQuery] Guid? settingsModuleGuid, [FromQuery] bool expanded) => {
                if (GridLoadSave.UseGridSettings(settingsModuleGuid)) {
                    GridLoadSave.GridSavedSettings gridSavedSettings = GridLoadSave.LoadModuleSettings((Guid)settingsModuleGuid!);
                    gridSavedSettings.Collapsed = !expanded;
                    GridLoadSave.SaveModuleSettings((Guid)settingsModuleGuid, gridSavedSettings);
                }
                return Results.Ok();
            });
        }
    }
}
