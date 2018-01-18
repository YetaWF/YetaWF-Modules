/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Sites#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Sites.Modules {

    public class SiteSelectorModuleDataProvider : ModuleDefinitionDataProvider<Guid, SiteSelectorModule>, IInstallableModel { }

    [ModuleGuid("{36a82326-481d-4451-960b-35d2fcf87c94}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class SiteSelectorModule : ModuleDefinition {

        public SiteSelectorModule() {
            Title = this.__ResStr("modTitle", "Site Selector");
            Name = this.__ResStr("modName", "Site Selector");
            Description = this.__ResStr("modSummary", "Site selector (used during development)");
            ShowTitle = false;
            WantFocus = false;
            WantSearch = false;
            Print = false;
            DefaultViewName = StandardViews.PropertyListEdit;
            UsePartialFormCss = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SiteSelectorModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles {
            get {
#if DEBUG
                return AnonymousLevel_DefaultAllowedRoles;
#else
                return SuperuserLevel_DefaultAllowedRoles;
#endif
            }
        }
    }
}