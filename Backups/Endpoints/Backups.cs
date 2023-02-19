/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using YetaWF.Core.Addons;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Backups.Modules;

namespace YetaWF.Modules.Backups.Endpoints;

public class BackupsModuleEndpoints : YetaWFEndpoints {

    internal const string PerformSiteBackup = "PerformSiteBackup";
    internal const string MakeSiteTemplateData = "MakeSiteTemplateData";
    internal const string Remove = "Remove";
    internal const string Download = "Download";

    private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(BackupsModuleEndpoints), name, defaultValue, parms); }

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(BackupsModuleEndpoints)))
            .RequireAuthorization()
            .AntiForgeryToken();

        group.MapPost(GridSupport.BrowseGridData, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
            BackupsModule module = await GetModuleAsync<BackupsModule>(gridPvData.__ModuleGuid);
            if (!module.IsAuthorized("Backups")) return Results.Unauthorized();
            return await GridSupport.GetGridPartialAsync(context, module, module.GetGridModel(), gridPvData);
        });

        group.MapPost(PerformSiteBackup, async (HttpContext context, [FromQuery] Guid __ModuleGuid) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("Backups")) return Results.Unauthorized();
            List<string> errorList = new List<string>();
            SiteBackup siteBackup = new SiteBackup();
            if (!await siteBackup.CreateAsync(errorList, ForDistribution: true)) {
                ScriptBuilder sb = new ScriptBuilder();
                sb.Append(__ResStr("cantBackup", "Can't create a site backup for site {0}:(+nl)"), Manager.CurrentSite.SiteDomain);
                sb.Append(errorList, LeadingNL: true);
                throw new Error(sb.ToString());
            }
            return Reload(ReloadEnum.ModuleParts, PopupText: __ResStr("backupCreated", "The site backup has been successfully created"));
        })
            .ExcludeDemoMode();

        group.MapPost(MakeSiteTemplateData, async (HttpContext context, [FromQuery] Guid __ModuleGuid) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("Backups")) return Results.Unauthorized();
            List<string> errorList = new List<string>();
            if (YetaWFManager.Deployed)
                throw new InternalError("Can't make site template data on a deployed site");
            SiteTemplateData siteTemplateData = new SiteTemplateData();
            await siteTemplateData.MakeSiteTemplateDataAsync();
            return Reload(ReloadEnum.ModuleParts, PopupText: __ResStr("templatesCreated", "The templates for the current site have been successfully created in the \\SiteTemplates\\Data folder"));
        })
            .ExcludeDemoMode();

        group.MapPost(Remove, async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromQuery] string filename) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("Backups")) return Results.Unauthorized();
            if (string.IsNullOrWhiteSpace(filename))
                throw new InternalError("No backup specified");
            SiteBackup siteBackup = new SiteBackup();
            await siteBackup.RemoveAsync(filename);
            return Reload(ReloadEnum.ModuleParts);
        })
            .ExcludeDemoMode();

        group.MapGet(Download, async (HttpContext context, [FromQuery] Guid __ModuleGuid, string filename, long cookieToReturn) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("Downloads")) return Results.Unauthorized();
            if (string.IsNullOrWhiteSpace(filename))
                throw new InternalError("No backup specified");
            filename = Path.ChangeExtension(filename, "zip");
            string path = Path.Combine(Manager.SiteFolder, SiteBackup.BackupFolder, filename);
            if (!await FileSystem.FileSystemProvider.FileExistsAsync(path))
                throw new Error(__ResStr("backupNotFound", "The backup '{0}' cannot be located.", filename));

            context.Response.Headers.Remove("Cookie");
            context.Response.Cookies.Append(Basics.CookieDone, cookieToReturn.ToString(), new Microsoft.AspNetCore.Http.CookieOptions { HttpOnly = false, Path = "/" });
            return Results.File(path, null, filename);
        })
            .ExcludeDemoMode();
    }
}
