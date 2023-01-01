/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Scheduler.Modules {

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
            return new ModuleAction(this) {
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
                SaveReturnUrl = true,
            };
        }
    }
}