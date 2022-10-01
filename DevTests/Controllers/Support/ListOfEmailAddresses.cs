/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

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
        public async Task<ActionResult> ListOfEmailAddressesDisplay_SortFilter(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync<ListOfEmailAddressesDisplayComponent.Entry>(ListOfEmailAddressesDisplayComponent.GetGridModel(false), gridPVData);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> ListOfEmailAddressesEdit_SortFilter(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync<ListOfEmailAddressesEditComponent.Entry>(ListOfEmailAddressesEditComponent.GetGridModel(false), gridPVData);
        }
    }
}
