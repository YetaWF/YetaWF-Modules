/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using YetaWF.Core.Components;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core.Upload;

namespace YetaWF.Modules.Packages.Endpoints;

public class ImportDataModuleEndpoints : YetaWFEndpoints {

    internal const string ImportPackageData = "ImportPackageData";
    internal const string RemovePackageData = "RemovePackageData";

    private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(ImportDataModuleEndpoints), name, defaultValue, parms); }

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(ImportDataModuleEndpoints)))
            .RequireAuthorization()
            .AntiForgeryToken();


        group.MapPost(ImportPackageData, async (HttpContext context, Guid __ModuleGuid, IFormFile __filename) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("Imports")) return Results.Unauthorized();

            FileUpload upload = new FileUpload();
            string tempName = await upload.StoreTempPackageFileAsync(__filename);

            List<string> errorList = new List<string>();
            bool success = await Package.ImportDataAsync(tempName, errorList);
            await FileSystem.TempFileSystemProvider.DeleteFileAsync(tempName);

            string errs = "";
            if (errorList.Count > 0) {
                ScriptBuilder sbErr = new ScriptBuilder();
                sbErr.Append(errorList, LeadingNL: true);
                errs = sbErr.ToString();
            }

            if (success) {
                string msg = __ResStr("importedData", "\"{0}\" successfully imported - These settings won't take effect until the site is restarted(+nl)", __filename.FileName) + errs;
                UploadResponse response = new UploadResponse {
                    Result = $"$YetaWF.confirm('{Utility.JserEncode(msg)}', null, function() {{ $YetaWF.reloadPage(true); }} );",
                };
                return Results.Json(response);
            } else {
                // Anything else is a failure
                throw new Error(__ResStr("cantImportData", "Can't import {0}:(+nl)", __filename.FileName) + errs);
            }
        })
            .ExcludeDemoMode();

        group.MapPost(RemovePackageData, async (HttpContext context, Guid __ModuleGuid, string filename) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("Imports")) return Results.Unauthorized();
            UploadRemoveResponse response = new UploadRemoveResponse();
            return Results.Json(response);
        })
            .ExcludeDemoMode();
    }
}
