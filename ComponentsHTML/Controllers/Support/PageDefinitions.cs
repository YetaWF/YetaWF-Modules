/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
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
        /// <param name="data">A string containing JSON with the grid data.</param>
        /// <param name="fieldPrefix">The name prefix used for fields in the grid.</param>
        /// <param name="skip">The number of records to skip (grid paging).</param>
        /// <param name="take">The number of records to record.</param>
        /// <param name="sorts">The sort criteria.</param>
        /// <param name="filters">The filtering criteria.</param>
        /// <returns>Returns a partial view with the grid data.</returns>
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> PageDefinitionsDisplay_SortFilter(string data, string fieldPrefix, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) {
            return await GridPartialViewAsync<PageDefinitionsDisplayComponent.Entry>(PageDefinitionsDisplayComponent.GetGridModel(false), data, fieldPrefix, skip, take, sorts, filters);
        }
    }
}
