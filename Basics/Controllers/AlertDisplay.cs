/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Basics#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Modules.Basics.DataProvider;

namespace YetaWF.Modules.Basics.Controllers {

    public class AlertDisplayModuleController : ControllerImpl<YetaWF.Modules.Basics.Modules.AlertDisplayModule> {

        public AlertDisplayModuleController() { }

        public class DisplayModel {
            public AlertConfig.MessageHandlingEnum MessageHandling { get; set; }
            public string Message { get; set; }
        }

        [HttpGet]
        public ActionResult AlertDisplay() {
            if (Manager.EditMode) return new EmptyResult();
            if (Manager.IsInPopup) return new EmptyResult();

            bool done = Manager.SessionSettings.SiteSettings.GetValue<bool>("YetaWF_Basics_AlertDone", false);
            if (done) return new EmptyResult();

            using (AlertConfigDataProvider dataProvider = new AlertConfigDataProvider()) {

                AlertConfig config = dataProvider.GetItem();
                if (config == null || !config.Enabled)
                    return new EmptyResult();

                if (config.MessageHandling == AlertConfig.MessageHandlingEnum.DisplayOnce) {
                    Manager.SessionSettings.SiteSettings.SetValue<bool>("YetaWF_Basics_AlertDone", true);
                    Manager.SessionSettings.SiteSettings.Save();
                }

                DisplayModel model = new DisplayModel() {
                    MessageHandling = config.MessageHandling,
                    Message = config.Message,
                };
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult Off() {
            Manager.SessionSettings.SiteSettings.SetValue<bool>("YetaWF_Basics_AlertDone", true);
            Manager.SessionSettings.SiteSettings.Save();
            return new EmptyResult();
        }
    }
}