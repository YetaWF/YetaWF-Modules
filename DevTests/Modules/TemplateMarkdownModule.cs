/* Copyright � 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateMarkdownModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateMarkdownModule>, IInstallableModel { }

    [ModuleGuid("{5873e206-5fc5-45ee-932e-1ca53251ccc5}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateMarkdownModule : ModuleDefinition {

        public TemplateMarkdownModule() {
            Title = this.__ResStr("modTitle", "Markdown Test Template");
            Name = this.__ResStr("modName", "Template Test - Markdown");
            Description = this.__ResStr("modSummary", "Markdown test template");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateMarkdownModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}
