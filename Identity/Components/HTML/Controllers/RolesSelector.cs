/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.Identity.Components;

namespace YetaWF.Modules.Identity.Controllers {

    public class RolesSelectorController : YetaWFController {

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> RolesSelectorDisplay_SortFilter(GridPartialViewData gridPvData) {
            return await GridPartialViewAsync<RolesSelectorDisplayComponent.Entry>(RolesSelectorDisplayComponent.GetGridModel(false, false), gridPvData);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> RolesSelectorEdit_SortFilter(GridPartialViewData gridPvData) {
            return await GridPartialViewAsync<RolesSelectorEditComponent.Entry>(RolesSelectorEditComponent.GetGridModel(false, false), gridPvData);
        }
    }
}
