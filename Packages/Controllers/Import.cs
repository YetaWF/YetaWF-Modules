/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Packages#License */

using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core.Upload;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Packages.Modules;

namespace YetaWF.Modules.Packages.Controllers {

    public class ImportModuleController : ControllerImpl<YetaWF.Modules.Packages.Modules.ImportModule> {

        [Header("Provide a remote or local ZIP file to import a binary or source code package.")]
        public class ImportModel {
            [Category("Remote ZIP File"), Caption("ZIP File"), Description("Enter the Url of a ZIP file to download - Used to import a package (binary or source code package)")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Remote), StringLength(Globals.MaxUrl), UrlValidation, Required, Trim]
            public string RemoteFile { get; set; }

            [Category("Remote ZIP File"), Caption("Submit"), Description("Click to download and install the package")]
            [UIHint("FormButton"), ReadOnly]
            public FormButton RemoteGo { get; set; }

            [Category("Local ZIP File"), Caption("ZIP File"), Description("Select a local ZIP file - Used to import a package (binary or source code package)")]
            [UIHint("FileUpload1")]
            public FileUpload1 UploadFile { get; set; }

            public void Update(ImportModule mod, ImportModuleController ctrl) {
                UploadFile = new FileUpload1 {
                    SelectButtonText = this.__ResStr("btnImport", "Import Binary or Source Code Package..."),
                    SaveURL = ctrl.GetActionUrl("ImportPackage", new { __ModuleGuid = mod.ModuleGuid }),
                    RemoveURL = ctrl.GetActionUrl("RemovePackage", new { __ModuleGuid = mod.ModuleGuid }),
                };
                RemoteGo = new FormButton() {
                    Text = "Download and Install",
                    ButtonType = ButtonTypeEnum.Submit,
                };
            }
        }

        [HttpGet]
        [Permission("Imports")]
        public ActionResult Import() {
            ImportModel model = new ImportModel  { };
            model.Update(Module, this);
            return View(model);
        }

        [HttpPost]
        [Permission("Imports")]
        [ExcludeDemoMode]
        public ActionResult Import_Partial(ImportModel model) {
            model.Update(Module, this);
            if (!ModelState.IsValid)
                return PartialView(model);

            // Download the zip file
            FileUpload upload = new FileUpload();
            string tempName = upload.StoreTempPackageFile(model.RemoteFile);

            // import it
            List <string> errorList = new List<string>();
            bool success = Package.Import(tempName, errorList);

            // delete the temp file just uploaded
            System.IO.File.Delete(tempName);

            string msg = FormatMessage(success, errorList, model.RemoteFile);
            if (success) {
                model.RemoteFile = null;
                return FormProcessed(model, msg);
            } else {
                // Anything else is a failure
                return FormProcessed(model, msg);
            }
        }
        private string FormatMessage(bool success, List<string> errorList, string fileName) {
            string errs = "";
            if (errorList.Count > 0) {
                ScriptBuilder sbErr = new ScriptBuilder();
                sbErr.Append(errorList, LeadingNL: false);
                sbErr.Append("(+nl)(+nl)");
                errs = sbErr.ToString();
            }
            if (success) {
                return errs + this.__ResStr("imported", "\"{0}\" successfully imported - YOU MUST RESTART THE SITE FOR PROPER OPERATION", fileName);
            } else {
                // Anything else is a failure
                return errs + this.__ResStr("cantImport", "Can't import {0}:{1}", fileName, errs);
            }
        }

        [HttpPost]
        [Permission("Imports")]
        [ExcludeDemoMode]
        public ActionResult ImportPackage(HttpPostedFileBase __filename) {
            // Save the uploaded file as a temp file
            FileUpload upload = new FileUpload();
            string tempName = upload.StoreTempPackageFile(__filename);
            // Import the package
            List<string> errorList = new List<string>();
            bool success = Package.Import(tempName, errorList);
            // Delete the temp file just uploaded
            System.IO.File.Delete(tempName);

            string msg = FormatMessage(success, errorList, __filename.FileName);

            if (success) {
                // Upload control considers Json result a success
                ScriptBuilder sb = new ScriptBuilder();
                sb.Append("{{ \"result\": \"Y_Confirm(\\\"{0}\\\", null, function() {{ window.location.reload(); }} ); \" }}",
                    YetaWFManager.JserEncode(YetaWFManager.JserEncode(msg))
                );
                //System.Web.HttpRuntime.UnloadAppDomain();
                //System.Web.HttpContext.Current.Response.Redirect("/");
                return new JsonResult { Data = sb.ToString() };
            } else {
                // Anything else is a failure
                throw new Error(msg);
            }
        }
        [HttpPost]
        [Permission("Imports")]
        [ExcludeDemoMode]
        public ActionResult RemovePackage(string filename) {
            // there is nothing to remove because we already imported the file
            return new EmptyResult();
        }
    }
}