/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/AddThis#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.AddThis.Modules {

    public class SharingSidebarModuleDataProvider : ModuleDefinitionDataProvider<Guid, SharingSidebarModule>, IInstallableModel { }

    [ModuleGuid("{d790d324-ec41-419d-abba-fdb03652cd9d}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class SharingSidebarModule : ModuleDefinition {

        public SharingSidebarModule() {
            Title = this.__ResStr("modTitle", "Sharing Sidebar");
            Name = this.__ResStr("modName", "Sharing Sidebar (Skin)");
            Description = this.__ResStr("modSummary", "Displays an AddThis Sharing Sidebar");
            ShowTitle = false;
            WantSearch = false;
            WantFocus = false;
            Print = false;
            Invokable = true;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SharingSidebarModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }
    }
}