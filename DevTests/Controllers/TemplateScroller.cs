/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplateScrollerModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplateScrollerModule> {

        public TemplateScrollerModuleController() { }

        public class ScrollerItem {
            [UIHint("Image")]
            public string Image { get; set; } = null!;
            [UIHint("String")]
            public string Title { get; set; } = null!;
            [UIHint("String")]
            public string Summary { get; set; } = null!;
        }

        [Trim]
        public class Model {

            // The Scroller component is a core component implementing the overall Scroller
            // The AdditionalMetadata describes the user-defined component used for each item in the Scroller
            [UIHint("Scroller"), ReadOnly, AdditionalMetadata("Template", "YetaWF_DevTests_ScrollerItem")]
            public List<ScrollerItem> Items { get; set; }

            public Model() {
                Items = new List<ScrollerItem>();
            }
        }

        [AllowGet]
        public ActionResult TemplateScroller() {
            Model model = new Model { };
            string addonUrl = Package.GetAddOnPackageUrl(AreaRegistration.CurrentPackage.AreaName);
            // generate some random data for the scroller items
            for (int index = 0 ; index < 12 ; ++index) {
                model.Items.Add(new ScrollerItem {
                    Image = Manager.GetCDNUrl(string.Format("{0}Images/image{1}.png", addonUrl, index)),
                    Title = string.Format("Item {0}", index),
                    Summary = string.Format("Summary for item {0} - Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. ", index),
                });
            }
            return View(model);
        }
    }
}
