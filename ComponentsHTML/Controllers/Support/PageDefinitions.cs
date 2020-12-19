/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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
    /// Implements controllers used by the PageDefinitions component.
    /// </summary>
    public class PageDefinitionsController : YetaWFController {

        /// <summary>
        /// Called by the client-side grid to sort/filter data in the grid used by the PageDefinitions display component.
        /// </summary>
        /// <param name="gridPVData">Describes the data necessary to render the grid contents.</param>
        /// <returns>Returns a partial view with the grid data.</returns>
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> PageDefinitionsDisplay_SortFilter(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync<PageDefinitionsDisplayComponent.Entry>(PageDefinitionsDisplayComponent.GetGridModel(false), gridPVData);
        }
    }
}
