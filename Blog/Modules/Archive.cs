/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Blog.Modules {

    public class ArchiveModuleDataProvider : ModuleDefinitionDataProvider<Guid, ArchiveModule>, IInstallableModel { }

    [ModuleGuid("{2b4f7842-370b-4a03-aa09-4e1341f7b87c}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class ArchiveModule : ModuleDefinition {

        public ArchiveModule() {
            Title = this.__ResStr("modTitle", "Blog Archive");
            Name = this.__ResStr("modName", "Archive");
            Description = this.__ResStr("modSummary", "Displays a list of links to blog entries, grouped by month");
            Print = false;
            DefaultViewName = StandardViews.PropertyListEdit;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new ArchiveModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}