/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using Microsoft.AspNetCore.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Modules.Panels.DataProvider;

namespace YetaWF.Modules.Panels.Controllers {

    public class PageBarInfoController : YetaWFController {

        [AllowPost]
        public ActionResult SaveExpandCollapse(bool expanded) {
            PageBarDataProvider.SaveExpanded(expanded);
            return new EmptyResult();
        }
    }
}
