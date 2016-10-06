/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Blog#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.DataProvider;

namespace YetaWF.Modules.Blog.Controllers {

    public class DisqusModuleController : ControllerImpl<YetaWF.Modules.Blog.Modules.DisqusModule> {

        public DisqusModuleController() { }

        public class DisplayModel {
            public string ShortName { get; set; }
        }

        [HttpGet]
        public ActionResult Disqus() {
            using (DisqusConfigDataProvider dataProvider = new DisqusConfigDataProvider()) {
                DisqusConfigData config = dataProvider.GetItem();
                if (config == null)
                    throw new Error(this.__ResStr("notFound", "The Disqus settings could not be found"));
                if (string.IsNullOrWhiteSpace(config.ShortName))
                    throw new Error(this.__ResStr("notShortName", "The Disqus settings must be updated to define the site's Shortname"));
                DisplayModel model = new DisplayModel {
                    ShortName = config.ShortName,
                };
                return View(model);
            }
        }
    }
}
