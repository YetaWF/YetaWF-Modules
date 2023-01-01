/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Site;
using System.Threading.Tasks;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplatePropertyListModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplatePropertyListModule> {

        public class SampleSiteDefinition : SiteDefinition {

            public PropertyList.PropertyListStyleEnum Style { get; set; }

            // Override PropertyList definition found in definition file locate at ./Addons./_Main/PropertyLists/TemplatePropertyListModuleController.SampleSiteDefinition
            public Task __PropertyListSetupAsync(PropertyList.PropertyListSetup setup) {
                setup.Style = Style;
                return Task.CompletedTask;
            }
        }

        [Trim]
        public class Model {

            [Caption("Style"), Description("Defines the display style of the property list")]
            [UIHint("Enum")]
            public PropertyList.PropertyListStyleEnum Style { get; set; }

            [Caption(" "), Description(" ")]
            [UIHint("FormButton"), ReadOnly]
            public FormButton ApplyButton { get; set; }

            [Caption("Sample Property List"), Description("")]
            [UIHint("PropertyList")]
            public SampleSiteDefinition Site { get; set; }

            public Model() {
                Site = new SampleSiteDefinition();
                ApplyButton = new FormButton { ButtonType = ButtonTypeEnum.Apply };
            }
        }

        [AllowGet]
        public ActionResult TemplatePropertyList() {
            Model model = new Model {
                Style = PropertyList.PropertyListStyleEnum.BoxedWithHeaders,
            };
            ObjectSupport.CopyData(Manager.CurrentSite, model.Site);
            model.Site.Style = model.Style;
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplatePropertyList_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            model.Site.Style = model.Style;
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }
    }
}
