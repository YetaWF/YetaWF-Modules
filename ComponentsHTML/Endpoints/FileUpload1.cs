/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Image;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core.Upload;

namespace YetaWF.Modules.ComponentsHTML.Endpoints {

    /// <summary>
    /// Endpoints for the FileUpload1 template.
    /// </summary>
    public class FileUpload1Endpoints : YetaWFEndpoints {

        internal const string SaveImage = "SaveImage";
        internal const string RemoveImage = "RemoveImage";

        private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(FileUpload1Endpoints), name, defaultValue, parms); }

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageRoute(package, typeof(FileUpload1Endpoints)))
                .RequireAuthorization();

            // Saves an uploaded image file. Works in conjunction with the FileUpload1 template and YetaWF.Core.Upload.FileUpload.
            group.MapPost(SaveImage, async (HttpContext context, Guid __ModuleGuid, IFormFile __filename, string? __lastInternalName) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized()) return Results.Unauthorized();
                FileUpload upload = new FileUpload();
                string tempName = await upload.StoreTempImageFileAsync(__filename);

                await upload.RemoveTempFileAsync(__lastInternalName);// delete the previous file we had open

                (int width, int height) = await ImageSupport.GetImageSizeAsync(tempName);

                UploadResponse resp = new UploadResponse {
                    Result = $"$YetaWF.confirm('{Utility.JserEncode(__ResStr("saveImageOK", "Image \"{0}\" successfully uploaded", __filename.FileName))}');",
                    FileName = tempName,
                    FileNamePlain = tempName,
                    RealFileName = __filename.FileName,
                    Attributes = __ResStr("imgAttr", "{0} x {1} (w x h)", width, height),
                };
                return Results.Json(resp);
            })
                .ResourceAuthorize(CoreInfo.Resource_UploadImages);

            // Removes an uploaded image file. Works in conjunction with the FileUpload1 template and YetaWF.Core.Upload.FileUpload.
            group.MapPost(RemoveImage, async (HttpContext context, Guid __ModuleGuid, string? __internalName) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized()) return Results.Unauthorized();
                FileUpload upload = new FileUpload();
                await upload.RemoveTempFileAsync(__internalName);
                UploadRemoveResponse resp = new UploadRemoveResponse();
                return Results.Json(resp);
            })
                .ResourceAuthorize(CoreInfo.Resource_RemoveImages);
        }
    }
}
