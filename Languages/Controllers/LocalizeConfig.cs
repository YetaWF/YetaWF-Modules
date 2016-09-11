/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Languages#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Languages.DataProvider;

namespace YetaWF.Modules.Languages.Controllers {

    public class LocalizeConfigModuleController : ControllerImpl<YetaWF.Modules.Languages.Modules.LocalizeConfigModule> {

        public LocalizeConfigModuleController() { }


        [Trim]
        [Footer("Individual web sites can enable/disable localization support (Site Settings). But localization is only available for individiual sites run by this instance of YetaWF if 'Use Localization Resources' is enabled, which applies to all sites.")]
        public class Model {

            [Caption("Use Localization Resources"), Description("Defines whether localization resources are used, otherwise the application built-in text strings are used instead")]
            [TextBelow("If multi-language support is required, this property must be enabled. This property applies to ALL sites run by this instance of YetaWF. The web site is automatically restarted when this property is changed.")]
            [UIHint("Boolean")]
            public bool UseLocalizationResources { get; set; }

            [Caption("Failure Mode"), Description("Defines whether a missing localization resource string causes an exception (failure) or whether the built-in string is used instead")]
            [TextBelow("This property is only used if 'Use Localization Resources' is selected. It is typically enabled for production systems and disabled for development/testing.")]
            [UIHint("Boolean")]
            public bool AbortOnFailure { get; set; }

            [ExcludeDemoMode]
            [Caption("Google Translate API Key"), Description("Defines the Google Cloud Platform / Google Translate API key, which is used when translating localization resources into other languages - You can obtain an API key from the Google Cloud Platform service - This is not a free service")]
            [HelpLink("https://cloud.google.com/translate/docs/")]
            [UIHint("Text80"), StringLength(LocalizeConfigData.MaxGoogleTranslateAPIKey), Trim]
            public string GoogleTranslateAPIKey { get; set; }

            [Caption("Google Translate App Name"), Description("Defines the name of your application performing translations")]
            [HelpLink("https://cloud.google.com/translate/docs/")]
            [UIHint("Text80"), StringLength(LocalizeConfigData.MaxGoogleTranslateAppName), RequiredIfSupplied("GoogleTranslateAPIKey"), Trim]
            public string GoogleTranslateAppName { get; set; }

            public LocalizeConfigData GetData(LocalizeConfigData data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void SetData(LocalizeConfigData data) {
                ObjectSupport.CopyData(data, this);
                UseLocalizationResources = LocalizationSupport.UseLocalizationResources;
                AbortOnFailure = LocalizationSupport.AbortOnFailure;
            }
            public Model() { }
        }

        [HttpGet]
        public ActionResult LocalizeConfig() {
            using (LocalizeConfigDataProvider dataProvider = new LocalizeConfigDataProvider()) {
                Model model = new Model { };
                LocalizeConfigData data = dataProvider.GetItem();
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "The localization settings could not be found"));
                model.SetData(data);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult LocalizeConfig_Partial(Model model) {
            using (LocalizeConfigDataProvider dataProvider = new LocalizeConfigDataProvider()) {
                LocalizeConfigData data = dataProvider.GetItem();// get the original item
                if (!ModelState.IsValid)
                    return PartialView(model);
                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display

                LocalizationSupport locSupport = new LocalizationSupport();
                locSupport.SetUseLocalizationResources(model.UseLocalizationResources);
                locSupport.SetAbortOnFailure(model.AbortOnFailure);
                dataProvider.UpdateConfig(data);
            }
            return FormProcessed(model, this.__ResStr("okSaved", "Localization settings saved - The site is now restarting"));
        }
    }
}