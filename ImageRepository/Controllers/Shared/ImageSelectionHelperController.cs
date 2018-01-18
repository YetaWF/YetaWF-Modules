/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using System;
using System.IO;
using YetaWF.Core.Addons;
using YetaWF.Core.Controllers;
using YetaWF.Core.Extensions;
using YetaWF.Core.Identity;
using YetaWF.Core.Image;
using YetaWF.Core.Localize;
using YetaWF.Core.Support;
using YetaWF.Core.Upload;
using YetaWF.Modules.ImageRepository.Views.Shared;
#if MVC6
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
#else
using System.Web;
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.ImageRepository.Controllers.Shared {
    public class ImageSelectionHelperController : YetaWFController {

        [AllowPost]
        [ResourceAuthorize(CoreInfo.Resource_UploadImages)]
#if MVC6
        public ActionResult SaveImage(IFormFile __filename, string folderGuid, string subFolder, string fileType)
#else
        public ActionResult SaveImage(HttpPostedFileBase __filename, string folderGuid, string subFolder, string fileType)
#endif
        {
            FileUpload upload = new FileUpload();
            string storagePath = ImageSelectionInfo.StoragePath(new Guid(folderGuid), subFolder, fileType);
            string name = upload.StoreFile(__filename, storagePath, MimeSection.ImageUse, uf => {
                return Path.GetFileName(uf.FileName);
            });

            System.Drawing.Size size = ImageSupport.GetImageSize(name, storagePath);

            ScriptBuilder sb = new ScriptBuilder();
            // Upload control considers Json result a success. result has a function to execute, newName has the file name
            sb.Append("{\n");
            sb.Append("  \"result\":");
            sb.Append("      \"Y_Confirm(\\\"{0}\\\");\",", this.__ResStr("saveImageOK", "Image \\\\\\\"{0}\\\\\\\" successfully uploaded", YetaWFManager.JserEncode(name)));
            sb.Append("  \"filename\": \"{0}\",\n", YetaWFManager.JserEncode(name));
            sb.Append("  \"realFilename\": \"{0}\",\n", YetaWFManager.JserEncode(__filename.FileName));
            sb.Append("  \"attributes\": \"{0}\",\n", this.__ResStr("imgAttr", "{0} x {1} (w x h)", size.Width, size.Height));
            sb.Append("  \"list\": \"");
            foreach (var f in ImageSelectionInfo.ReadFiles(new Guid(folderGuid), subFolder, fileType))
                sb.Append("<option title=\\\"{0}\\\">{0}</option>", YetaWFManager.JserEncode(f.RemoveStartingAt(ImageSupport.ImageSeparator)));
            sb.Append("\"\n");
            sb.Append("}");
            return new YJsonResult { Data = sb.ToString() };
        }

        [AllowPost]
        [ResourceAuthorize(CoreInfo.Resource_RemoveImages)]
        public ActionResult RemoveSelectedImage(string name, string folderGuid, string subFolder, string fileType) {

            name = name.RemoveStartingAt(ImageSupport.ImageSeparator);

            FileUpload upload = new FileUpload();
            string storagePath = ImageSelectionInfo.StoragePath(new Guid(folderGuid), subFolder, fileType);
            upload.RemoveFile(name, storagePath);

            ScriptBuilder sb = new ScriptBuilder();
            // return new list of selected files
            sb.Append("{\n");
            sb.Append("  \"result\": ");
            sb.Append("      \"Y_Confirm(\\\"{0}\\\");\",", this.__ResStr("removeImageOK", "Image \\\\\\\"{0}\\\\\\\" successfully removed", YetaWFManager.JserEncode(name)));
            sb.Append("  \"list\": \"");
            foreach (var f in ImageSelectionInfo.ReadFiles(new Guid(folderGuid), subFolder, fileType))
                sb.Append("<option title=\\\"{0}\\\">{0}</option>", YetaWFManager.JserEncode(f.RemoveStartingAt(ImageSupport.ImageSeparator)));
            sb.Append("\"\n");
            sb.Append("}");
            return new YJsonResult { Data = sb.ToString() };
        }
    }
}
