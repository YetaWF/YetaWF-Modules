/* Copyright �2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Logging#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using System.Threading.Tasks;
using YetaWF.Modules.Logging.DataProvider;
using Microsoft.AspNetCore.Mvc;

namespace YetaWF.Modules.Logging.Controllers {

    public class LoggingConfigModuleController : ControllerImpl<YetaWF.Modules.Logging.Modules.LoggingConfigModule> {

        public LoggingConfigModuleController() { }

        [Trim]
        public class Model {

            [Caption("Retention (Days)"), Description("Defines the number of days logging information is saved - Logging information that is older than the specified number of days is deleted")]
            [UIHint("IntValue"), Range(1, 99999999), Required]
            public int Days { get; set; }

            public LoggingConfigData GetData(LoggingConfigData data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void SetData(LoggingConfigData data) {
                ObjectSupport.CopyData(data, this);
            }
            public Model() { }
        }

        [AllowGet]
        public async Task<ActionResult> LoggingConfig() {
            using (LoggingConfigDataProvider dataProvider = new LoggingConfigDataProvider()) {
                Model model = new Model { };
                LoggingConfigData data = await dataProvider.GetItemAsync();
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "The logging settings could not be found"));
                model.SetData(data);
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> LoggingConfig_Partial(Model model) {
            using (LoggingConfigDataProvider dataProvider = new LoggingConfigDataProvider()) {
                LoggingConfigData data = await dataProvider.GetItemAsync();// get the original item
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