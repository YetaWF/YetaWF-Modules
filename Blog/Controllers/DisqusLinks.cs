/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Modules.Blog.DataProvider;

namespace YetaWF.Modules.Blog.Controllers {

    public class DisqusLinksModuleController : ControllerImpl<YetaWF.Modules.Blog.Modules.DisqusLinksModule> {

        public DisqusLinksModuleController() { }

        public class DisplayModel {
            public string? ShortName { get; set; }
        }

        [AllowGet]
        public async Task<ActionResult> DisqusLinks(string disqus) {
            using (DisqusConfigDataProvider dataProvider = new DisqusConfigDataProvider()) {
                DisqusConfigData config = await dataProvider.GetItemAsync();
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
