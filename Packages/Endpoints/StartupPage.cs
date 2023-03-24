/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YetaWF.Core.Endpoints;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Site;
using YetaWF.Core.Support;
using YetaWF.Modules.Packages.DataProvider;

namespace YetaWF.Modules.Packages.Endpoints;

public class StartupPageEndpoints : YetaWFEndpoints {

    internal const string Show = "Show";
    internal const string Run = "Run";
    internal const string GetPackageList = "GetPackageList";

    private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(StartupPageEndpoints), name, defaultValue, parms); }

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(StartupPageEndpoints)));

        group.MapGet(Show, async (HttpContext context) => {

            string url = Package.GetAddOnPackageUrl(AreaRegistration.CurrentPackage.AreaName);
            string path = Utility.UrlToPhysical(url);

            string file;
            if (SiteDefinition.INITIAL_INSTALL_ENDED)
                file = "StartupDone.html";
            else
                file = "StartupPage.html";

            string content = await FileSystem.FileSystemProvider.ReadAllTextAsync(Path.Combine(path, "HTML", file));
            return Results.Text(content, "text/html");
        });

        group.MapPost(Run, async (HttpContext context) => {

            if (!SiteDefinition.INITIAL_INSTALL || SiteDefinition.INITIAL_INSTALL_ENDED)
                return Results.Unauthorized();

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

            return Results.Empty;
        });

        group.MapPost(GetPackageList, (HttpContext context) => {

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
            return Results.Json(hb.ToString());
        });
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
}
