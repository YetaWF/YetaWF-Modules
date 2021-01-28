/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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
        /// <param name="columns">A list of hidden columns. All other columns are considered visible.</param>
        /// <remarks>This is invoked by client-side code via Ajax whenever a grid's visible/hidden columns change.
        ///
        /// Used in conjunction with client-side code.</remarks>
        [AllowPost]
        public ActionResult GridSaveHiddenColumns(Guid settingsModuleGuid, List<string> columns) {
            if (GridLoadSave.UseGridSettings(settingsModuleGuid)) {
                GridLoadSave.GridSavedSettings gridSavedSettings = GridLoadSave.LoadModuleSettings(settingsModuleGuid);
                foreach (string col in gridSavedSettings.Columns.Keys) {
                    gridSavedSettings.Columns[col].Visible = true;
                }
                foreach (var col in columns) {
                    if (gridSavedSettings.Columns.ContainsKey(col))
                        gridSavedSettings.Columns[col].Visible = false;
                    else
                        gridSavedSettings.Columns.Add(col, new GridDefinition.ColumnInfo() { Visible = false });
                }
                GridLoadSave.SaveModuleSettings(settingsModuleGuid, gridSavedSettings);
            }
            return new EmptyResult();
        }
    }
}
