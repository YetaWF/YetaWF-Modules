/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.ComponentsHTML.Endpoints {

    /// <summary>
    /// Endpoints for the Grid template.
    /// </summary>
    public class GridSaveSettingsEndpoints : YetaWFEndpoints {

        internal const string GridSaveColumnWidths = "GridSaveColumnWidths";
        internal const string GridSaveHiddenColumns = "GridSaveHiddenColumns";

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageRoute(package, typeof(GridSaveSettingsEndpoints)));

            group.MapPost(GridSaveColumnWidths, (HttpContext context, [FromQuery] Guid? settingsModuleGuid, [FromQuery] Dictionary<string, int> columns) => {
                if (GridLoadSave.UseGridSettings(settingsModuleGuid)) {
                    GridLoadSave.GridSavedSettings gridSavedSettings = GridLoadSave.LoadModuleSettings((Guid)settingsModuleGuid!);
                    foreach (var col in columns) {
                        if (gridSavedSettings.Columns.ContainsKey(col.Key))
                            gridSavedSettings.Columns[col.Key].Width = col.Value;
                        else
                            gridSavedSettings.Columns.Add(col.Key, new GridDefinition.ColumnInfo() { Width = col.Value });
                    }
                    GridLoadSave.SaveModuleSettings((Guid)settingsModuleGuid, gridSavedSettings);
                }
                return Results.Ok();
            });

            group.MapPost(GridSaveHiddenColumns, (HttpContext context, [FromQuery] Guid? settingsModuleGuid, [FromQuery] List<string> columnsOn, [FromQuery] List<string> columnsOff) => {
                if (GridLoadSave.UseGridSettings(settingsModuleGuid)) {
                    GridLoadSave.GridSavedSettings gridSavedSettings = GridLoadSave.LoadModuleSettings((Guid)settingsModuleGuid!);
                    ToggleColumns(gridSavedSettings, columnsOn, true);
                    ToggleColumns(gridSavedSettings, columnsOff, false);
                    GridLoadSave.SaveModuleSettings((Guid)settingsModuleGuid, gridSavedSettings);
                }
                return Results.Ok();
            });
        }

        private static void ToggleColumns(GridLoadSave.GridSavedSettings gridSavedSettings, List<string> columns, bool on) {
            foreach (var col in columns) {
                if (gridSavedSettings.Columns.ContainsKey(col))
                    gridSavedSettings.Columns[col].Visible = on;
                else
                    gridSavedSettings.Columns.Add(col, new GridDefinition.ColumnInfo() { Visible = on });
            }
        }
    }
}
