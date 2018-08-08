/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateColorPickerModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateColorPickerModule>, IInstallableModel { }

    [ModuleGuid("{a9a13167-f308-4749-8130-3e3b3d28cc70}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateColorPickerModule : ModuleDefinition {

        public TemplateColorPickerModule() {
            Title = this.__ResStr("modTitle", "ColorPicker Test Template");
            Name = this.__ResStr("modName", "Template Test - ColorPicker");
            Description = this.__ResStr("modSummary", "ColorPicker test template");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateColorPickerModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}
