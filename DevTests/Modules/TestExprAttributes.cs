/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TestExprAttributesModuleDataProvider : ModuleDefinitionDataProvider<Guid, TestExprAttributesModule>, IInstallableModel { }

    [ModuleGuid("{8B4EAD54-BC75-459f-8388-4056CB1234D3}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TestExprAttributesModule : ModuleDefinition {

        public TestExprAttributesModule() {
            Title = this.__ResStr("modTitle", "Test ExprAttributes");
            Name = this.__ResStr("modName", "Test ExprAttributes");
            Description = this.__ResStr("modSummary", "Test module for various property attributes. This is used for internal testing only.");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TestExprAttributesModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    }
}
