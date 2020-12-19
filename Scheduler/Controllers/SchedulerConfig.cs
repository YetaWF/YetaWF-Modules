/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Scheduler.DataProvider;

namespace YetaWF.Modules.Scheduler.Controllers {

    public class SchedulerConfigModuleController : ControllerImpl<YetaWF.Modules.Scheduler.Modules.SchedulerConfigModule> {

        public SchedulerConfigModuleController() { }

        [Trim]
        public class Model {

            [Caption("Retention (Days)"), Description("Defines the number of days scheduler logging information is saved - Scheduler logging information that is older than the specified number of days is deleted")]
            [UIHint("IntValue"), Range(1, 99999999), Required]
            public int Days { get; set; }

            public SchedulerConfigData GetData(SchedulerConfigData data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void SetData(SchedulerConfigData data) {
                ObjectSupport.CopyData(data, this);
            }
            public Model() { }
        }

        [AllowGet]
        public async Task<ActionResult> SchedulerConfig() {
            using (SchedulerConfigDataProvider dataProvider = new SchedulerConfigDataProvider()) {
                Model model = new Model { };
                SchedulerConfigData data = await dataProvider.GetItemAsync();
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "The logging settings could not be found"));
                model.SetData(data);
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> SchedulerConfig_Partial(Model model) {
            using (SchedulerConfigDataProvider dataProvider = new SchedulerConfigDataProvider()) {
                SchedulerConfigData data = await dataProvider.GetItemAsync();// get the original item
                if (!ModelState.IsValid)
                    return PartialView(model);
                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display
                await dataProvider.UpdateConfigAsync(data);
                return FormProcessed(model, this.__ResStr("okSaved", "Logging settings saved"), NextPage: Manager.ReturnToUrl);
            }
        }
    }
}