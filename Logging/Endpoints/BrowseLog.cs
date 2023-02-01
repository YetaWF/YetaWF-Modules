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
using YetaWF.Modules.Logging.Controllers;
using YetaWF.Modules.LoggingDataProvider.DataProvider;

namespace YetaWF.Modules.Logging.Endpoints {

    public class BrowseLogModuleEndpoints : YetaWFEndpoints {

        internal static string RemoveAll = "RemoveAll";
        internal static string DownloadLog = "DownloadLog";
        internal static string DownloadZippedLog = "DownloadZippedLog";

        private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(BrowseLogModuleEndpoints), name, defaultValue, parms); }

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(BrowseLogModuleEndpoints)))
                .RequireAuthorization();

            group.MapPost(GridSupport.BrowseGridData, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
                ModuleDefinition module = await GetModuleAsync(gridPvData.__ModuleGuid);
                if (!module.IsAuthorized()) return Results.Unauthorized();
                return await GridSupport.GetGridPartialAsync(context, module, BrowseLogModuleController.GetGridModel(module), gridPvData);
            })
                .AntiForgeryToken();

            group.MapPost(RemoveAll, async (HttpContext context, [FromQuery] Guid __ModuleGuid) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized("RemoveLog")) return Results.Unauthorized();
                BrowseLogModuleController.FlushLog();
                using (LogRecordDataProvider dataProvider = LogRecordDataProvider.GetLogRecordDataProvider()) {
                    await dataProvider.RemoveItemsAsync(null);// that means all records
                    return Reload(ReloadEnum.ModuleParts, __ResStr("allRemoved", "All log records have been removed"));
                }
            })
                .AntiForgeryToken()
                .ExcludeDemoMode();

            group.MapGet(DownloadLog, async (HttpContext context, [FromQuery] Guid __ModuleGuid, long cookieToReturn) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized("Downloads")) return Results.Unauthorized();
                BrowseLogModuleController.FlushLog();
                using (LogRecordDataProvider dataProvider = LogRecordDataProvider.GetLogRecordDataProvider()) {
                    string filename = dataProvider.GetLogFileName();
                    if (!await FileSystem.FileSystemProvider.FileExistsAsync(filename))
                        throw new Error(__ResStr("logNotFound", "The log file '{0}' cannot be located", filename));
                    context.Response.Headers.Remove("Cookie");
                    context.Response.Cookies.Append(Basics.CookieDone, cookieToReturn.ToString(), new Microsoft.AspNetCore.Http.CookieOptions { HttpOnly = false, Path = "/" });
                    return Results.File(filename, null, "Logfile.txt");
                }
            })
                .ExcludeDemoMode();

            group.MapGet(DownloadZippedLog, async (HttpContext context, [FromQuery] Guid __ModuleGuid, long cookieToReturn) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized("Downloads")) return Results.Unauthorized();
                BrowseLogModuleController.FlushLog();
                using (LogRecordDataProvider dataProvider = LogRecordDataProvider.GetLogRecordDataProvider()) {
                    string filename = dataProvider.GetLogFileName();
                    if (!await FileSystem.FileSystemProvider.FileExistsAsync(filename))
                        throw new Error(__ResStr("logNotFound", "The log file '{0}' cannot be located", filename));
                    string zipName = "Logfile.zip";
                    YetaWFZipFile zipFile = new YetaWFZipFile {
                        FileName = zipName,
                    };
                    zipFile.AddFile(filename, "Logfile.txt");
                    return await ZippedFileResult.ZipFileAsync(context, zipFile, cookieToReturn);
                }
            })
                .ExcludeDemoMode();
        }
    }
}
