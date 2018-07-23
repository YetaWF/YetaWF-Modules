/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Sites#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Site;
using YetaWF.Core.Components;
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

            public async Task UpdateDataAsync() {
                DataProviderGetRecords<SiteDefinition> info = await SiteDefinition.GetSitesAsync(0, 0, null, null);
                SiteDomain_List = (from s in info.Data orderby s.SiteDomain select new SelectionItem<string>() {
                    Text = s.SiteDomain,
                    Value = s.SiteDomain,
                    Tooltip = this.__ResStr("switchSite", "Switch to site \"{0}\"", s.SiteDomain),
                }).ToList();
            }
            public EditModel() { }
        }

        [AllowGet]
        public async Task<ActionResult> SiteSelector(string siteDomain) {
#if !DEBUG
            if (Manager.Deployed && !Manager.HasSuperUserRole) return new EmptyResult();
#endif
            if (Manager.RenderStaticPage) return new EmptyResult();

            EditModel model = new EditModel { SiteDomain = Manager.CurrentSite.SiteDomain };
            await model.UpdateDataAsync();
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> SiteSelector_Partial(EditModel model) {
            await model.UpdateDataAsync();
            if (!ModelState.IsValid)
                return PartialView(model);

            string nextPage = Manager.CurrentSite.MakeFullUrl(RealDomain: model.SiteDomain, SecurityType: Core.Pages.PageDefinition.PageSecurityType.httpOnly);
            return Redirect(nextPage, ForceRedirect: true);
        }
    }
}