/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.ImageRepository.Modules {

    public class TemplateTestModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateTestModule>, IInstallableModel { }

    [ModuleGuid("{66fb78ed-a185-4251-8115-d783b5554b37}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateTestModule : ModuleDefinition {

        public TemplateTestModule() {
            Title = this.__ResStr("modTitle", "Image Repository Template Test");
            Name = this.__ResStr("modName", "Image Repository Template Test");
            Description = this.__ResStr("modSummary", "Image Repository Template Test");
            WantSearch = false;
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateTestModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}