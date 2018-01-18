/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using System.Collections.Generic;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core.Upload;
using YetaWF.Core.Views.Shared;
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
        public ActionResult ImportPackageData(IFormFile __filename)
#else
        public ActionResult ImportPackageData(HttpPostedFileBase __filename)
#endif
        {
            FileUpload upload = new FileUpload();
            string tempName = upload.StoreTempPackageFile(__filename);
            List<string> errorList = new List<string>();
            bool success = Package.ImportData(tempName, errorList);
            upload.RemoveTempFile(tempName);

            string errs = "";
            if (errorList.Count > 0) {
                ScriptBuilder sbErr = new ScriptBuilder();
                sbErr.Append(errorList, LeadingNL: true);
                errs = sbErr.ToString();
            }
            ScriptBuilder sb = new ScriptBuilder();
            if (success) {
                // Upload control considers Json result a success
                sb.Append("{{ \"result\": \"Y_Confirm(\\\"{0}\\\", null, function() {{ Y_ReloadPage(true); }} ); \" }}",
                    YetaWFManager.JserEncode(YetaWFManager.JserEncode(this.__ResStr("importedData", "\"{0}\" successfully imported - The site is now restarting...(+nl)", __filename.FileName) + errs))
                );
                return new YJsonResult { Data = sb.ToString() };
            } else {
                // Anything else is a failure
                sb.Append(this.__ResStr("cantImportData", "Can't import {0}:(+nl)"), __filename.FileName);
                sb.Append(errs);
                throw new Error(sb.ToString());
            }
        }
        [AllowPost]
        [Permission("Imports")]
        [ExcludeDemoMode]
        public ActionResult RemovePackageData(string filename) {
            // there is nothing to remove because we already imported the file
            return new EmptyResult();
        }
    }
}