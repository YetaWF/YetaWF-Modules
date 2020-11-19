/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
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
        public async Task<ActionResult> ResourceRolesDisplay_SortFilter(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync<ResourceRolesDisplayComponent.Entry>(ResourceRolesDisplayComponent.GetGridModel(false), gridPVData);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> ResourceRolesEdit_SortFilter(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync<ResourceRolesEditComponent.Entry>(ResourceRolesEditComponent.GetGridModel(false), gridPVData);
        }
    }
}
