/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using System;
using System.Collections.Generic;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Packages.Modules {

    public class ImportModuleDataProvider : ModuleDefinitionDataProvider<Guid, ImportModule>, IInstallableModel { }

    [ModuleGuid("{9AC300E6-099B-47d3-A6F6-282DA545998D}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class ImportModule : ModuleDefinition {

        public ImportModule() {
            Name = this.__ResStr("modName", "Import Package (Binary or Source Code Package)");
            Title = this.__ResStr("modTitle", "Import Package (Binary or Source Code Package)");
            Description = this.__ResStr("modSummary", "Imports binary or source code packages");
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
    }
}