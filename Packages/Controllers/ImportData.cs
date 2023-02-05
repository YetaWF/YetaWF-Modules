/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using Microsoft.AspNetCore.Mvc;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Packages.Endpoints;

namespace YetaWF.Modules.Packages.Controllers {

    public class ImportDataModuleController : ControllerImpl<YetaWF.Modules.Packages.Modules.ImportDataModule> {

        public class ImportDataModel {
            [UIHint("FileUpload1")]
            public FileUpload1? UploadFile { get; set; }
        }

        [AllowGet]
        [Permission("Imports")]
        public ActionResult ImportData() {
            ImportDataModel model = new ImportDataModel  { };
            model.UploadFile = new FileUpload1 {
                SelectButtonText = this.__ResStr("btnImport", "Import Data..."),
                SaveURL = Utility.UrlFor<ImportDataModuleEndpoints>(ImportDataModuleEndpoints.ImportPackageData, new { __ModuleGuid = Module.ModuleGuid }),
                RemoveURL = Utility.UrlFor<ImportDataModuleEndpoints>(ImportDataModuleEndpoints.RemovePackageData, new { __ModuleGuid = Module.ModuleGuid } ),
            };
            return View(model);
        }
    }
}