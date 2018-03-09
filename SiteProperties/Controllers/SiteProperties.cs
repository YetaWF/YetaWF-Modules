/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SiteProperties#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Site;
using YetaWF.Core.Support;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web;
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.SiteProperties.Controllers {

    public class SitePropertiesModuleController : ControllerImpl<YetaWF.Modules.SiteProperties.Modules.SitePropertiesModule> {

        public class SitePropertiesModel {
            [UIHint("PropertyListTabbed")]
            public SiteDefinition Site { get; set; }
            [UIHint("Hidden")]
            public string SiteHost { get; set; }
        }

        [AllowGet]
        public async Task<ActionResult> SiteProperties(string domain) {
            SiteDefinition site;
            if (domain == null)
                site = Manager.CurrentSite;
            else
                site = await SiteDefinition.LoadSiteDefinitionAsync(domain);
            if (site == null)
                throw new Error(this.__ResStr("errNoSite", "Site \"{0}\" not found", domain));
            SitePropertiesModel model = new SitePropertiesModel {
                SiteHost = domain,
                Site = site,
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> SiteProperties_Partial(SitePropertiesModel model) {
            SiteDefinition site;
            if (model.SiteHost == null)
                site = Manager.CurrentSite;
            else
                site = await SiteDefinition.LoadSiteDefinitionAsync(model.SiteHost);
            if (!ModelState.IsValid)
                return PartialView(model);
            ObjectSupport.CopyDataFromOriginal(site, model.Site);
            SiteDefinition.SaveResult res = await model.Site.SaveAsync();
            if (res.RestartRequired) {
                Manager.RestartSite();
                return FormProcessed(model, this.__ResStr("okSavedRestart", "Site settings updated - Site is now restarting"),
                    NextPage: Manager.CurrentSite.HomePageUrl, ForceRedirect: true);
            } else
                return FormProcessed(model, this.__ResStr("okSaved", "Site settings updated"));
        }
    }
}
