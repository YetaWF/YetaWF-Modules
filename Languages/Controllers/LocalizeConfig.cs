/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Languages#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;

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

            public void SetData() {
                UseLocalizationResources = LocalizationSupport.UseLocalizationResources;
                AbortOnFailure = LocalizationSupport.AbortOnFailure;
            }
            public Model() { }
        }

        [HttpGet]
        public ActionResult LocalizeConfig() {
            Model model = new Model { };
            model.SetData();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LocalizeConfig_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            LocalizationSupport locSupport = new LocalizationSupport();
            locSupport.SetUseLocalizationResources(model.UseLocalizationResources);
            locSupport.SetAbortOnFailure(model.AbortOnFailure);
            return FormProcessed(model, this.__ResStr("okSaved", "Localization settings saved - The site is now restarting"));
        }
    }
}