/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Scheduler;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Scheduler.DataProvider;

namespace YetaWF.Modules.Scheduler.Modules;

public class SchedulerDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, SchedulerDisplayModule>, IInstallableModel { }

[ModuleGuid("{A18C31AE-730A-4221-A03B-3152FC0607CF}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class SchedulerDisplayModule : ModuleDefinition {

    public SchedulerDisplayModule() : base() {
        Title = this.__ResStr("modTitle", "Scheduler Item");
        Name = this.__ResStr("modName", "Display Scheduler Item");
        Description = this.__ResStr("modSummary", "Displays details for an existing scheduler item. Used by the Scheduler Module.");
        DefaultViewName = StandardViews.Display;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new SchedulerDisplayModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction? GetAction_Display(string? url, string name) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { EventName = name },
            Image = "#Display",
            LinkText = this.__ResStr("displayLink", "Display"),
            MenuText = this.__ResStr("displayText", "Display"),
            Tooltip = this.__ResStr("displayTooltip", "Display an existing scheduler item"),
            Legend = this.__ResStr("displayLegend", "Displays an existing scheduler item"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,

        };
    }

    [Trim]
    public class SchedulerDisplayModel {

        [Caption("Name"), Description("The name of this scheduler item - the name is used to identify a scheduler item")]
        [UIHint("String"), StringLength(SchedulerItemData.MaxName), ReadOnly]
        public string Name { get; set; } = null!;

        [Caption("Description"), Description("The description of this scheduler item")]
        [UIHint("TextArea"), ReadOnly]
        public string? Description { get; set; }

        [Caption("Enabled"), Description("Defines whether the scheduler item is enabled")]
        [UIHint("Boolean"), ReadOnly]
        public bool Enabled { get; set; }

        [Caption("Enable On Startup"), Description("Defines whether the scheduler item is enabled every time the website is restarted")]
        [UIHint("Boolean"), ReadOnly]
        public bool EnableOnStartup { get; set; }

        [Caption("Run Once"), Description("Defines whether the scheduler item is run just once - once it completes, it is disabled")]
        [UIHint("Boolean"), ReadOnly]
        public bool RunOnce { get; set; }

        [Caption("Startup"), Description("Defines whether the scheduler item runs at website startup")]
        [UIHint("Boolean"), ReadOnly]
        public bool Startup { get; set; }

        [Caption("Site Specific"), Description("Defines whether the scheduler item runs for each site")]
        [UIHint("Boolean"), ReadOnly]
        public bool SiteSpecific { get; set; }

        [Caption("Interval"), Description("The scheduler item's frequency")]
        [UIHint("YetaWF_Scheduler_Frequency"), ReadOnly]
        public SchedulerFrequency Frequency { get; set; } = null!;

        [Caption("Last"), Description("The last time this item ran")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime Last { get; set; }

        [Caption("Next"), Description("The time this item is scheduled to run next")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime Next { get; set; }

        [Caption("RunTime"), Description("The duration of the last occurrence of this scheduler item (hh:mm:ss)")]
        [UIHint("TimeSpan"), ReadOnly]
        public TimeSpan RunTime { get; set; }

        [Caption("Errors"), Description("The errors that occurred during the scheduler item's last run")]
        [UIHint("TextArea"), ReadOnly]
        public string? Errors { get; set; }

        [Caption("Event"), Description("The event running at the scheduled time")]
        [UIHint("YetaWF_Scheduler_Event"), ReadOnly]
        public SchedulerEvent Event { get; set; } = null!;

        public void SetEvent(SchedulerItemData evnt) {
            ObjectSupport.CopyData(evnt, this);
        }
    }

    public async Task<ActionInfo> RenderModuleAsync(string eventName) {
        using (SchedulerDataProvider dataProvider = new SchedulerDataProvider()) {
            SchedulerDisplayModel model = new SchedulerDisplayModel { };
            SchedulerItemData? evnt = await dataProvider.GetItemAsync(eventName);
            if (evnt == null)
                throw new Error(this.__ResStr("notFound", "Scheduler item \"{0}\" not found."), eventName);
            model.SetEvent(evnt);
            Title = this.__ResStr("title", "Scheduler Item \"{0}\"", eventName);
            return await RenderAsync(model);
        }
    }
}