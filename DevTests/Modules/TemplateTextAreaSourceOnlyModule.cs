/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateTextAreaSourceOnlyModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateTextAreaSourceOnlyModule>, IInstallableModel { }

    [ModuleGuid("{00f87a82-ca91-4436-8353-2cc09cb2b89c}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateTextAreaSourceOnlyModule : ModuleDefinition {

        public TemplateTextAreaSourceOnlyModule() {
            Title = this.__ResStr("modTitle", "TextAreaSourceOnly Test Template");
            Name = this.__ResStr("modName", "Template Test - TextAreaSourceOnly");
            Description = this.__ResStr("modSummary", "TextAreaSourceOnly test template");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateTextAreaSourceOnlyModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}
