/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Core.Support.Zip;
using YetaWF.Core.Upload;

namespace YetaWF.Modules.PageEdit.Endpoints;

public class PageControlModuleEndpoints : YetaWFEndpoints {

    internal const string SwitchToEdit = "SwitchToEdit";
    internal const string SwitchToView = "SwitchToView";
    internal const string ClearJsCss = "ClearJsCss";
    internal const string ImportModule = "ImportModule";
    internal const string RemoveModule = "RemoveModule";
    internal const string ImportPage = "ImportPage";
    internal const string RemovePage = "RemovePage";
    internal const string ExportPage = "ExportPage";

    private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(PageControlModuleEndpoints), name, defaultValue, parms); }

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(PageControlModuleEndpoints)))
            .RequireAuthorization()
            .AntiForgeryToken();

        group.MapPost(ClearJsCss, async (HttpContext context, Guid __ModuleGuid) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();
            if (!Manager.HasSuperUserRole)
                return Results.Unauthorized();
            await FileBundles.ResetCacheAsync();
            await using (ICacheDataProvider cacheDP = YetaWF.Core.IO.Caching.GetStaticSmallObjectCacheProvider()) {
                ICacheClearable? clearableDP = cacheDP as ICacheClearable;
                if (clearableDP != null)
                    await clearableDP.ClearAllAsync();
            }
            return Done(__ResStr("clearJsCssAll", "JavaScript/CSS bundles and cached static small objects have been cleared"));
        })
            .ExcludeDemoMode();

        group.MapPost(ImportModule, async (HttpContext context, Guid __ModuleGuid, IFormFile __filename, 
                [FromQuery] Guid currentPageGuid, [FromQuery] string modulePane, [FromQuery] Modules.PageControlModule.Location moduleLocation) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();

            FileUpload upload = new FileUpload();
            string tempName = await upload.StoreTempPackageFileAsync(__filename);

            List<string> errorList = new List<string>();
            bool success = await ModuleDefinition.ImportAsync(tempName, currentPageGuid, true, modulePane, moduleLocation == Modules.PageControlModule.Location.Top, errorList);
            await FileSystem.TempFileSystemProvider.DeleteFileAsync(tempName);

            string errs = "";
            if (errorList.Count > 0) {
                ScriptBuilder sbErr = new ScriptBuilder();
                sbErr.Append(errorList, LeadingNL: true);
                errs = sbErr.ToString();
            }
            if (success) {
                string msg = __ResStr("imported", "\"{0}\" successfully imported(+nl)", __filename.FileName) + errs;
                UploadResponse response = new UploadResponse {
                    Result = $"$YetaWF.confirm('{Utility.JserEncode(msg)}', null, function() {{ $YetaWF.reloadPage(true); }} );",
                };
                return Results.Json(response);
            } else {
                // Anything else is a failure
                throw new Error(__ResStr("cantImport", "Can't import {0}:(+nl)", __filename.FileName) + errs);
            }
        })
            .ResourceAuthorize(CoreInfo.Resource_ModuleImport)
            .ExcludeDemoMode();

        group.MapPost(RemoveModule, async (HttpContext context, Guid __ModuleGuid, string name, string folderGuid, string subFolder, string fileType) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();
            UploadRemoveResponse response = new UploadRemoveResponse();
            return Results.Json(response);
        })
            .ResourceAuthorize(CoreInfo.Resource_ModuleImport)
            .ExcludeDemoMode();

        group.MapPost(ImportPage, async (HttpContext context, Guid __ModuleGuid, IFormFile __filename) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();

            FileUpload upload = new FileUpload();
            string tempName = await upload.StoreTempPackageFileAsync(__filename);

            List<string> errorList = new List<string>();
            PageDefinition.ImportInfo info = await PageDefinition.ImportAsync(tempName, errorList);
            await FileSystem.TempFileSystemProvider.DeleteFileAsync(tempName);

            string errs = "";
            if (errorList.Count > 0) {
                ScriptBuilder sbErr = new ScriptBuilder();
                sbErr.Append(errorList, LeadingNL: true);
                errs = sbErr.ToString();
            }
            if (info.Success) {
                string msg = __ResStr("imported", "\"{0}\" successfully imported(+nl)", __filename.FileName) + errs;
                UploadResponse response = new UploadResponse {
                    Result = $"$YetaWF.confirm('{Utility.JserEncode(msg)}', null, function() {{ window.location.assign('{Utility.JserEncode(info.Url)}'); }} );",
                };
                return Results.Json(response);
            } else {
                // Anything else is a failure
                throw new Error(__ResStr("cantImport", "Can't import {0}:(+nl)", __filename.FileName) + errs);
            }
        })
            .ResourceAuthorize(CoreInfo.Resource_PageImport)
            .ExcludeDemoMode();

        group.MapPost(RemovePage, async (HttpContext context, Guid __ModuleGuid, string name, string folderGuid, string subFolder, string fileType) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();
            UploadRemoveResponse response = new UploadRemoveResponse();
            return Results.Json(response);
        })
            .ResourceAuthorize(CoreInfo.Resource_PageImport)
            .ExcludeDemoMode();


        RouteGroupBuilder groupDownload = endpoints.MapGroup(GetPackageApiRoute(package, typeof(PageControlModuleEndpoints)))
            .RequireAuthorization();

        groupDownload.MapGet(ExportPage, async (HttpContext context, [FromQuery] Guid __ModuleGuid, Guid pageGuid, long cookieToReturn) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();
            PageDefinition page = await PageDefinition.LoadAsync(pageGuid) ?? throw new InternalError($"Page {pageGuid} not found");
            YetaWFZipFile zipFile = await page.ExportAsync();
            return await ZippedFileResult.ZipFileAsync(context, zipFile, cookieToReturn);
        })
            .ResourceAuthorize(CoreInfo.Resource_PageExport)
            .ExcludeDemoMode();


        RouteGroupBuilder groupSwitch = endpoints.MapGroup(GetPackageApiRoute(package, typeof(PageControlModuleEndpoints)))
            .RequireAuthorization();

        groupSwitch.MapGet(SwitchToEdit, async (HttpContext context, Guid __ModuleGuid) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();
            Manager.PageControlShown = false;
            Manager.EditMode = true;
            string url = AddUrlPayload(Manager.ReturnToUrl, true, null);
            return Results.Redirect(url);
        });

        groupSwitch.MapGet(SwitchToView, async (HttpContext context, Guid __ModuleGuid) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();
            Manager.PageControlShown = false;
            Manager.EditMode = false;
            string url = AddUrlPayload(Manager.ReturnToUrl, true, null);
            return Results.Redirect(url);
        });
    }
}
