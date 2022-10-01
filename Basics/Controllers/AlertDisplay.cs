/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

using YetaWF.Core.Controllers;
using YetaWF.Modules.Basics.DataProvider;
using System.Threading.Tasks;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Basics.Controllers {

    public class AlertDisplayModuleController : ControllerImpl<YetaWF.Modules.Basics.Modules.AlertDisplayModule> {

        public AlertDisplayModuleController() { }

        public class DisplayModel {
            public AlertConfig.MessageHandlingEnum MessageHandling { get; set; }
            public string Message { get; set; } = null!;
        }

        [AllowGet]
        public async Task<ActionResult> AlertDisplay() {
            if (Manager.EditMode) return new EmptyResult();
            if (Manager.IsInPopup) return new EmptyResult();

            bool done = Manager.SessionSettings.SiteSettings.GetValue<bool>("YetaWF_Basics_AlertDone", false);
            if (done) return new EmptyResult();

            using (AlertConfigDataProvider dataProvider = new AlertConfigDataProvider()) {

                AlertConfig config = await dataProvider.GetItemAsync();
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
        [AllowPost]
        public ActionResult Off() {
            Manager.SessionSettings.SiteSettings.SetValue<bool>("YetaWF_Basics_AlertDone", true);
            Manager.SessionSettings.SiteSettings.Save();
            return new EmptyResult();
        }
    }
}