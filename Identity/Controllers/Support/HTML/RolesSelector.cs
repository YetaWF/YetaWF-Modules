/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.ComponentsHTML.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class RolesSelectorController : YetaWFController {

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> RolesSelectorDisplay_SortFilter(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync<RolesSelectorDisplayComponent.Entry>(RolesSelectorDisplayComponent.GetGridModel(false, false), gridPVData);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> RolesSelectorEdit_SortFilter(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync<RolesSelectorEditComponent.Entry>(RolesSelectorEditComponent.GetGridModel(false, false), gridPVData);
        }
    }
}
