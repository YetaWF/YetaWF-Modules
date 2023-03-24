/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Packages.Endpoints;

namespace YetaWF.Modules.Packages.Modules;

public class ImportDataModuleDataProvider : ModuleDefinitionDataProvider<Guid, ImportDataModule>, IInstallableModel { }

[ModuleGuid("{3AEB70E7-A60F-4306-BDE3-C4355B9B30A4}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class ImportDataModule : ModuleDefinition {

    public ImportDataModule() {
        Name = this.__ResStr("modName", "Import Package Data");
        Title = this.__ResStr("modTitle", "Import Package Data");
        Description = this.__ResStr("modSummary", "Used to import all the data for one package. Only data for the current site is imported. A site backup contains multiple zip files, each containing data a backup for a particular package. These zip files can be restored, or imported, using the Import Package Data Module. The Import Package Data Module is accessible using Admin > Panel > Packages.");
        Print = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new ImportDataModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    public override List<RoleDefinition> ExtraRoles {
        get {
            return new List<RoleDefinition>() {
                new RoleDefinition("Imports",
                    this.__ResStr("roleImportsC", "Import Package Data"), this.__ResStr("roleImports", "The role has permission to import package data"),
                    this.__ResStr("userImportsC", "Import Package Data"), this.__ResStr("userImports", "The user has permission to import package data")),
            };
        }
    }

    public class ImportDataModel {
        [UIHint("FileUpload1")]
        public FileUpload1? UploadFile { get; set; }
    }

    [Permission("Imports")]
    public async Task<ActionInfo> RenderModuleAsync() {
        ImportDataModel model = new ImportDataModel { };
        model.UploadFile = new FileUpload1 {
            SelectButtonText = this.__ResStr("btnImport", "Import Data..."),
            SaveURL = Utility.UrlFor<ImportDataModuleEndpoints>(ImportDataModuleEndpoints.ImportPackageData),
            RemoveURL = Utility.UrlFor<ImportDataModuleEndpoints>(ImportDataModuleEndpoints.RemovePackageData),
        };
        return await RenderAsync(model);
    }
}