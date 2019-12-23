/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Support;
using YetaWF.Modules.Packages.DataProvider;
using YetaWF.Core.Site;
using YetaWF.Core.Packages;
using System.Collections.Generic;
using System.Linq;
using YetaWF.DataProvider;
using YetaWF.Core.IO;
using System.IO;
using YetaWF.Core.Addons;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Packages.Controllers {

    // Standard MVC Controller
    // Standard MVC Controller
    // Standard MVC Controller

    public class StartupPageController : YetaWFController {

        [AllowGet]
        public async Task<ActionResult> Show() {

            string url = VersionManager.GetAddOnPackageUrl(AreaRegistration.CurrentPackage.AreaName);
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
                "YetaWF.BootstrapSkin",
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
                if (RequiredPackages.Contains(package.Name))
                    disabled = " disabled='disabled'";

                hb.Append($@"
<tr>
    <td><input type='checkbox' checked='checked' name='{package.Name}'{disabled}/></td>
    <td>{Utility.HtmlEncode(package.Name)}</td>
    <td>{Utility.HtmlEncode(package.Description)}</td>
</tr>");

            }
            return new YJsonResult() { Data = hb.ToString() };
        }
    }
}