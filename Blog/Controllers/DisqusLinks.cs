/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using YetaWF.Core.Controllers;
using YetaWF.Modules.Blog.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
using YetaWF.Core.Support;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Blog.Controllers {

    public class DisqusLinksModuleController : ControllerImpl<YetaWF.Modules.Blog.Modules.DisqusLinksModule> {

        public DisqusLinksModuleController() { }

        public class DisplayModel {
            public string ShortName { get; set; }
        }

        [AllowGet]
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
