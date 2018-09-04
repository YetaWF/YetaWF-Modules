/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Sites#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Site;
using YetaWF.Core.Support;
using System;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Sites.Controllers {

    public class ConfirmRemovalModuleController : ControllerImpl<YetaWF.Modules.Sites.Modules.ConfirmRemovalModule> {

        public ConfirmRemovalModuleController() { }

        [Header("Are you absolutely sure you want to remove this site? This will remove all data for this site!!! Please make sure you have a backup (just in case...).")]
        [Footer("Click Confirm to delete this site and all site-specific data.")]
        [Trim]
        public class EditModel {

            [Caption("Site"), Description("The domain name of the site to remove")]
            [UIHint("String"), ReadOnly]
            public string SiteDomainDisplay { get; set; }

            [UIHint("Hidden")]
            public string SiteDomain { get; set; }

            public EditModel() { }
        }

        [AllowGet]
        public ActionResult ConfirmRemoval(string siteDomain) {
            EditModel model = new EditModel {};
            model.SiteDomainDisplay = siteDomain;
            model.SiteDomain = siteDomain;
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> ConfirmRemoval_Partial(EditModel model) {
            model.SiteDomainDisplay = model.SiteDomain;
            if (!ModelState.IsValid)
                return PartialView(model);

            SiteDefinition site = await SiteDefinition.LoadSiteDefinitionAsync(model.SiteDomain);
            if (site == null)
                throw new InternalError($"Site {site.SiteDomain} not found");

            SiteDefinition currentSite = Manager.CurrentSite;
            Manager.CurrentSite = site;
            try {
                await Manager.CurrentSite.RemoveAsync();
            } catch (Exception) {
                throw;
            } finally {
                Manager.CurrentSite = currentSite;
            }

            string nextPage = Manager.CurrentSite.MakeUrl(RealDomain: Manager.CurrentSite.SiteDomain);

            Manager.RestartSite();

            return FormProcessed(null, this.__ResStr("okRemoved", "Site \"{0}\" has been removed(+nl)(+nl)The site is now restarting", model.SiteDomain),
                NextPage: nextPage);
        }
    }
}