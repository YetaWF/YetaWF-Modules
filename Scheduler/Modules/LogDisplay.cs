/* Copyright �2020 Softel vdm, Inc.. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Scheduler.Modules {

    public class LogDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, LogDisplayModule>, IInstallableModel { }

    [ModuleGuid("{c279240c-bbe6-49e6-9dcf-5681754d8ff5}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class LogDisplayModule : ModuleDefinition {

        public LogDisplayModule() {
            Title = this.__ResStr("modTitle", "Scheduler Log Entry");
            Name = this.__ResStr("modName", "Scheduler Log Entry");
            Description = this.__ResStr("modSummary", "Displays an existing scheduler log entry. Used by the Scheduler Log Module.");
            DefaultViewName = StandardViews.Display;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new LogDisplayModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url, int logEntry) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { LogEntry = logEntry },
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Display"),
                MenuText = this.__ResStr("displayText", "Display"),
                Tooltip = this.__ResStr("displayTooltip", "Display the scheduler log entry"),
                Legend = this.__ResStr("displayLegend", "Displays an existing scheduler log entry"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}
