/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Identity;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.Identity.Addons;
using YetaWF.Modules.Identity.Components;
using Microsoft.AspNetCore.Mvc;

namespace YetaWF.Modules.Identity.Controllers {

    public class UserIdController : YetaWFController {

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ResourceAuthorize(Info.Resource_AllowUserIdAjax)]
        public async Task<ActionResult> UsersBrowse_GridData(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync(UserIdEditComponent.GetGridAllUsersModel(false), gridPVData);
        }
    }
}
