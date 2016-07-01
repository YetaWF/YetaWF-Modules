/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Sites#License */

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Site;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;

namespace YetaWF.Modules.Sites.Controllers {

    public class SiteSelectorModuleController : ControllerImpl<YetaWF.Modules.Sites.Modules.SiteSelectorModule> {

        public SiteSelectorModuleController() { }

        [Trim]
        public class EditModel {

            [Caption("Active Site"), Description("List of sites that can be accessed")]
            [UIHint("DropDownList"), SubmitFormOnChange]
            public string SiteDomain { get; set; }

            public List<SelectionItem<string>> SiteDomain_List { get; set; }

            public void UpdateData() {
                SiteDefinition.SitesInfo info = SiteDefinition.GetSites(0, 0, null, null);
                SiteDomain_List = (from s in info.Sites orderby s.SiteDomain select new SelectionItem<string>() {
                    Text = s.SiteDomain,
                    Value = s.SiteDomain,
                    Tooltip = this.__ResStr("switchSite", "Switch to site \"{0}\"", s.SiteDomain),
                }).ToList();
            }
            public EditModel() { }
        }

        [HttpGet]
        public ActionResult SiteSelector(string siteDomain) {
            if (Manager.Deployed && !Manager.HasSuperUserRole) return new EmptyResult();
            EditModel model = new EditModel { SiteDomain = Manager.CurrentSite.SiteDomain };
            model.UpdateData();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SiteSelector_Partial(EditModel model) {
            model.UpdateData();
            if (!ModelState.IsValid)
                return PartialView(model);

            string nextPage = Manager.CurrentSite.MakeUrl(RealDomain: model.SiteDomain);
            return Redirect(nextPage);
        }
    }
}