/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateDropDownModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateDropDownModule>, IInstallableModel { }

    [ModuleGuid("{2274EE2B-D9EE-46b1-AFDC-6BF3713A60CA}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateDropDownModule : ModuleDefinition {

        public TemplateDropDownModule() {
            Title = this.__ResStr("modTitle", "DropDown Test Component");
            Name = this.__ResStr("modName", "Component Test - DropDown");
            Description = this.__ResStr("modSummary", "Test module for the DropDown component. A test page for this module can be found at Tests > Templates > DropDown (standard YetaWF site).");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateDropDownModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}
