/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Addons.Templates {

    public class FileUpload1 : IAddOnSupport {

        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string area = AreaRegistration.CurrentPackage.AreaName;

            scripts.AddLocalization(area, "StatusUploadNoResp", "Upload failed - The file is too large or the server did not respond");
            scripts.AddLocalization(area, "StatusUploadFailed", "Upload failed - {0}");
            scripts.AddLocalization(area, "FileTypeError", "The file type is invalid and can't be uploaded");
            scripts.AddLocalization(area, "FileSizeError", "The file is too large and can't be uploaded");
            scripts.AddLocalization(area, "FallbackMode", "Your browser doesn't support file uploading");

            return Task.CompletedTask;
        }
    }
}
