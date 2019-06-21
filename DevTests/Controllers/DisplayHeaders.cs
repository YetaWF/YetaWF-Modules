/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using System.Collections.Generic;
using YetaWF.Core.Support;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DevTests.Controllers {

    public class DisplayHeadersModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.DisplayHeadersModule> {

        public DisplayHeadersModuleController() { }

        public class DisplayModel {

            [Caption("Headers"), Description("Request Headers")]
            [UIHint("ListOfStrings"), AdditionalMetadata("Delimiter", "<br/>"), ReadOnly]
            public List<string> Headers { get; set; }

            public DisplayModel() {
                Headers = new List<string>();
            }
        }

        [AllowGet]
        public ActionResult DisplayHeaders(int iD) {
            DisplayModel model = new DisplayModel();
            foreach (var x in Request.Headers) {
                model.Headers.Add(x.ToString());
            }
            return View(model);
        }
    }
}
