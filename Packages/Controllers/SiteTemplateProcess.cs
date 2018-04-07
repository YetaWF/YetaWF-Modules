/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Views.Shared;
using YetaWF.Core.Support;
using YetaWF.Modules.Packages.DataProvider;
using System.Threading.Tasks;
using YetaWF.Core.IO;
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

            public async Task UpdateDataAsync() {
                PackagesDataProvider packagesDP = new PackagesDataProvider();
                SiteTemplate_List = (from f in await FileSystem.FileSystemProvider.GetFilesAsync(packagesDP.TemplateFolder, "*.txt") orderby f select new SelectionItem<string>() {
                    Text = Path.GetFileName(f),
                    Value = Path.GetFileName(f),
                }).ToList();
            }
            public EditModel() { }
        }

        [AllowGet]
        public async Task<ActionResult> SiteTemplateProcess(string fileName) {
            EditModel model = new EditModel { };
            await model.UpdateDataAsync();
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> SiteTemplateProcess_Partial(EditModel model) {
            await model.UpdateDataAsync();
            if (!ModelState.IsValid)
                return PartialView(model);

            if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Site template processing is not possible when distributed caching is enabled");

            PackagesDataProvider packagesDP = new PackagesDataProvider();
            await packagesDP.BuildSiteUsingTemplateAsync(model.SiteTemplate);
            return FormProcessed(model);
        }
    }
}