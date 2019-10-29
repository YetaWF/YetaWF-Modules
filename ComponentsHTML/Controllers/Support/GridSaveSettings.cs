/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using YetaWF.Core.Models;
using YetaWF.Core.Controllers;
using YetaWF.Modules.ComponentsHTML.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

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
            GridLoadSave.GridSavedSettings gridSavedSettings = GridLoadSave.LoadModuleSettings(settingsModuleGuid);
            foreach (var col in columns) {
                if (gridSavedSettings.Columns.ContainsKey(col.Key))
                    gridSavedSettings.Columns[col.Key].Width = col.Value;
                else
                    gridSavedSettings.Columns.Add(col.Key, new GridDefinition.ColumnInfo() { Width = col.Value });
            }
            GridLoadSave.SaveModuleSettings(settingsModuleGuid, gridSavedSettings);
            return new EmptyResult();
        }
    }
}
