/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Visitors.Modules {

    public class SkinVisitorModuleDataProvider : ModuleDefinitionDataProvider<Guid, SkinVisitorModule>, IInstallableModel { }

    [ModuleGuid("{7e432be4-1dbf-438a-9286-9c88ab225f5b}"), PublishedModuleGuid]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class SkinVisitorModule : ModuleDefinition {

        public SkinVisitorModule() {
            Title = this.__ResStr("modTitle", "Skin Visitor Module");
            Name = this.__ResStr("modName", "Visitor (Skin) Module");
            Description = this.__ResStr("modSummary", "Records site visitor information. This module is typically added to every page (as a skin module) so all visitor activity is recorded. This is best done using a Module Reference in the Site Settings.");
            Invokable = true;
            InvokeInPopup = true;
            WantFocus = false;
            WantSearch = false;
            Print = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SkinVisitorModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }
    }
}