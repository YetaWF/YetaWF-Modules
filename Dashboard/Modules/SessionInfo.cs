/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

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
            Description = this.__ResStr("modSummary", "Displays SessionState information (HttpContext.Current.Session). Session state information can be accessed using Admin > Dashboard > HttpContext.Current.Session (standard YetaWF site).");
            DefaultViewName = StandardViews.Display;
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
        public ModuleAction GetAction_ClearAll() {
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(YetaWF.Modules.Dashboard.Controllers.SessionInfoModuleController), "ClearAll"),
                QueryArgs = new { __ModuleGuid = ModuleGuid },
                Image = "#Remove",
                LinkText = this.__ResStr("removeLink", "Remove Session Settings"),
                MenuText = this.__ResStr("removeText", "Remove Session Settings"),
                Tooltip = this.__ResStr("removeTooltip", "Remove all SessionState information"),
                Legend = this.__ResStr("removeLegend", "Removes all SessionState information"),
                Style = ModuleAction.ActionStyleEnum.Post,
                Category = ModuleAction.ActionCategoryEnum.Delete,
                Mode = ModuleAction.ActionModeEnum.Any,
                ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove all session settings for all users?"),
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.InPopup,
            };
        }
    }
}