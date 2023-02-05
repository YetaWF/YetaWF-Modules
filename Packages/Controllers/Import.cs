/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core.Upload;
using YetaWF.Modules.Packages.Endpoints;
using YetaWF.Modules.Packages.Modules;

namespace YetaWF.Modules.Packages.Controllers {

    public class ImportModuleController : ControllerImpl<YetaWF.Modules.Packages.Modules.ImportModule> {

        [Header("Provide a remote or local ZIP file to import a binary or source code package.")]
        public class ImportModel {
            [Category("Remote ZIP File"), Caption("ZIP File"), Description("Enter the Url of a ZIP file to download - Used to import a package (binary or source code package)")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Remote), StringLength(Globals.MaxUrl), UrlValidation, Required, Trim]
            public string? RemoteFile { get; set; }

            [Category("Remote ZIP File"), Caption("Submit"), Description("Click to download and install the package")]
            [UIHint("FormButton"), ReadOnly]
            public FormButton? RemoteGo { get; set; }

            [Category("Local ZIP File"), Caption("ZIP File"), Description("Select a local ZIP file - Used to import a package (binary or source code package)")]
            [UIHint("FileUpload1")]
            public FileUpload1? UploadFile { get; set; }

            public void Update(ImportModule mod) {
                UploadFile = new FileUpload1 {
                    SelectButtonText = this.__ResStr("btnImport", "Import Binary or Source Code Package..."),
                    SaveURL = Utility.UrlFor<ImportModuleEndpoints>(ImportModuleEndpoints.ImportPackage, new { __ModuleGuid = mod.ModuleGuid }),
                    RemoveURL = Utility.UrlFor<ImportModuleEndpoints>(ImportModuleEndpoints.RemovePackage, new { __ModuleGuid = mod.ModuleGuid }),
                };
                RemoteGo = new FormButton() {
                    Text = "Download and Install",
                    ButtonType = ButtonTypeEnum.Submit,
                };
            }
        }

        [AllowGet]
        [Permission("Imports")]
        public ActionResult Import() {
            ImportModel model = new ImportModel  { };
            model.Update(Module);
            return View(model);
        }

        [AllowPost]
        [Permission("Imports")]
        [ExcludeDemoMode]
        public async Task<ActionResult> Import_Partial(ImportModel model) {
            model.Update(Module);
            if (!ModelState.IsValid)
                return PartialView(model);

            // Download the zip file
            FileUpload upload = new FileUpload();
            string tempName = await upload.StoreTempPackageFileAsync(model.RemoteFile!);

            // import it
            List<string> errorList = new List<string>();
            bool success = await Package.ImportAsync(tempName, errorList);

            // delete the temp file just uploaded
            await FileSystem.TempFileSystemProvider.DeleteFileAsync(tempName);

            string msg = ImportModuleEndpoints.FormatMessage(success, errorList, model.RemoteFile!);
            if (success) {
                model.RemoteFile = null;
                return FormProcessed(model, msg);
            } else {
                // Anything else is a failure
                return FormProcessed(model, msg);
            }
        }
    }
}