/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Blog.Modules {

    public class DisqusModuleDataProvider : ModuleDefinitionDataProvider<Guid, DisqusModule>, IInstallableModel { }

    [ModuleGuid("{3ba64dfb-9292-4f9b-937e-0c8fe110bf45}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class DisqusModule : ModuleDefinition {

        public DisqusModule() {
            Title = this.__ResStr("modTitle", "Comments");
            Name = this.__ResStr("modName", "Disqus Comments");
            Description = this.__ResStr("modSummary", "Comments using the Disqus service");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new DisqusModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    }
}