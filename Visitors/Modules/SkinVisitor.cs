/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Visitors#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Visitors.Modules {

    public class SkinVisitorModuleDataProvider : ModuleDefinitionDataProvider<Guid, SkinVisitorModule>, IInstallableModel { }

    [ModuleGuid("{7e432be4-1dbf-438a-9286-9c88ab225f5b}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class SkinVisitorModule : ModuleDefinition {

        public SkinVisitorModule() {
            Title = this.__ResStr("modTitle", "Skin Visitor Module");
            Name = this.__ResStr("modName", "Visitor (Skin) Module");
            Description = this.__ResStr("modSummary", "Records visitor activity");
            Invokable = true;
            InvokeInPopup = true;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SkinVisitorModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }
    }
}