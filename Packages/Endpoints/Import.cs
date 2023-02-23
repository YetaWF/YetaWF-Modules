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

public class ImportModuleEndpoints : YetaWFEndpoints {

    internal const string ImportPackage = "ImportPackage";
    internal const string RemovePackage = "RemovePackage";

    private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(ImportModuleEndpoints), name, defaultValue, parms); }

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(ImportModuleEndpoints)))
            .RequireAuthorization()
            .AntiForgeryToken();


        group.MapPost(ImportPackage, async (HttpContext context, Guid __ModuleGuid, IFormFile __filename) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("Imports")) return Results.Unauthorized();

            FileUpload upload = new FileUpload();
            string tempName = await upload.StoreTempPackageFileAsync(__filename);

            List<string> errorList = new List<string>();
            bool success = await Package.ImportAsync(tempName, errorList);
            await FileSystem.TempFileSystemProvider.DeleteFileAsync(tempName);

            string msg = FormatMessage(success, errorList, __filename.FileName);

            if (success) {
                UploadResponse response = new UploadResponse {
                    Result = $"$YetaWF.confirm('{Utility.JserEncode(msg)}', null, function() {{ $YetaWF.reloadPage(true); }} );",
                };
                return Results.Json(response);
            } else {
                // Anything else is a failure
                throw new Error(msg);
            }
        })
            .ExcludeDemoMode();

        group.MapPost(RemovePackage, async (HttpContext context, Guid __ModuleGuid, string filename) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("Imports")) return Results.Unauthorized();
            UploadRemoveResponse response = new UploadRemoveResponse();
            return Results.Json(response);
        })
            .ExcludeDemoMode();
    }

    internal static string FormatMessage(bool success, List<string> errorList, string fileName) {
        string errs = "";
        if (errorList.Count > 0) {
            ScriptBuilder sbErr = new ScriptBuilder();
            sbErr.Append(errorList, LeadingNL: false);
            sbErr.Append("(+nl)(+nl)");
            errs = sbErr.ToString();
        }
        if (success) {
            return errs + __ResStr("imported", "\"{0}\" successfully imported - YOU MUST RESTART THE SITE FOR PROPER OPERATION", fileName);
        } else {
            // Anything else is a failure
            return errs + __ResStr("cantImport", "Can't import {0}:{1}", fileName, errs);
        }
    }
}
