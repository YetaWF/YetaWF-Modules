/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Dashboard.Modules {

    public class HttpModulesBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, HttpModulesBrowseModule>, IInstallableModel { }

    [ModuleGuid("{e3adcdfa-24ed-423e-9d7c-01541b1bca70}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class HttpModulesBrowseModule : ModuleDefinition {

        public HttpModulesBrowseModule() {
            Title = this.__ResStr("modTitle", "Http Modules");
            Name = this.__ResStr("modName", "Http Modules");
            Description = this.__ResStr("modSummary", "Displays all Http Modules (Pipeline)");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new HttpModulesBrowseModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Items(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Browse",
                LinkText = this.__ResStr("browseLink", "Http Modules"),
                MenuText = this.__ResStr("browseText", "Http Modules"),
                Tooltip = this.__ResStr("browseTooltip", "Display all Http Modules (Pipeline)"),
                Legend = this.__ResStr("browseLegend", "Displays all Http Modules (Pipeline)"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
    }
}