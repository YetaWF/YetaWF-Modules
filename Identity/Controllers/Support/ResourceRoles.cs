/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.Identity.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class ResourceRolesController : YetaWFController {

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> ResourceRolesDisplay_SortFilter(string data, string fieldPrefix, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) {
            return await GridPartialViewAsync<ResourceRolesDisplayComponent.Entry>(ResourceRolesDisplayComponent.GetGridModel(false), data, fieldPrefix, skip, take, sorts, filters);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> ResourceRolesEdit_SortFilter(string data, string fieldPrefix, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) {
            return await GridPartialViewAsync<ResourceRolesEditComponent.Entry>(ResourceRolesEditComponent.GetGridModel(false), data, fieldPrefix, skip, take, sorts, filters);
        }
    }
}
