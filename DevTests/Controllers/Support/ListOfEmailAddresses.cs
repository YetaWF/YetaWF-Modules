/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.DevTests.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DevTests.Controllers {

    public class ListOfEmailAddressesController : YetaWFController {

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> ListOfEmailAddressesDisplay_SortFilter(GridPartialViewData gridPvData) {
            return await GridPartialViewAsync<ListOfEmailAddressesDisplayComponent.Entry>(ListOfEmailAddressesDisplayComponent.GetGridModel(false), gridPvData);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> ListOfEmailAddressesEdit_SortFilter(GridPartialViewData gridPvData) {
            return await GridPartialViewAsync<ListOfEmailAddressesEditComponent.Entry>(ListOfEmailAddressesEditComponent.GetGridModel(false), gridPvData);
        }
    }
}
