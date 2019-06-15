/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core.Upload;
using YetaWF.Core.Components;
#if MVC6
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
#else
using System.Web;
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Packages.Controllers {

    public class ImportDataModuleController : ControllerImpl<YetaWF.Modules.Packages.Modules.ImportDataModule> {

        public class ImportDataModel {
            [UIHint("FileUpload1")]
            public FileUpload1 UploadFile { get; set; }
        }

        [AllowGet]
        [Permission("Imports")]
        public ActionResult ImportData() {
            ImportDataModel model = new ImportDataModel  { };
            model.UploadFile = new FileUpload1 {
                SelectButtonText = this.__ResStr("btnImport", "Import Data..."),
                SaveURL = GetActionUrl("ImportPackageData", new { __ModuleGuid = Module.ModuleGuid }),
                RemoveURL = GetActionUrl("RemovePackageData", new { __ModuleGuid = Module.ModuleGuid } ),
            };
            return View(model);
        }

        [AllowPost]
        [Permission("Imports")]
        [ExcludeDemoMode]
#if MVC6
        public async Task<ActionResult> ImportPackageData(IFormFile __filename)
#else
        public async Task<ActionResult> ImportPackageData(HttpPostedFileBase __filename)
#endif
        {
            FileUpload upload = new FileUpload();
            string tempName = await upload.StoreTempPackageFileAsync(__filename);
            List<string> errorList = new List<string>();
            bool success = await Package.ImportDataAsync(tempName, errorList);
            await FileSystem.TempFileSystemProvider.DeleteFileAsync(tempName);

            string errs = "";
            if (errorList.Count > 0) {
                ScriptBuilder sbErr = new ScriptBuilder();
                sbErr.Append(errorList, LeadingNL: true);
                errs = sbErr.ToString();
            }

            if (success) {
                string msg = this.__ResStr("importedData", "\"{0}\" successfully imported - These settings won't take effect until the site is restarted(+nl)", __filename.FileName) + errs;
                UploadResponse resp = new UploadResponse {
                    Result = $"$YetaWF.confirm('{Utility.JserEncode(msg)}', null, function() {{ $YetaWF.reloadPage(true); }} );",
                };
                return new YJsonResult { Data = resp };
            } else {
                // Anything else is a failure
                throw new Error(this.__ResStr("cantImportData", "Can't import {0}:(+nl)", __filename.FileName) + errs);
            }
        }
        [AllowPost]
        [Permission("Imports")]
        [ExcludeDemoMode]
        public ActionResult RemovePackageData(string filename) {
            // there is nothing to remove because we already imported the file
            UploadRemoveResponse resp = new UploadRemoveResponse();
            return new YJsonResult { Data = resp };
        }
    }
}