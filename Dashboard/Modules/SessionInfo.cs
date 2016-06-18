/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Dashboard.Modules {

    public class SessionInfoModuleDataProvider : ModuleDefinitionDataProvider<Guid, SessionInfoModule>, IInstallableModel { }

    [ModuleGuid("{FDC457A6-EAF7-4874-949F-67AB6DDD5343}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class SessionInfoModule : ModuleDefinition {

        public SessionInfoModule() {
            Title = this.__ResStr("modTitle", "SessionState Information (HttpContext.Current.Session)");
            Name = this.__ResStr("modName", "SessionState Information (HttpContext.Current.Session)");
            Description = this.__ResStr("modSummary", "Displays SessionState information (HttpContext.Current.Session)");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SessionInfoModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "HttpContext.Current.Session"),
                MenuText = this.__ResStr("displayText", "HttpContext.Current.Session"),
                Tooltip = this.__ResStr("displayTooltip", "Display SessionState information (HttpContext.Current.Session)"),
                Legend = this.__ResStr("displayLegend", "Displays SessionState information (HttpContext.Current.Session)"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}