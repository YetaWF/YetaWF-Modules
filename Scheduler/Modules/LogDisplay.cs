/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Scheduler.DataProvider;

namespace YetaWF.Modules.Scheduler.Modules;

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

    public ModuleAction? GetAction_Display(string? url, int logEntry) {
        return new ModuleAction() {
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

        };
    }

    public class DisplayModel {

        [Caption("Created"), Description("The date/time this log record was created")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime TimeStamp { get; set; }

        [Caption("Id"), Description("The id of the scheduler item run")]
        [UIHint("LongValue"), ReadOnly]
        public long RunId { get; set; }

        [Caption("Name"), Description("The name of the running scheduler item")]
        [UIHint("String"), ReadOnly]
        public string Name { get; set; } = null!;

        [Caption("Level"), Description("The message level")]
        [UIHint("Enum"), ReadOnly]
        public Core.Log.Logging.LevelEnum Level { get; set; }

        [Caption("Site Id"), Description("The site which was affected by the scheduler item")]
        [UIHint("SiteId"), ReadOnly]
        public int SiteIdentity { get; set; }

        [Caption("Message"), Description("The message")]
        [UIHint("String"), ReadOnly]
        public string? Info { get; set; }

        public void SetData(LogData data) {
            ObjectSupport.CopyData(data, this);
        }
    }

    public async Task<ActionInfo> RenderModuleAsync(int logEntry) {
        using (LogDataProvider logDP = new LogDataProvider()) {
            LogData? data = await logDP.GetItemAsync(logEntry);
            if (data == null)
                throw new Error(this.__ResStr("notFound", "Scheduler log entry with id {0} not found"), logEntry);
            DisplayModel model = new DisplayModel();
            model.SetData(data);
            return await RenderAsync(model);
        }
    }
}
