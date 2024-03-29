/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Languages.DataProvider;
using YetaWF.Core.Audit;
using System;
using YetaWF.Core;
using Microsoft.AspNetCore.Mvc;

namespace YetaWF.Modules.Languages.Controllers {

    public class LocalizeConfigModuleController : ControllerImpl<YetaWF.Modules.Languages.Modules.LocalizeConfigModule> {

        public LocalizeConfigModuleController() { }

        [Trim]
        [Footer("Individual web sites can enable/disable localization support (Site Settings). But localization is only available for individual sites run by this instance of YetaWF if 'Use Localization Resources' is enabled, which applies to all sites.")]
        public class Model {

            [Caption("Use Localization Resources"), Description("Defines whether localization resources are used, otherwise the application built-in text strings are used instead")]
            [TextBelow("If multi-language support is required, this property must be enabled. This property applies to ALL sites run by this instance of YetaWF.")]
            [UIHint("Boolean")]
            public bool UseLocalizationResources { get; set; }

            [Caption("Failure Mode"), Description("Defines whether a missing localization resource string causes an exception (failure) or whether the built-in string is used instead")]
            [TextBelow("This property is only used if 'Use Localization Resources' is selected. It is typically enabled for development/testing and disabled for production systems.")]
            [UIHint("Boolean")]
            public bool AbortOnFailure { get; set; }

            [Caption("Translation Service"), Description("Defines the translation service used to translate localization resources")]
            [UIHint("Enum")]
            public LocalizeConfigData.TranslationServiceEnum TranslationService { get; set; }

            [Caption("Endpoint"), Description("Defines the endpoint used for Text Translation - Provided by Microsoft when registering your application - This is not a free service, although there are limited free accounts")]
            [HelpLink("https://docs.microsoft.com/en-us/azure/cognitive-services/translator/text-translation-overview")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Remote), StringLength(Globals.MaxUrl), Trim]
            [RequiredIf(nameof(TranslationService), LocalizeConfigData.TranslationServiceEnum.MicrosoftTranslator)]
            [ProcessIf(nameof(TranslationService), LocalizeConfigData.TranslationServiceEnum.MicrosoftTranslator)]
            public string? MSTextTranslationUrl { get; set; }

            [Caption("Location/Region"), Description("Defines the location (or region) of your resource. You may need to use this field when making calls to this API. - Provided by Microsoft when registering your application - This is not a free service, although there are limited free accounts")]
            [UIHint("Text80"), StringLength(LocalizeConfigData.MaxMSRegion)]
            [RequiredIf(nameof(TranslationService), LocalizeConfigData.TranslationServiceEnum.MicrosoftTranslator), Trim]
            [ProcessIf(nameof(TranslationService), LocalizeConfigData.TranslationServiceEnum.MicrosoftTranslator)]
            public string? MSTextTranslationRegion { get; set; }

            [ExcludeDemoMode]
            [Caption("Key"), Description("Defines the API Key of your application performing translations - Provided by Microsoft when registering your application - This is not a free service, although there are limited free accounts")]
            [UIHint("Text80"), StringLength(LocalizeConfigData.MaxMSClientKey)]
            [RequiredIf(nameof(TranslationService), LocalizeConfigData.TranslationServiceEnum.MicrosoftTranslator), Trim]
            [ProcessIf(nameof(TranslationService), LocalizeConfigData.TranslationServiceEnum.MicrosoftTranslator)]
            public string? MSClientKey { get; set; }

            [Caption("Request Limit"), Description("Defines the hourly quota that can be consumed evenly throughout the hour, expressed in Millions of characters per hour - Provided by Microsoft when registering your application - This is not a free service, although there are limited free accounts")]
            [HelpLink("https://docs.microsoft.com/en-us/azure/cognitive-services/translator/request-limits")]
            [TextBelow("Millions of Characters Per Hour")]
            [UIHint("IntValue4"), Range(0, 999)]
            [RequiredIf(nameof(TranslationService), LocalizeConfigData.TranslationServiceEnum.MicrosoftTranslator), Trim]
            [ProcessIf(nameof(TranslationService), LocalizeConfigData.TranslationServiceEnum.MicrosoftTranslator)]
            public int? MSRequestLimit { get; set; }

            [ExcludeDemoMode]
            [Caption("Translate API Key"), Description("Defines the Google Cloud Platform / Google Translate API key, which is used when translating localization resources into other languages - You can obtain an API key from the Google Cloud Platform service - This is not a free service")]
            [HelpLink("https://cloud.google.com/translate/docs/setup")]
            [UIHint("Text80"), StringLength(LocalizeConfigData.MaxGoogleTranslateAPIKey)]
            [RequiredIf(nameof(TranslationService), LocalizeConfigData.TranslationServiceEnum.GoogleTranslate), Trim]
            [ProcessIf(nameof(TranslationService), LocalizeConfigData.TranslationServiceEnum.GoogleTranslate)]
            public string? GoogleTranslateAPIKey { get; set; }

            [ExcludeDemoMode]
            [Caption("Translate App Name"), Description("Defines the name of your application performing translations - This is not a free service")]
            [HelpLink("https://cloud.google.com/translate/docs/setup")]
            [UIHint("Text80"), StringLength(LocalizeConfigData.MaxGoogleTranslateAppName)]
            [RequiredIf(nameof(TranslationService), LocalizeConfigData.TranslationServiceEnum.GoogleTranslate), Trim]
            [ProcessIf(nameof(TranslationService), LocalizeConfigData.TranslationServiceEnum.GoogleTranslate)]
            public string? GoogleTranslateAppName { get; set; }

            public LocalizeConfigData GetData(LocalizeConfigData data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void UpdateData(LocalizeConfigData data) {
                UseLocalizationResources = LocalizationSupport.UseLocalizationResources;
                AbortOnFailure = LocalizationSupport.AbortOnFailure;
            }
            public void SetData(LocalizeConfigData data) {
                ObjectSupport.CopyData(data, this);
            }
            public Model() { }
        }

        [AllowGet]
        public async Task<ActionResult> LocalizeConfig() {
            using (LocalizeConfigDataProvider dataProvider = new LocalizeConfigDataProvider()) {
                Model model = new Model { };
                LocalizeConfigData data = await dataProvider.GetItemAsync();
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "The localization settings could not be found"));
                model.SetData(data);
                model.UpdateData(data);
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> LocalizeConfig_Partial(Model model) {
            using (LocalizeConfigDataProvider dataProvider = new LocalizeConfigDataProvider()) {
                LocalizeConfigData? origData = YetaWF.Core.Audit.Auditing.Active ? await dataProvider.GetItemAsync() : null;
                LocalizeConfigData data = await dataProvider.GetItemAsync();// get the original item
                if (!ModelState.IsValid)
                    return PartialView(model);
                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display

                LocalizationSupport locSupport = new LocalizationSupport();
                await locSupport.SetUseLocalizationResourcesAsync(model.UseLocalizationResources);
                await locSupport.SetAbortOnFailureAsync(model.AbortOnFailure);
                await dataProvider.UpdateConfigAsync(data);

                await Auditing.AddAuditAsync($"{nameof(LocalizeConfigModuleController)}.{nameof(LocalizeConfig)}", null, Guid.Empty, $"Localization",
                    DataBefore: origData,
                    DataAfter: data,
                    RequiresRestart: true
                );
            }
            return FormProcessed(model, this.__ResStr("okSaved", "Localization settings saved - These settings won't take effect until the site is restarted"));
        }
    }
}