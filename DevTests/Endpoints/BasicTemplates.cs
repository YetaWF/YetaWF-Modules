/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using YetaWF.Core.Components;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core.Upload;

namespace YetaWF.Modules.DevTests.Endpoints {

    public class BasicTemplatesModuleEndpoints : YetaWFEndpoints {

        internal const string UploadSomething = "UploadSomething";
        internal const string RemoveSomething = "RemoveSomething";

        private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(BasicTemplatesModuleEndpoints), name, defaultValue, parms); }

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(BasicTemplatesModuleEndpoints)))
                .RequireAuthorization()
                .AntiForgeryToken();

            // Saves an uploaded image file. Works in conjunction with the BasicTemplatesModule template and YetaWF.Core.Upload.FileUpload.
            group.MapPost(UploadSomething, async (HttpContext context, Guid __ModuleGuid, IFormFile __filename, string? __lastInternalName) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized()) return Results.Unauthorized();
                // Save the uploaded file as a temp file
                FileUpload upload = new FileUpload();
                string tempName = await upload.StoreTempImageFileAsync(__filename);
                // do something with the uploaded file "tempName"
                //...
                // Delete the temp file just uploaded
                await upload.RemoveTempFileAsync(tempName);

                bool success = true;
                string msg = __ResStr("uploadSuccess", "File {0} successfully uploaded", __filename.FileName);

                if (success) {
                    UploadResponse resp = new UploadResponse {
                        Result = $"$YetaWF.confirm('{Utility.JserEncode(msg)}', null, function() {{ /*add some javascript like  $YetaWF.reloadPage(true); */ }} );",
                        FileName = tempName,
                        FileNamePlain = tempName,
                        RealFileName = __filename.FileName,
                        Attributes = __ResStr("someAttr", "File Info Here"),
                    };
                    return Results.Json(resp);
                } else {
                    // Anything else is a failure
                    throw new Error(msg);
                }
            });

            group.MapPost(RemoveSomething, async (HttpContext context, Guid __ModuleGuid, string? __internalName) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized()) return Results.Unauthorized();
                FileUpload upload = new FileUpload();
                // there is nothing to remove because we already deleted the file right after uploading it
                // await upload.RemoveTempFileAsync(__internalName);
                UploadRemoveResponse resp = new UploadRemoveResponse();
                return Results.Json(resp);
            });
        }
    }
}
