/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Sites#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Site;

namespace YetaWF.Modules.Sites.Controllers {

    public class ConfirmRemovalModuleController : ControllerImpl<YetaWF.Modules.Sites.Modules.ConfirmRemovalModule> {

        public ConfirmRemovalModuleController() { }

        [Header("Are you absolutely sure you want to remove this site? This will remove all data for this site!!! Please make sure you have a backup (just in case...).")]
        [Footer("Click Confirm to delete this site and all site-specific data.")]
        [Trim]
        public class EditModel {

            [Caption("Site"), Description("The domain name of the site to remove")]
            [UIHint("String"), ReadOnly]
            public string SiteDomain { get; set; }

            public EditModel() { }
        }

        [HttpGet]
        public ActionResult ConfirmRemoval(string siteDomain) {
            EditModel model = new EditModel {};
            model.SiteDomain = Manager.CurrentSite.SiteDomain;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmRemoval_Partial(EditModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            string siteName = Manager.CurrentSite.SiteDomain;
            SiteDefinition site = SiteDefinition.LoadSiteDefinition(null);//load the default site
            string nextPage = Manager.CurrentSite.MakeUrl(RealDomain: site.SiteDomain);

            Manager.CurrentSite.Remove();
            return FormProcessed(null, this.__ResStr("okRemoved", "Site \"{0}\" has been removed(+nl)(+nl)The site is now restarting", siteName),
                NextPage: nextPage);
        }
    }
}