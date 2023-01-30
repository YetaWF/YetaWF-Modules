/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Extensions;
using YetaWF.Core.Image;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core.Upload;
using YetaWF.Modules.ImageRepository.Components;

namespace YetaWF.Modules.ImageRepository.Endpoints {

    public class ImageSelectionEndpoints : YetaWFEndpoints {

        internal const string SaveImage = "SaveImage";
        internal const string RemoveSelectedImage = "RemoveSelectedImage";

        private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(ImageSelectionEndpoints), name, defaultValue, parms); }

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(ImageSelectionEndpoints)))
                .RequireAuthorization()
                .AntiForgeryToken();

            // Saves an uploaded image file. Works in conjunction with the ImageSelection template and YetaWF.Core.Upload.FileUpload.
            group.MapPost(SaveImage, async (HttpContext context, Guid __ModuleGuid, IFormFile __filename, string folderGuid, string subFolder, string fileType) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized()) return Results.Unauthorized();

                FileUpload upload = new FileUpload();
                string storagePath = ImageSelectionInfo.StoragePath(new Guid(folderGuid), subFolder, fileType);
                string namePlain = await upload.StoreFileAsync(__filename, storagePath, MimeSection.ImageUse);
                string name = namePlain;

                (int width, int height) = await ImageSupport.GetImageSizeAsync(namePlain, storagePath);

                HtmlBuilder hb = new HtmlBuilder();
                foreach (var f in await ImageSelectionInfo.ReadFilesAsync(new Guid(folderGuid), subFolder, fileType)) {
                    string plain = f.RemoveStartingAt(ImageSupport.ImageSeparator);
                    string sel = "";
                    if (plain == namePlain) {
                        sel = " selected";
                        name = f;
                    }
                    hb.Append(string.Format("<option title='{0}' value='{1}'{2}>{0}</option>", Utility.HAE(plain), Utility.HAE(f), sel));
                }

                // Upload control considers Json result a success. result has a function to execute, newName has the file name
                UploadResponse response = new UploadResponse {
                    Result = $@"$YetaWF.confirm('{Utility.JserEncode(__ResStr("saveImageOK", "Image \"{0}\" successfully uploaded", namePlain))}');",
                    FileName = name,
                    FileNamePlain = namePlain,
                    RealFileName = __filename.FileName,
                    Attributes = __ResStr("imgAttr", "{0} x {1} (w x h)", width, height),
                    List = hb.ToString(),
                };
                return Results.Json(response);
            })
                .ResourceAuthorize(CoreInfo.Resource_UploadImages);

            group.MapPost(RemoveSelectedImage, async (HttpContext context, Guid __ModuleGuid, string name, string folderGuid, string subFolder, string fileType) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized()) return Results.Unauthorized();

                string namePlain = name.RemoveStartingAt(ImageSupport.ImageSeparator);

                FileUpload upload = new FileUpload();
                string storagePath = ImageSelectionInfo.StoragePath(new Guid(folderGuid), subFolder, fileType);
                await upload.RemoveFileAsync(namePlain, storagePath);

                HtmlBuilder hb = new HtmlBuilder();
                foreach (var f in await ImageSelectionInfo.ReadFilesAsync(new Guid(folderGuid), subFolder, fileType)) {
                    string fPlain = f.RemoveStartingAt(ImageSupport.ImageSeparator);
                    hb.Append(string.Format("<option title='{0}' value='{1}'>{2}</option>", Utility.HAE(fPlain), Utility.HAE(f), Utility.HE(fPlain)));
                }
                UploadRemoveResponse response = new UploadRemoveResponse {
                    Result = $@"$YetaWF.confirm('{Utility.JserEncode(__ResStr("removeImageOK", "Image \"{0}\" successfully removed", namePlain))}');",
                    List = hb.ToString(),
                };
                return Results.Json(response);
            })
                .ResourceAuthorize(CoreInfo.Resource_RemoveImages);
        }
    }
}
