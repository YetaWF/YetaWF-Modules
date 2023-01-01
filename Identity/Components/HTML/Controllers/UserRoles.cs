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
        public async Task<ActionResult> UserRolesDisplay_SortFilter(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync<UserRolesDisplayComponent.Entry>(UserRolesDisplayComponent.GetGridModel(false), gridPVData);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> UserRolesEdit_SortFilter(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync<UserRolesEditComponent.Entry>(UserRolesEditComponent.GetGridModel(false), gridPVData);
        }
    }
}
