/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.Identity;
using YetaWF.Core.Image;
using YetaWF.Core.Localize;
using YetaWF.Core.Support;
using YetaWF.Core.Upload;

namespace YetaWF.Modules.ComponentsHTML.Controllers {

    /// <summary>
    /// FileUpload1 template support.
    /// </summary>
    public class FileUpload1Controller : YetaWFController {

        /// <summary>
        /// Saves an uploaded image file. Works in conjunction with the FileUpload1 template and YetaWF.Core.Upload.FileUpload.
        /// </summary>
        /// <param name="__filename">Describes the image file being uploaded.</param>
        /// <param name="__lastInternalName">The name of a previously uploaded file (if any) that is being replaced by the current file being uploaded.</param>
        /// <returns>An action result.</returns>
        [AllowPost]
        [ResourceAuthorize(CoreInfo.Resource_UploadImages)]
        public async Task<ActionResult> SaveImage(IFormFile __filename, string __lastInternalName) {
            FileUpload upload = new FileUpload();
            string tempName = await upload.StoreTempImageFileAsync(__filename);

            if (!string.IsNullOrWhiteSpace(__lastInternalName)) // delete the previous file we had open
                await upload.RemoveTempFileAsync(__lastInternalName);

            (int width, int height) = await ImageSupport.GetImageSizeAsync(tempName);

            UploadResponse resp = new UploadResponse {
                Result = $"$YetaWF.confirm('{Utility.JserEncode(this.__ResStr("saveImageOK", "Image \"{0}\" successfully uploaded", __filename.FileName))}');",
                FileName = tempName,
                FileNamePlain = tempName,
                RealFileName = __filename.FileName,
                Attributes = this.__ResStr("imgAttr", "{0} x {1} (w x h)", width,height),
            };

            return new YJsonResult { Data = resp };
        }

        /// <summary>
        /// Removes an uploaded image file. Works in conjunction with the FileUpload1 template and YetaWF.Core.Upload.FileUpload.
        /// </summary>
        /// <param name="__filename">Describes the image file being uploaded.</param>
        /// <param name="__internalName">The name of the uploaded file that is to be removed.</param>
        /// <returns>An action result.</returns>
        [AllowPost]
        [ResourceAuthorize(CoreInfo.Resource_RemoveImages)]
        public async Task<ActionResult> RemoveImage(string __filename, string __internalName) {
            FileUpload upload = new FileUpload();
            await upload.RemoveTempFileAsync(__internalName);
            UploadRemoveResponse resp = new UploadRemoveResponse();
            return new YJsonResult { Data = resp };
        }
    }
}
