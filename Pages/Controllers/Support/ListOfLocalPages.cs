/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Identity;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.Pages.Addons;
using YetaWF.Modules.Pages.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Pages.Controllers {

    public class ListOfLocalPagesController : YetaWFController {

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ResourceAuthorize(Info.Resource_AllowListOfLocalPagesAjax)]
        public async Task<ActionResult> ListOfLocalPagesBrowse_GridData(GridPartialViewData gridPVData /* settingsModuleGuid - not available in templates */) {
            return await GridPartialViewAsync(ListOfLocalPagesEditComponent.GetGridAllUsersModel(), gridPVData);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> ListOfLocalPagesDisplay_SortFilter(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync<ListOfLocalPagesDisplayComponent.Entry>(ListOfLocalPagesDisplayComponent.GetGridModel(false), gridPVData);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> ListOfLocalPagesEdit_SortFilter(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync<ListOfLocalPagesEditComponent.Entry>(ListOfLocalPagesEditComponent.GetGridModel(false), gridPVData);
        }
    }
}
