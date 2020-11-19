/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */

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

    public class ImageSelectionController : YetaWFController {

        [AllowPost]
        [ResourceAuthorize(CoreInfo.Resource_UploadImages)]
#if MVC6
        public async Task<ActionResult> SaveImage(IFormFile __filename, string folderGuid, string subFolder, string fileType)
#else
        public async Task<ActionResult> SaveImage(HttpPostedFileBase __filename, string folderGuid, string subFolder, string fileType)
#endif
        {
            FileUpload upload = new FileUpload();
            string storagePath = ImageSelectionInfo.StoragePath(new Guid(folderGuid), subFolder, fileType);
            string namePlain = await upload.StoreFileAsync(__filename, storagePath, MimeSection.ImageUse);
            string name = namePlain;

            System.Drawing.Size size = ImageSupport.GetImageSize(namePlain, storagePath);

            HtmlBuilder hb = new HtmlBuilder();
            foreach (var f in await ImageSelectionInfo.ReadFilesAsync(new Guid(folderGuid), subFolder, fileType)) {
                string plain = f.RemoveStartingAt(ImageSupport.ImageSeparator);
                string sel = "";
                if (plain == namePlain) {
                    sel = " selected";
                    name = f;
                }
                hb.Append(string.Format("<option title='{0}' value='{1}'{2}>{0}</option>", Utility.HtmlAttributeEncode(plain), Utility.HtmlAttributeEncode(f), sel));
            }

            // Upload control considers Json result a success. result has a function to execute, newName has the file name
            UploadResponse response = new UploadResponse {
                Result = $@"$YetaWF.confirm('{Utility.JserEncode(this.__ResStr("saveImageOK", "Image \"{0}\" successfully uploaded", namePlain))}');",
                FileName = name,
                FileNamePlain = namePlain,
                RealFileName = __filename.FileName,
                Attributes = this.__ResStr("imgAttr", "{0} x {1} (w x h)", size.Width, size.Height),
                List = hb.ToString(),
            };

            return new YJsonResult { Data = response };
        }

        [AllowPost]
        [ResourceAuthorize(CoreInfo.Resource_RemoveImages)]
        public async Task<ActionResult> RemoveSelectedImage(string name, string folderGuid, string subFolder, string fileType) {

            string namePlain = name.RemoveStartingAt(ImageSupport.ImageSeparator);

            FileUpload upload = new FileUpload();
            string storagePath = ImageSelectionInfo.StoragePath(new Guid(folderGuid), subFolder, fileType);
            await upload.RemoveFileAsync(namePlain, storagePath);

            HtmlBuilder hb = new HtmlBuilder();
            foreach (var f in await ImageSelectionInfo.ReadFilesAsync(new Guid(folderGuid), subFolder, fileType)) {
                string fPlain = f.RemoveStartingAt(ImageSupport.ImageSeparator);
                hb.Append(string.Format("<option title='{0}' value='{1}'>{2}</option>", Utility.HtmlAttributeEncode(fPlain), Utility.HtmlAttributeEncode(f), Utility.HtmlEncode(fPlain)));
            }
            UploadRemoveResponse response = new UploadRemoveResponse {
                Result = $@"$YetaWF.confirm('{Utility.JserEncode(this.__ResStr("removeImageOK", "Image \"{0}\" successfully removed", namePlain))}');",
                List = hb.ToString(),
            };

            return new YJsonResult { Data = response };
        }
    }
}
