/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/AddThis#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Modules.AddThis.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.AddThis.Controllers {

    public class SharingSidebarModuleController : ControllerImpl<YetaWF.Modules.AddThis.Modules.SharingSidebarModule> {

        public SharingSidebarModuleController() { }

        public class DisplayModel {

            public string? Code { get; set; } 

        }

        [AllowGet]
        public async Task<ActionResult> SharingSidebar() {
            ConfigData config = await ConfigDataProvider.GetConfigAsync();
            if (string.IsNullOrWhiteSpace(config.Code)) return new EmptyResult();
            DisplayModel model = new DisplayModel() {
                Code = config.Code,
            };
            return View(model);
        }
    }
}