/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Packages#License */

using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core.Upload;
using YetaWF.Core.Views.Shared;

namespace YetaWF.Modules.Packages.Controllers {

    public class ImportModuleController : ControllerImpl<YetaWF.Modules.Packages.Modules.ImportModule> {

        public class ImportModel {
            [UIHint("FileUpload1")]
            public FileUpload1 UploadFile { get; set; }
        }

        [HttpGet]
        [Permission("Imports")]
        public ActionResult Import() {
            ImportModel model = new ImportModel  { };
            model.UploadFile = new FileUpload1 {
                SelectButtonText = this.__ResStr("btnImport", "Import Binary or Source Code Package..."),
                SaveURL = GetActionUrl("ImportPackage", new { __ModuleGuid = Module.ModuleGuid }),
                RemoveURL = GetActionUrl("RemovePackage", new { __ModuleGuid = Module.ModuleGuid } ),
            };
            return View(model);
        }

        [HttpPost]
        [Permission("Imports")]
        public ActionResult ImportPackage(HttpPostedFileBase __filename) {
            FileUpload upload = new FileUpload();
            string tempName = upload.StoreTempPackageFile(__filename);
            List<string> errorList = new List<string>();
            bool success = Package.Import(tempName, errorList);
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
                sb.Append("{{ \"result\": \"Y_Confirm(\\\"{0}\\\", null, function() {{ window.location.reload(); }} ); \" }}",
                    YetaWFManager.JserEncode(YetaWFManager.JserEncode(this.__ResStr("imported", "\"{0}\" successfully imported - YOU MUST RESTART THE SITE FOR PROPER OPERATION(+nl)", __filename.FileName) + errs))
                );
                //System.Web.HttpRuntime.UnloadAppDomain();
                //System.Web.HttpContext.Current.Response.Redirect("/");
                return new JsonResult { Data = sb.ToString() };
            } else {
                // Anything else is a failure
                sb.Append(this.__ResStr("cantImport", "Can't import {0}:(+nl)"), __filename.FileName);
                sb.Append(errs);
                throw new Error(sb.ToString());
            }
        }
        [HttpPost]
        [Permission("Imports")]
        public ActionResult RemovePackage(string filename) {
            // there is nothing to remove because we already imported the file
            return new EmptyResult();
        }
    }
}