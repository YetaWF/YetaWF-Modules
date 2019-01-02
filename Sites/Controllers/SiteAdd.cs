/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Sites#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Site;
using YetaWF.Modules.Packages.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Sites.Controllers {
    public class SiteAddModuleController : ControllerImpl<YetaWF.Modules.Sites.Modules.SiteAddModule> {

        public SiteAddModuleController() { }

        [Trim]
        public class AddModel {

            [Caption("Site Domain"), Description("The domain name of the site")]
            [UIHint("Text80"), StringLength(SiteDefinition.MaxSiteDomain), SiteDomainValidation, Required, Trim]
            public string SiteDomain { get; set; }

            [Caption("Site Name"), Description("The name associated with your site, usually your company name or your name")]
            [UIHint("Text80"), StringLength(SiteDefinition.MaxSiteName), Required, Trim]
            public string SiteName { get; set; }

            public AddModel() { }

            public SiteDefinition GetData() {
                SiteDefinition data = new SiteDefinition();
                ObjectSupport.CopyData(this, data);
                return data;
            }
        }

        [AllowGet]
        public ActionResult SiteAdd() {
            AddModel model = new AddModel { };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> SiteAdd_Partial(AddModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);

            SiteDefinition currentSite = Manager.CurrentSite;

            SiteDefinition newSite = model.GetData();
            await newSite.AddNewAsync();

            Manager.CurrentSite = newSite;
            try {
                PackagesDataProvider packagesDP = new PackagesDataProvider();
                await packagesDP.InitNewAsync(true);
            } catch (Exception) {
                throw;
            } finally {
                Manager.CurrentSite = currentSite;
            }

            string nextPage = Manager.CurrentSite.MakeUrl(RealDomain: Manager.CurrentSite.SiteDomain);

            Manager.RestartSite();

            return FormProcessed(model, this.__ResStr("okSaved", "New site \"{0}\" created - Click OK to populate the new site with the current site template.(+nl)(+nl)IMPORTANT: This site is not accessible by its Url until the domain \"{0}\" is defined in IIS and in the hosts file.", newSite.SiteDomain),
                NextPage: nextPage);
        }
    }
}
