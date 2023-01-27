/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.Identity.Components;
using Microsoft.AspNetCore.Mvc;

namespace YetaWF.Modules.Identity.Controllers {

    public class UserRolesController : YetaWFController {

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> UserRolesDisplay_SortFilter(GridPartialViewData gridPvData) {
            return await GridPartialViewAsync<UserRolesDisplayComponent.Entry>(UserRolesDisplayComponent.GetGridModel(false), gridPvData);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> UserRolesEdit_SortFilter(GridPartialViewData gridPvData) {
            return await GridPartialViewAsync<UserRolesEditComponent.Entry>(UserRolesEditComponent.GetGridModel(false), gridPvData);
        }
    }
}
