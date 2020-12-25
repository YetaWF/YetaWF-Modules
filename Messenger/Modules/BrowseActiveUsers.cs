/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Messenger.Modules {

    public class BrowseActiveUsersModuleDataProvider : ModuleDefinitionDataProvider<Guid, BrowseActiveUsersModule>, IInstallableModel { }

    [ModuleGuid("{A48F67F7-AF4A-47cf-AE9F-1859E5FB722C}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class BrowseActiveUsersModule : ModuleDefinition {

        public BrowseActiveUsersModule() {
            Title = this.__ResStr("modTitle", "Active Users");
            Name = this.__ResStr("modName", "Browse Active Users");
            Description = this.__ResStr("modSummary", "Displays currently active users. It is accessible using Admin > Dashboard > Active Users (standard YetaWF site).");
            DefaultViewName = StandardViews.Browse;
            UsePartialFormCss = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new BrowseActiveUsersModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}