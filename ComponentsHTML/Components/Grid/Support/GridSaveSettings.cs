/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using YetaWF.Core.Models;
using YetaWF.Core.Controllers;
using YetaWF.Modules.ComponentsHTML.Components;
using Microsoft.AspNetCore.Mvc;

namespace YetaWF.Modules.ComponentsHTML.Controllers {

    /// <summary>
    /// Grid template support.
    /// </summary>
    public class GridSaveSettingsController : YetaWFController {

        /// <summary>
        /// Saves a grid's user-defined column widths.
        /// </summary>
        /// <remarks>This is invoked by client-side code via Ajax whenever a grid's column widths change.
        ///
        /// Used in conjunction with client-side code.</remarks>
        [AllowPost]
        public ActionResult GridSaveColumnWidths(Guid settingsModuleGuid, Dictionary<string, int> columns) {
            if (GridLoadSave.UseGridSettings(settingsModuleGuid)) {
                GridLoadSave.GridSavedSettings gridSavedSettings = GridLoadSave.LoadModuleSettings(settingsModuleGuid);
                foreach (var col in columns) {
                    if (gridSavedSettings.Columns.ContainsKey(col.Key))
                        gridSavedSettings.Columns[col.Key].Width = col.Value;
                    else
                        gridSavedSettings.Columns.Add(col.Key, new GridDefinition.ColumnInfo() { Width = col.Value });
                }
                GridLoadSave.SaveModuleSettings(settingsModuleGuid, gridSavedSettings);
            }
            return new EmptyResult();
        }

        /// <summary>
        /// Saves a grid's hidden columns.
        /// </summary>
        /// <param name="settingsModuleGuid">The module Guid used to save the settings.</param>
        /// <param name="columnsOff">A list of columns becoming hidden.</param>
        /// <param name="columnsOn">A list of columns becoming visible.</param>
        /// <remarks>This is invoked by client-side code via Ajax whenever a grid's visible/hidden columns change.
        ///
        /// Used in conjunction with client-side code5</remarks>
        [AllowPost]
        public ActionResult GridSaveHiddenColumns(Guid settingsModuleGuid, List<string> columnsOn, List<string> columnsOff) {
            if (GridLoadSave.UseGridSettings(settingsModuleGuid)) {
                GridLoadSave.GridSavedSettings gridSavedSettings = GridLoadSave.LoadModuleSettings(settingsModuleGuid);
                ToggleColumns(gridSavedSettings, columnsOn, true);
                ToggleColumns(gridSavedSettings, columnsOff, false);
                GridLoadSave.SaveModuleSettings(settingsModuleGuid, gridSavedSettings);
            }
            return new EmptyResult();
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
