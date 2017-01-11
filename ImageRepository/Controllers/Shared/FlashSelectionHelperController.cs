/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using System;
using System.Web;
using System.Web.Mvc;
using YetaWF.Core.Addons;
using YetaWF.Core.Controllers;
using YetaWF.Core.Extensions;
using YetaWF.Core.Identity;
using YetaWF.Core.Image;
using YetaWF.Core.Localize;
using YetaWF.Core.Support;
using YetaWF.Core.Upload;
using YetaWF.Modules.ImageRepository.Views.Shared;

namespace YetaWF.Modules.ImageRepository.Controllers.Shared {
    public class FlashSelectionHelperController : YetaWFController {

        [HttpPost]
        [ResourceAuthorize(CoreInfo.Resource_UploadImages)]
        public ActionResult SaveFlashImage(HttpPostedFileBase __filename, string folderGuid, string subFolder, string fileType) {
            FileUpload upload = new FileUpload();
            string storagePath = FlashSelectionInfo.StoragePath(new Guid(folderGuid), subFolder, fileType);
            string name = upload.StoreFile(__filename, storagePath, m => m.FlashUse, uf => uf.FileName);

            ScriptBuilder sb = new ScriptBuilder();
            // Upload control considers Json result a success. result has a function to execute, newName has the file name
            sb.Append("{\n");
            sb.Append("  \"result\":");
            sb.Append("      \"Y_Confirm(\\\"{0}\\\");\",", this.__ResStr("saveImageOK", "Image \\\\\\\"{0}\\\\\\\" successfully uploaded", YetaWFManager.JserEncode(__filename.FileName)));
            sb.Append("  \"filename\": \"{0}\",\n", YetaWFManager.JserEncode(name));
            sb.Append("  \"realFilename\": \"{0}\",\n", YetaWFManager.JserEncode(__filename.FileName));
            sb.Append("  \"list\": \"");
            foreach (var f in FlashSelectionInfo.ReadFiles(new Guid(folderGuid), subFolder, fileType))
                sb.Append("<option title=\\\"{0}\\\">{0}</option>", YetaWFManager.JserEncode(f.RemoveStartingAt(ImageSupport.ImageSeparator)));
            sb.Append("\"\n");
            sb.Append("}");
            return new JsonResult { Data = sb.ToString() };
        }

        [HttpPost]
        [ResourceAuthorize(CoreInfo.Resource_RemoveImages)]
        public ActionResult RemoveSelectedFlashImage(string name, string folderGuid, string subFolder, string fileType) {

            name = name.RemoveStartingAt(ImageSupport.ImageSeparator);

            FileUpload upload = new FileUpload();
            string storagePath = FlashSelectionInfo.StoragePath(new Guid(folderGuid), subFolder, fileType);
            upload.RemoveFile(name, storagePath);

            ScriptBuilder sb = new ScriptBuilder();
            // return new list of selected files
            sb.Append("{\n");
            sb.Append("  \"result\": ");
            sb.Append("      \"Y_Confirm(\\\"{0}\\\");\",", this.__ResStr("removeImageOK", "Image \\\\\\\"{0}\\\\\\\" successfully removed", YetaWFManager.JserEncode(name)));
            sb.Append("  \"list\": \"");
            foreach (var f in FlashSelectionInfo.ReadFiles(new Guid(folderGuid), subFolder, fileType))
                sb.Append("<option title=\\\"{0}\\\">{0}</option>", YetaWFManager.JserEncode(f.RemoveStartingAt(ImageSupport.ImageSeparator)));
            sb.Append("\"\n");
            sb.Append("}");
            return new JsonResult { Data = sb.ToString() };
        }
    }
}
