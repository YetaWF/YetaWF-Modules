/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Endpoints.Support;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Core.Upload;
using YetaWF.DataProvider;
using YetaWF.Modules.Packages.Endpoints;

namespace YetaWF.Modules.Packages.Modules;

public class ImportModuleDataProvider : ModuleDefinitionDataProvider<Guid, ImportModule>, IInstallableModel { }

[ModuleGuid("{9AC300E6-099B-47d3-A6F6-282DA545998D}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class ImportModule : ModuleDefinition {

    public ImportModule() {
        Name = this.__ResStr("modName", "Import Package (Binary or Source Code Package)");
        Title = this.__ResStr("modTitle", "Import Package (Binary or Source Code Package)");
        Description = this.__ResStr("modSummary", "Used to import a package into your YetaWF site. The package can be a complete package including source code or a binary only package. Source code packages can only be imported on development systems in a Debug build.");
        Print = false;
        DefaultViewName = StandardViews.PropertyListEdit;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new ImportModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    public override List<RoleDefinition> ExtraRoles {
        get {
            return new List<RoleDefinition>() {
                new RoleDefinition("Imports",
                    this.__ResStr("roleImportsC", "Import Packages"), this.__ResStr("roleImports", "The role has permission to import binary and source code packages"),
                    this.__ResStr("userImportsC", "Import Packages"), this.__ResStr("userImports", "The user has permission to import binary and source code packages")),
            };
        }
    }

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
                SaveURL = Utility.UrlFor<ImportModuleEndpoints>(ImportModuleEndpoints.ImportPackage),
                RemoveURL = Utility.UrlFor<ImportModuleEndpoints>(ImportModuleEndpoints.RemovePackage),
            };
            RemoteGo = new FormButton() {
                Text = "Download and Install",
                ButtonType = ButtonTypeEnum.Submit,
            };
        }
    }

    [Permission("Imports")]
    public async Task<ActionInfo> RenderModuleAsync() {
        ImportModel model = new ImportModel { };
        model.Update(this);
        return await RenderAsync(model);
    }

    [Permission("Imports")]
    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(ImportModel model) {
        model.Update(this);
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);

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
            return await FormProcessedAsync(model, msg);
        } else {
            // Anything else is a failure
            return await FormProcessedAsync(model, msg);
        }
    }
}