/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Blog.Modules {

    public class SummaryModuleDataProvider : ModuleDefinitionDataProvider<Guid, SummaryModule>, IInstallableModel { }

    [ModuleGuid("{2b2c61b6-8f0c-4f39-b927-e09f5e118d86}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    [ModuleCategory("Navigation")]
    public class SummaryModule : ModuleDefinition {

        public SummaryModule() {
            Title = this.__ResStr("modTitle", "Blog Summary");
            Name = this.__ResStr("modName", "Blog Summary");
            Description = this.__ResStr("modSummary", "Displays a summary of blog entries. The most recent blog entries for all categories are shown. This module is typically used in a sidebar on a page showing blog entries (using the Blog Entries Module).");
            Entries = 20;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SummaryModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        [Category("General"), Caption("Total Entries"), Description("The maximum number of blog entries shown in the summary display")]
        [UIHint("IntValue4"), Range(1, 9999), Required]
        public int Entries { get; set; }
    }
}
