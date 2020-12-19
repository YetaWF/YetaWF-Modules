/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/CurrencyConverter#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.CurrencyConverter.DataProvider;
using System;
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
            [UIHint("Text40"), StringLength(ConfigData.MaxAppID), Trim]
            [ExcludeDemoMode]
            public string AppID { get; set; }

            [Caption("Use Https"), Description("Use https to access openexchangerates.org (requires a paid account) - This setting is used for all sites within this YetaWF instance")]
            [UIHint("Boolean")]
            public bool UseHttps { get; set; }

            [Caption("Refresh Interval"), Description("Defines at which interval new currency rates are retrieved - Check the maximum allowed under your rate plan at openexchangerates.org")]
            [UIHint("TimeSpan"), Required, TimeSpanRange("00.00:05:00", "30.00:00:00")]
            public TimeSpan RefreshInterval { get; set; }

            [Category("General"), Caption("openexchangerates.org"), Description("Provides a link to openexchangerates.org to set up an account for all your sites within this YetaWF instance")]
            [UIHint("Url"), ReadOnly]
            public string OpenExchangeRatesUrl { get; set; }

            public ConfigData GetData(ConfigData config) {
                ObjectSupport.CopyData(this, config);
                return config;
            }

            public void SetData(ConfigData config) {
                ObjectSupport.CopyData(config, this);
            }
            public Model() {
                OpenExchangeRatesUrl = "http://openexchangerates.org/";
            }
        }

        [AllowGet]
        public async Task<ActionResult> Config() {
            using (ConfigDataProvider configDP = new ConfigDataProvider()) {
                Model model = new Model { };
                ConfigData data = await configDP.GetItemAsync();
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "Currency converter configuration not found."));
                model.SetData(data);
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> Config_Partial(Model model) {
            using (ConfigDataProvider configDP = new ConfigDataProvider()) {
                ConfigData data = await configDP.GetItemAsync();// get the original item
                if (data == null)
                    throw new Error(this.__ResStr("alreadyDeleted", "The currency converter configuration has been removed and can no longer be updated."));

                if (!ModelState.IsValid)
                    return PartialView(model);

                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display

                await configDP.UpdateConfigAsync(data); // save updated item
                return FormProcessed(model, this.__ResStr("okSaved", "Currency converter configuration saved"));
            }
        }
    }
}