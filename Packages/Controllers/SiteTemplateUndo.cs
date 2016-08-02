/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Packages#License */

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Packages.DataProvider;

namespace YetaWF.Modules.Packages.Controllers {

    public class SiteTemplateUndoModuleController : ControllerImpl<YetaWF.Modules.Packages.Modules.SiteTemplateUndoModule> {

        public SiteTemplateUndoModuleController() { }

        [Trim]
        [Footer("The selected template is used to remove pages and menu entries from the current site - Once completed the page has to be manually refreshed (deliberately not automatic)")]
        public class EditModel {

            [Caption("Name"), Description("List of site templates (located in the SiteTemplates folder)")]
            [UIHint("DropDownList")]
            public string SiteTemplate { get; set; }

            public List<SelectionItem<string>> SiteTemplate_List { get; set; }

            public void UpdateData() {
                PackagesDataProvider packagesDP = new PackagesDataProvider();
                FileData fd = new FileData { BaseFolder = packagesDP.TemplateFolder };
                SiteTemplate_List = (from f in fd.GetNames() orderby f select new SelectionItem<string>() {
                    Text = f,
                    Value = f,
                }).ToList();
            }
            public EditModel() { }
        }

        [HttpGet]
        public ActionResult SiteTemplateUndo(string fileName) {
            EditModel model = new EditModel { };
            model.UpdateData();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult SiteTemplateUndo_Partial(EditModel model) {
            model.UpdateData();
            if (!ModelState.IsValid)
                return PartialView(model);

            PackagesDataProvider packagesDP = new PackagesDataProvider();
            packagesDP.BuildSiteUsingTemplate(model.SiteTemplate, false);
            return FormProcessed(model);
        }
    }
}