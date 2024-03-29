/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Basics.Modules {

    public class AlertDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, AlertDisplayModule>, IInstallableModel { }

    [ModuleGuid("{24b7dc07-e96a-409d-911f-47bffd38d0fc}"), PublishedModuleGuid]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class AlertDisplayModule : ModuleDefinition {

        public AlertDisplayModule() {
            Title = this.__ResStr("modTitle", "Alert Message (Skin)");
            Name = this.__ResStr("modName", "Alert Message (Skin)");
            Description = this.__ResStr("modSummary", "Can be referenced by sites, pages or modules. Once referenced, an Alert will be displayed (once or until dismissed, depending on Alert Settings).");
            WantFocus = false;
            WantSearch = false;
            ShowTitle = false;
            Print = false;
            Invokable = true;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new AlertDisplayModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }
    }
}