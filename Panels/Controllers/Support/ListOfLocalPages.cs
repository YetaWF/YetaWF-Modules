/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Identity;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.Panels.Addons;
using YetaWF.Modules.Panels.Components;

namespace YetaWF.Modules.Panels.Controllers {

    public class ListOfLocalPagesController : YetaWFController {

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ResourceAuthorize(Info.Resource_AllowListOfLocalPagesAjax)]
        public async Task<ActionResult> ListOfLocalPagesBrowse_GridData(GridPartialViewData gridPvData /* settingsModuleGuid - not available in templates */) {
            return await GridPartialViewAsync(ListOfLocalPagesEditComponent.GetGridAllUsersModel(), gridPvData);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> ListOfLocalPagesEdit_SortFilter(GridPartialViewData gridPvData) {
            return await GridPartialViewAsync<ListOfLocalPagesEditComponent.Entry>(ListOfLocalPagesEditComponent.GetGridModel(false), gridPvData);
        }
    }
}
