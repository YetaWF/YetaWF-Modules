/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Blog#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Modules.Blog.DataProvider;

namespace YetaWF.Modules.Blog.Controllers {

    public class DisqusLinksModuleController : ControllerImpl<YetaWF.Modules.Blog.Modules.DisqusLinksModule> {

        public DisqusLinksModuleController() { }

        public class DisplayModel {
            public string ShortName { get; set; }
        }

        [HttpGet]
        public ActionResult DisqusLinks(string disqus) {
            using (DisqusConfigDataProvider dataProvider = new DisqusConfigDataProvider()) {
                DisqusConfigData config = dataProvider.GetItem();
                if (config == null)
                    return new EmptyResult();
                if (string.IsNullOrWhiteSpace(config.ShortName))
                    return new EmptyResult();
                DisplayModel model = new DisplayModel {
                    ShortName = config.ShortName,
                };
                return View(model);
            }
        }
    }
}
