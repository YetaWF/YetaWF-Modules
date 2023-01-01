/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/AddThis#License */

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
            Description = this.__ResStr("modSummary", "Displays an AddThis Sharing Sidebar and is referenced by your page(s) or site. It is not added to a skin or page, instead it is referenced by a site, page or module. Referencing the Sharing Sidebar (Skin) Module causes the AddThis Sharing Sidebar to be displayed as configured by the settings defined using Admin > Settings > AddThis Sharing SideBar Settings (standard YetaWF site).");
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