/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using System;
using YetaWF.Core.Addons;
using YetaWF.Core.Controllers;
using YetaWF.Core.Extensions;
using YetaWF.Core.Identity;
using YetaWF.Core.Image;
using YetaWF.Core.Localize;
using YetaWF.Core.Support;
using YetaWF.Core.Upload;
using System.Threading.Tasks;
using YetaWF.Modules.ImageRepository.Components;
using YetaWF.Core.Components;
#if MVC6
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
#else
using System.Web;
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.ImageRepository.Controllers {

    public class FlashSelectionController : YetaWFController {

        [AllowPost]
        [ResourceAuthorize(CoreInfo.Resource_UploadImages)]
#if MVC6
        public async Task<ActionResult> SaveFlashImage(IFormFile __filename, string folderGuid, string subFolder, string fileType)
#else
        public async Task<ActionResult> SaveFlashImage(HttpPostedFileBase __filename, string folderGuid, string subFolder, string fileType)
#endif
        {
            FileUpload upload = new FileUpload();
            string storagePath = ImageSelectionInfo.StoragePath(new Guid(folderGuid), subFolder, fileType);
            string namePlain = await upload.StoreFileAsync(__filename, storagePath, MimeSection.FlashUse, uf => uf.FileName);
            string name = namePlain;

            HtmlBuilder hb = new HtmlBuilder();
            foreach (var f in await ImageSelectionInfo.ReadFilesAsync(new Guid(folderGuid), subFolder, fileType)) {
                string fPlain = f.RemoveStartingAt(ImageSupport.ImageSeparator);
                string sel = "";
                if (fPlain == namePlain) {
                    sel = " selected";
                    name = f;
                }
                hb.Append(string.Format("<option title='{0}' value='{1}'{2}>{3}</option>", YetaWFManager.HtmlAttributeEncode(fPlain), YetaWFManager.HtmlAttributeEncode(f), sel, YetaWFManager.HtmlEncode(fPlain)));
            }

            // Upload control considers Json result a success. result has a function to execute, newName has the file name
            UploadResponse response = new UploadResponse {
                Result = $@"$YetaWF.confirm('{YetaWFManager.JserEncode(this.__ResStr("saveImageOK", "Image \"{0}\" successfully uploaded", namePlain))}');",
                FileName = name,
                FileNamePlain = namePlain,
                RealFileName = __filename.FileName,
                List = hb.ToString(),
            };

            return new YJsonResult { Data = response };
        }

        [AllowPost]
        [ResourceAuthorize(CoreInfo.Resource_RemoveImages)]
        public async Task<ActionResult> RemoveSelectedFlashImage(string name, string folderGuid, string subFolder, string fileType) {

            string namePlain = name.RemoveStartingAt(ImageSupport.ImageSeparator);

            FileUpload upload = new FileUpload();
            string storagePath = ImageSelectionInfo.StoragePath(new Guid(folderGuid), subFolder, fileType);
            await upload.RemoveFileAsync(namePlain, storagePath);

            HtmlBuilder hb = new HtmlBuilder();
            foreach (var f in await ImageSelectionInfo.ReadFilesAsync(new Guid(folderGuid), subFolder, fileType)) {
                string fPlain = f.RemoveStartingAt(ImageSupport.ImageSeparator);
                hb.Append(string.Format("<option title='{0}' value='{1}'>{2}</option>", YetaWFManager.HtmlAttributeEncode(fPlain), YetaWFManager.HtmlAttributeEncode(f), YetaWFManager.HtmlEncode(fPlain)));
            }
            UploadRemoveResponse response = new UploadRemoveResponse {
                Result = $@"$YetaWF.confirm('{YetaWFManager.JserEncode(this.__ResStr("removeImageOK", "Image \"{0}\" successfully removed", namePlain))}');",
                List = hb.ToString(),
            };

            return new YJsonResult { Data = response };
        }
    }
}
