/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Packages.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Packages.Controllers {

    public class SiteTemplateProcessModuleController : ControllerImpl<YetaWF.Modules.Packages.Modules.SiteTemplateProcessModule> {

        public SiteTemplateProcessModuleController() { }

        [Trim]
        [Footer("The selected template is used to configure the current site - Once completed the page has to be manually refreshed (deliberately not automatic)")]
        public class EditModel {

            [Caption("Name"), Description("List of site templates (located in the SiteTemplates folder)")]
            [UIHint("DropDownList")]
            public string SiteTemplate { get; set; }

            public List<SelectionItem<string>> SiteTemplate_List { get; set; }

            public void UpdateData() {
                PackagesDataProvider packagesDP = new PackagesDataProvider();
                SiteTemplate_List = (from f in Directory.GetFiles(packagesDP.TemplateFolder, "*.txt") orderby f select new SelectionItem<string>() {
                    Text = Path.GetFileName(f),
                    Value = Path.GetFileName(f),
                }).ToList();
            }
            public EditModel() { }
        }

        [AllowGet]
        public ActionResult SiteTemplateProcess(string fileName) {
            EditModel model = new EditModel { };
            model.UpdateData();
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult SiteTemplateProcess_Partial(EditModel model) {
            model.UpdateData();
            if (!ModelState.IsValid)
                return PartialView(model);

            PackagesDataProvider packagesDP = new PackagesDataProvider();
            packagesDP.BuildSiteUsingTemplate(model.SiteTemplate);
            return FormProcessed(model);
        }
    }
}