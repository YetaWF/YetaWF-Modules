/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.DevTests.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DevTests.Controllers {

    public class ListOfEmailAddressesController : YetaWFController {

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> ListOfEmailAddressesDisplay_SortFilter(string data, string fieldPrefix, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) {
            return await GridPartialViewAsync<ListOfEmailAddressesDisplayComponent.Entry>(ListOfEmailAddressesDisplayComponent.GetGridModel(false), data, fieldPrefix, skip, take, sorts, filters);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> ListOfEmailAddressesEdit_SortFilter(string data, string fieldPrefix, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) {
            return await GridPartialViewAsync<ListOfEmailAddressesEditComponent.Entry>(ListOfEmailAddressesEditComponent.GetGridModel(false), data, fieldPrefix, skip, take, sorts, filters);
        }
    }
}
