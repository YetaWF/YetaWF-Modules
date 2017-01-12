/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/SiteProperties#License */

using System.Web;
using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Site;
using YetaWF.Core.Support;

namespace YetaWF.Modules.SiteProperties.Controllers {

    public class SitePropertiesModuleController : ControllerImpl<YetaWF.Modules.SiteProperties.Modules.SitePropertiesModule> {

        public class SitePropertiesModel {
            [UIHint("PropertyListTabbed")]
            public SiteDefinition Site { get; set; }
            [UIHint("Hidden")]
            public string SiteHost { get; set; }
        }

        [HttpGet]
        public ActionResult SiteProperties(string domain) {
            SiteDefinition site;
            if (domain == null)
                site = Manager.CurrentSite;
            else
                site = SiteDefinition.LoadSiteDefinition(domain);
            if (site == null)
                throw new Error(this.__ResStr("errNoSite", "Site \"{0}\" not found", domain));
            SitePropertiesModel model = new SitePropertiesModel {
                SiteHost = domain,
                Site = site,
            };
            return View(model);
        }

        [HttpPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult SiteProperties_Partial(SitePropertiesModel model) {
            SiteDefinition site;
            if (model.SiteHost == null)
                site = Manager.CurrentSite;
            else
                site = SiteDefinition.LoadSiteDefinition(model.SiteHost);
            if (!ModelState.IsValid)
                return PartialView(model);
            ObjectSupport.CopyDataFromOriginal(site, model.Site);
            bool restartRequired;
            model.Site.Save(out restartRequired);
            if (restartRequired) {
                Manager.RestartSite();
                return FormProcessed(model, this.__ResStr("okSavedRestart", "Site settings updated - Site is now restarting"), NextPage: Manager.CurrentSite.HomePageUrl);
            } else
                return FormProcessed(model, this.__ResStr("okSaved", "Site settings updated"));
        }
    }
}