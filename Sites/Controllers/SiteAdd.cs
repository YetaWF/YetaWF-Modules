/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Sites#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Site;

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

        [HttpGet]
        public ActionResult SiteAdd() {
            AddModel model = new AddModel { };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult SiteAdd_Partial(AddModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            SiteDefinition newSite = model.GetData();
            newSite.AddNew();
            string newUrl = newSite.MakeUrl("/$initnew?From=Data", ForceDomain: newSite.SiteDomain); // This builds the new site (supported by YetaWF.Packages as a builtin command)
            return FormProcessed(model, this.__ResStr("okSaved", "New site \"{0}\" created - Click OK to populate the new site with the current site template.(+nl)(+nl)IMPORTANT: This site is not accessible by its Url until the domain \"{0}\" is defined in IIS and in the hosts file.", newSite.SiteDomain),
                NextPage: newUrl);
        }
    }
}
