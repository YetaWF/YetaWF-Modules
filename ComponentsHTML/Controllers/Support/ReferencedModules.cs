/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.ComponentsHTML.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.ComponentsHTML.Controllers {

    /// <summary>
    /// Implements controllers used by the ReferencedModules component.
    /// </summary>
    public class ReferencedModulesController : YetaWFController {

        /// <summary>
        /// Called by the client-side grid to sort/filter data in the grid used by the ReferencedModules display component.
        /// </summary>
        /// <param name="gridPVData">Describes the data necessary to render the grid contents.</param>
        /// <returns>Returns a partial view with the grid data.</returns>
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> ReferencedModulesDisplay_SortFilter(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync<ReferencedModulesDisplayComponent.Entry>(ReferencedModulesDisplayComponent.GetGridModel(false), gridPVData);
        }
        /// <summary>
        /// Called by the client-side grid to sort/filter data in the grid used by the ReferencedModules edit component.
        /// </summary>
        /// <param name="gridPVData">Describes the data necessary to render the grid contents.</param>
        /// <returns>Returns a partial view with the grid data.</returns>
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> ReferencedModulesEdit_SortFilter(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync<ReferencedModulesEditComponent.Entry>(ReferencedModulesEditComponent.GetGridModel(false), gridPVData);
        }
    }
}
