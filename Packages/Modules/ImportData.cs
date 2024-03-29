/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using System;
using System.Collections.Generic;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Packages.Modules {

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
    }
}