/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using Microsoft.AspNetCore.Mvc;
using System;
using YetaWF.Core.Controllers;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.ComponentsHTML.Controllers {

    /// <summary>
    /// Grid template support.
    /// </summary>
    public class GridPanelSaveSettingsController : YetaWFController {

        /// <summary>
        /// Saves a grid's expand/collapse status (panel header only).
        /// </summary>
        /// <remarks>This is invoked by client-side code via Ajax whenever a grid's status changes.
        ///
        /// Used in conjunction with client-side code.</remarks>
        [AllowPost]
        public ActionResult SaveExpandCollapse(Guid settingsModuleGuid, bool expanded) {
            GridLoadSave.GridSavedSettings gridSavedSettings = GridLoadSave.LoadModuleSettings(settingsModuleGuid);
            gridSavedSettings.Collapsed = !expanded;
            GridLoadSave.SaveModuleSettings(settingsModuleGuid, gridSavedSettings);
            return new EmptyResult();
        }
    }
}
