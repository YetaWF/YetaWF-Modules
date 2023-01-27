/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.Identity.Components;
using Microsoft.AspNetCore.Mvc;

namespace YetaWF.Modules.Identity.Controllers {

    public class ResourceRolesController : YetaWFController {

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> ResourceRolesDisplay_SortFilter(GridPartialViewData gridPvData) {
            return await GridPartialViewAsync<ResourceRolesDisplayComponent.Entry>(ResourceRolesDisplayComponent.GetGridModel(false), gridPvData);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> ResourceRolesEdit_SortFilter(GridPartialViewData gridPvData) {
            return await GridPartialViewAsync<ResourceRolesEditComponent.Entry>(ResourceRolesEditComponent.GetGridModel(false), gridPvData);
        }
    }
}
