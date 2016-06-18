/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/AddThis#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Modules.AddThis.DataProvider;

namespace YetaWF.Modules.AddThis.Controllers {

    public class SharingSidebarModuleController : ControllerImpl<YetaWF.Modules.AddThis.Modules.SharingSidebarModule> {

        public SharingSidebarModuleController() { }

        public class DisplayModel {

            public string Code { get; set; }

        }

        [HttpGet]
        public ActionResult SharingSidebar() {
            ConfigData config = ConfigDataDataProvider.GetConfig();
            if (string.IsNullOrWhiteSpace(config.Code)) return new EmptyResult();
            DisplayModel model = new DisplayModel() {
                Code = config.Code,
            };
            return View(model);
        }
    }
}