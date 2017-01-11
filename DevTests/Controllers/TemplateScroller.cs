/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/DevTests#License */

using System.Collections.Generic;
using System.Web.Mvc;
using YetaWF.Core.Addons;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplateScrollerModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplateScrollerModule> {

        public TemplateScrollerModuleController() { }

        public class Item {
            [UIHint("Image")]
            public string Image { get; set; }
            [UIHint("String")]
            public string Title { get; set; }
            [UIHint("String")]
            public string Summary { get; set; }
        }

        [Trim]
        public class Model {

            // The Scroller template is a core template implementing the overall Scroller
            // The AdditionalMetadata describes the user-defined template used for each item in the Scroller
            [UIHint("Scroller"), ReadOnly, AdditionalMetadata("Template", "YetaWF_DevTests_ScrollerItem")]
            public List<Item> Items { get; set; }

            public Model() {
                Items = new List<Item>();
            }
        }

        [HttpGet]
        public ActionResult TemplateScroller() {
            Model model = new Model { };
            string addonUrl = VersionManager.GetAddOnModuleUrl(AreaRegistration.CurrentPackage.Domain, AreaRegistration.CurrentPackage.Product);
            // generate some random data for the scroller items
            for (int index = 0 ; index < 12 ; ++index) {
                model.Items.Add(new Item {
                    Image = Manager.GetCDNUrl(Manager.CurrentSite.MakeUrl(string.Format("{0}Images/Image{1}.png", addonUrl, index))),
                    Title = string.Format("Item {0}", index),
                    Summary = string.Format("Summary for item {0} - Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. ", index),
                });
            }
            return View(model);
        }
    }
}
