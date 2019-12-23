/* Copyright �2020 Softel vdm, Inc.. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

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
        public ActionResult DisplayHeaders() {
            DisplayModel model = new DisplayModel();
#if MVC6
            foreach (var hdr in Request.Headers) {
                model.Headers.Add(hdr.ToString());
            }
#else
            foreach (var hdr in Request.Headers.Keys) {
                string key = (string)hdr;
                model.Headers.Add($"[{key}, {Request.Headers[key]}]");
            }
#endif
            return View(model);
        }
    }
}
