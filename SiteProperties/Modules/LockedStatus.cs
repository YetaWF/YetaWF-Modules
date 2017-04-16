/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SiteProperties#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.SiteProperties.Modules {

    public class LockedStatusModuleDataProvider : ModuleDefinitionDataProvider<Guid, LockedStatusModule>, IInstallableModel { }

    [ModuleGuid("{d47cc2e3-fb2b-4e73-8088-1da894a36838}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class LockedStatusModule : ModuleDefinition {

        public LockedStatusModule() {
            Title = this.__ResStr("modTitle", "Site Locked Status");
            Name = this.__ResStr("modName", "Site Locked Status");
            Description = this.__ResStr("modSummary", "Displays the site's locked status");
            ShowTitle = false;
            WantFocus = false;
            WantSearch = false;
            Print = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new LockedStatusModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }
    }
}