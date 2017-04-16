/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/CurrencyConverter#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.CurrencyConverter.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.CurrencyConverter.Controllers {

    public class ConfigModuleController : ControllerImpl<YetaWF.Modules.CurrencyConverter.Modules.ConfigModule> {

        public ConfigModuleController() { }

        [Trim]
        public class Model {

            [Caption("App ID"), Description("App ID used by openexchangerates.org to identify your account - an account is needed to retrieve currency exchange rates - This account is used for all sites within this YetaWF instance")]
            [UIHint("Text40"), StringLength(ConfigData.MaxAppID), Required, Trim]
            [ExcludeDemoMode]
            public string AppID { get; set; }

            [Caption("Use Https"), Description("Use https to access openexchangerates.org (requires a paid account) - This setting is used for all sites within this YetaWF instance")]
            [UIHint("Boolean")]
            public bool UseHttps { get; set; }

            [Category("General"), Caption("openexchangerates.org"), Description("Provides a link to openexchangerates.org to set up an account for all your site within this YetaWF instance")]
            [UIHint("Url"), ReadOnly]
            public string OpenExchangeRatesUrl { get; set; }

            public ConfigData GetData(ConfigData data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }

            public void SetData(ConfigData data) {
                ObjectSupport.CopyData(data, this);
            }
            public Model() {
                AppID = "";
                UseHttps = false;
                OpenExchangeRatesUrl = "http://openexchangerates.org/";
            }
        }

        [HttpGet]
        public ActionResult Config() {
            using (ConfigDataProvider dataProvider = new ConfigDataProvider()) {
                Model model = new Model { };
                ConfigData data = dataProvider.GetItem();
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "Currency converter configuration not found."));
                model.SetData(data);
                return View(model);
            }
        }

        [HttpPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult Config_Partial(Model model) {
            using (ConfigDataProvider dataProvider = new ConfigDataProvider()) {
                ConfigData data = dataProvider.GetItem();// get the original item
                if (data == null)
                    ModelState.AddModelError("Key", this.__ResStr("alreadyDeleted", "The currency converter configuration has been removed and can no longer be updated."));

                if (!ModelState.IsValid)
                    return PartialView(model);

                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display

                dataProvider.UpdateConfig(data); // save updated item
                return FormProcessed(model, this.__ResStr("okSaved", "Currency converter configuration saved"));
            }
        }
    }
}