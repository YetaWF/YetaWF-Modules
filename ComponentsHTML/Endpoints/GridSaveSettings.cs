/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
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

        internal class GridColumns {
            public Guid? SettingsModuleGuid { get; set; }
            public List<GridColumnEntry> Columns { get; set; } = null!;
        }
        internal class GridColumnEntry {
            public string Name { get; set; } = null!;
            public int Width { get; set; }
        }
        internal class GridHiddenColumns {
            public Guid? SettingsModuleGuid { get; set; }
            public List<string> ColumnsOn { get; set; } = null!;
            public List<string> ColumnsOff { get; set; } = null!;
        }

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageRoute(package, typeof(GridSaveSettingsEndpoints)))
                .AntiForgeryToken();

            group.MapPost(GridSaveColumnWidths, (HttpContext context, [FromBody] GridColumns gridColumns) => {
                if (GridLoadSave.UseGridSettings(gridColumns.SettingsModuleGuid)) {
                    GridLoadSave.GridSavedSettings gridSavedSettings = GridLoadSave.LoadModuleSettings((Guid)gridColumns.SettingsModuleGuid!);
                    foreach (var col in gridColumns.Columns) {
                        if (gridSavedSettings.Columns.ContainsKey(col.Name))
                            gridSavedSettings.Columns[col.Name].Width = col.Width;
                        else
                            gridSavedSettings.Columns.Add(col.Name, new GridDefinition.ColumnInfo() { Width = col.Width });
                    }
                    GridLoadSave.SaveModuleSettings((Guid)gridColumns.SettingsModuleGuid, gridSavedSettings);
                }
                return Results.Ok();
            });

            group.MapPost(GridSaveHiddenColumns, (HttpContext context, [FromBody] GridHiddenColumns gridHiddenColumns) => {
                if (GridLoadSave.UseGridSettings(gridHiddenColumns.SettingsModuleGuid)) {
                    GridLoadSave.GridSavedSettings gridSavedSettings = GridLoadSave.LoadModuleSettings((Guid)gridHiddenColumns.SettingsModuleGuid!);
                    ToggleColumns(gridSavedSettings, gridHiddenColumns.ColumnsOn, true);
                    ToggleColumns(gridSavedSettings, gridHiddenColumns.ColumnsOff, false);
                    GridLoadSave.SaveModuleSettings((Guid)gridHiddenColumns.SettingsModuleGuid, gridSavedSettings);
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
