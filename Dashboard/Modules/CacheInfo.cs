/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Dashboard.Modules {

    public class CacheInfoModuleDataProvider : ModuleDefinitionDataProvider<Guid, CacheInfoModule>, IInstallableModel { }

    [ModuleGuid("{4dbd47e4-783e-4af9-bf3d-fb98a0d16574}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class CacheInfoModule : ModuleDefinition {

        public CacheInfoModule() {
            Title = this.__ResStr("modTitle", "Cache Information (HttpContext.Current.Cache)");
            Name = this.__ResStr("modName", "Cache Information (HttpContext.Current.Cache)");
            Description = this.__ResStr("modSummary", "Displays cache information (HttpContext.Current.Cache). Cache information can be accessed using Admin > Dashboard > HttpContext.Current.Cache (standard YetaWF site).");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new CacheInfoModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "HttpContext.Current.Cache"),
                MenuText = this.__ResStr("displayText", "HttpContext.Current.Cache"),
                Tooltip = this.__ResStr("displayTooltip", "Display cache information (HttpContext.Current.Cache)"),
                Legend = this.__ResStr("displayLegend", "Displays cache information (HttpContext.Current.Cache)"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}
