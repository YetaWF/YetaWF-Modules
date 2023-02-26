/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using YetaWF.Core.Addons;
using YetaWF.Core.Controllers;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core.Support.Zip;
using YetaWF.Modules.Scheduler.DataProvider;
using YetaWF.Modules.Scheduler.Modules;

namespace YetaWF.Modules.Scheduler.Endpoints;

public class LogBrowseModuleEndpoints : YetaWFEndpoints {

    internal const string RemoveAll = "RemoveAll";
    internal const string DownloadLog = "DownloadLog";
    internal const string DownloadZippedLog = "DownloadZippedLog";

    private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(LogBrowseModuleEndpoints), name, defaultValue, parms); }

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(LogBrowseModuleEndpoints)))
            .RequireAuthorization()
            .AntiForgeryToken();

        group.MapPost(GridSupport.BrowseGridData, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
            LogBrowseModule module = await GetModuleAsync<LogBrowseModule>(gridPvData.__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();
            return await GridSupport.GetGridPartialAsync(context, module, module.GetGridModel(), gridPvData);
        });

        group.MapPost(RemoveAll, async (HttpContext context, [FromQuery] Guid __ModuleGuid) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("RemoveLog")) return Results.Unauthorized();
            using (LogDataProvider logDP = new LogDataProvider()) {
                await logDP.RemoveItemsAsync(null);
                return Reload(ReloadEnum.ModuleParts);
            }
        })
            .ExcludeDemoMode();


        RouteGroupBuilder groupDownload = endpoints.MapGroup(GetPackageApiRoute(package, typeof(LogBrowseModuleEndpoints)))
            .RequireAuthorization();

        groupDownload.MapGet(DownloadLog, async (HttpContext context, [FromQuery] Guid __ModuleGuid, long cookieToReturn) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("Downloads")) return Results.Unauthorized();
            using (LogDataProvider logDP = new LogDataProvider()) {
                string? filename = logDP.GetLogFileName();
                if (filename == null || !await FileSystem.FileSystemProvider.FileExistsAsync(filename))
                    throw new Error(__ResStr("logNotFound", "The scheduler log file '{0}' cannot be located", filename));
                context.Response.Headers.Remove("Cookie");
                context.Response.Cookies.Append(Basics.CookieDone, cookieToReturn.ToString(), new Microsoft.AspNetCore.Http.CookieOptions { HttpOnly = false, Path = "/" });
                return Results.File(filename, null, "Logfile.txt");
            }
        })
            .ExcludeDemoMode();

        groupDownload.MapGet(DownloadZippedLog, async (HttpContext context, [FromQuery] Guid __ModuleGuid, long cookieToReturn) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("Downloads")) return Results.Unauthorized();
            using (LogDataProvider dataProvider = new LogDataProvider()) {
                string? filename = dataProvider.GetLogFileName();
                if (filename == null || !await FileSystem.FileSystemProvider.FileExistsAsync(filename))
                    throw new Error(__ResStr("logNotFound", "The scheduler log file '{0}' cannot be located", filename));
                string zipName = "Logfile.zip";
                YetaWFZipFile zipFile = new YetaWFZipFile {
                    FileName = zipName,
                };
                zipFile.AddFile(filename, "SchedulerLog.txt");
                return await ZippedFileResult.ZipFileAsync(context, zipFile, cookieToReturn);
            }
        })
            .ExcludeDemoMode();
    }
}
