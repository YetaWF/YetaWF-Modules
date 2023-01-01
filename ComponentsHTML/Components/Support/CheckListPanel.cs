/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.ComponentsHTML.Components;
using Microsoft.AspNetCore.Mvc;

namespace YetaWF.Modules.ComponentsHTML.Controllers {

    /// <summary>
    /// Implements controllers used by the CheckListPanel component.
    /// </summary>
    public class CheckListPanelController : YetaWFController {

        /// <summary>
        /// Called by the client-side grid to sort/filter data in the grid used by the CheckListPanel display component.
        /// </summary>
        /// <param name="gridPVData">Describes the data necessary to render the grid contents.</param>
        /// <returns>Returns a partial view with the grid data.</returns>
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> CheckListPanelDisplay_SortFilter(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync<CheckListPanelDisplayComponent.Entry>(CheckListPanelDisplayComponent.GetGridModel(false), gridPVData);
        }
        /// <summary>
        /// Called by the client-side grid to sort/filter data in the grid used by the CheckListPanel edit component.
        /// </summary>
        /// <param name="gridPVData">Describes the data necessary to render the grid contents.</param>
        /// <returns>Returns a partial view with the grid data.</returns>
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> CheckListPanelEdit_SortFilter(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync<CheckListPanelEditComponent.Entry>(CheckListPanelEditComponent.GetGridModel(false), gridPVData);
        }
    }
}
