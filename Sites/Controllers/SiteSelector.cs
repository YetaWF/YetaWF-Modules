/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Sites#License */

using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Site;
using YetaWF.Core.Views.Shared;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Sites.Controllers {

    public class SiteSelectorModuleController : ControllerImpl<YetaWF.Modules.Sites.Modules.SiteSelectorModule> {

        public SiteSelectorModuleController() { }

        [Trim]
        public class EditModel {

            [Caption("Active Site"), Description("List of sites that can be accessed - select an entry to visit the site")]
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

        [AllowGet]
        public ActionResult SiteSelector(string siteDomain) {
#if !DEBUG
            if (Manager.Deployed && !Manager.HasSuperUserRole) return new EmptyResult();
#endif
            if (Manager.RenderStaticPage) return new EmptyResult();

            EditModel model = new EditModel { SiteDomain = Manager.CurrentSite.SiteDomain };
            model.UpdateData();
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult SiteSelector_Partial(EditModel model) {
            model.UpdateData();
            if (!ModelState.IsValid)
                return PartialView(model);

            string nextPage = Manager.CurrentSite.MakeFullUrl(RealDomain: model.SiteDomain, SecurityType: Core.Pages.PageDefinition.PageSecurityType.httpOnly);
            return Redirect(nextPage, ForceRedirect: true);
        }
    }
}