/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.IO;
using YetaWF.Core.Packages;
using YetaWF.Core.Site;
using YetaWF.Core.Support;
using YetaWF.Modules.Packages.DataProvider;

namespace YetaWF.Modules.Packages.Controllers {

    // Standard MVC Controller
    // Standard MVC Controller
    // Standard MVC Controller

    public class StartupPageController : YetaWFController {

        [AllowGet]
        public async Task<ActionResult> Show() {

            string url = Package.GetAddOnPackageUrl(AreaRegistration.CurrentPackage.AreaName);
            string path = Utility.UrlToPhysical(url);

            string file;
            if (SiteDefinition.INITIAL_INSTALL_ENDED)
                file = "StartupDone.html";
            else
                file = "StartupPage.html";

            string content = await FileSystem.FileSystemProvider.ReadAllTextAsync(Path.Combine(path, "HTML", file));
            return Content(content, "text/html");
        }

        [AllowPost]
        public async Task<ActionResult> Run() {

            if (!SiteDefinition.INITIAL_INSTALL || SiteDefinition.INITIAL_INSTALL_ENDED)
                return NotAuthorized();

            List<string> wantedPackages = new List<string>();
            FormHelper form = Manager.RequestForm;
            foreach (string key in form.GetCollection().Keys) {
                if (form[key] == "on")
                    wantedPackages.Add(key);
            }
            wantedPackages.AddRange(RequiredPackages);

            PackagesDataProvider packagesDP = new PackagesDataProvider();
            QueryHelper qh = new QueryHelper();
            qh.Add("From", "Data");
            await packagesDP.InitAllAsync(qh, wantedPackages);

            return new EmptyResult();
        }

        static List<string> RequiredPackages = new List<string> {
                "YetaWF.Basics",
                "YetaWF.Caching",
                "YetaWF.Core",
                "YetaWF.Core.CssHttpHandler",
                "YetaWF.Core.ImageHttpHandler",
                "YetaWF.Core.WebpHttpHandler",
                "YetaWF.ComponentsHTML",
                "YetaWF.DataProvider.File",
                "YetaWF.DataProvider.Localization",
                "YetaWF.DataProvider.ModuleDefinition",
                "YetaWF.DataProvider.SQL",
                "YetaWF.DataProvider.PostgreSQL",
                "YetaWF.Languages",
                "YetaWF.Logging",
                "YetaWF.LoggingDataProvider",
                "YetaWF.Identity",
                "YetaWF.Menus",
                "YetaWF.ModuleEdit",
                "YetaWF.Modules",
                "YetaWF.Packages",
                "YetaWF.PageEdit",
                "YetaWF.Pages",
                "YetaWF.SiteProperties",
                "YetaWF.WebStartup",
            };

        [AllowPost]
        public ActionResult GetPackageList() {

            HtmlBuilder hb = new HtmlBuilder();

            List<Package> packages = Package.GetAvailablePackages();
            packages = packages.OrderBy((p) => p.Name).ToList();
            foreach (Package package in packages) {

                string disabled = "";
                if (RequiredPackages.Contains(package.Name)) {
                    //disabled = " disabled='disabled'";
                } else {
                    hb.Append($@"
<tr>
    <td><input type='checkbox' checked='checked' name='{package.Name}'{disabled}/></td>
    <td>{Utility.HE(package.Name)}</td>
    <td>{Utility.HE(package.Description)}</td>
</tr>");
                }
            }
            return new YJsonResult() { Data = hb.ToString() };
        }
    }
}