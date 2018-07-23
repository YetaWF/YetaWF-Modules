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
            [UIHint("PropertyList")]
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
            SiteDefinition origSite;
            if (model.SiteHost == null)
                origSite = Manager.CurrentSite;
            else
                origSite = await SiteDefinition.LoadSiteDefinitionAsync(model.SiteHost);
            if (!ModelState.IsValid)
                return PartialView(model);
            ObjectSupport.CopyDataFromOriginal(origSite, model.Site);
            await model.Site.SaveAsync();
            ObjectSupport.ModelDisposition modelDisp = ObjectSupport.EvaluateModelChanges(origSite, model.Site);
            switch (modelDisp) {
                default:
                case ObjectSupport.ModelDisposition.None:
                    return FormProcessed(model, this.__ResStr("okSaved", "Site settings updated"));
                case ObjectSupport.ModelDisposition.PageReload:
                    return FormProcessed(model, this.__ResStr("okSaved", "Site settings updated"), OnClose: OnCloseEnum.ReloadPage, OnPopupClose: OnPopupCloseEnum.ReloadParentPage, ForceRedirect: true);
                case ObjectSupport.ModelDisposition.SiteRestart:
                    return FormProcessed(model, this.__ResStr("okSavedRestart", "Site settings updated - These settings won't take effect until the site is restarted"));
            }
        }
    }
}
