/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Scheduler.Modules {

    public class SchedulerEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, SchedulerEditModule>, IInstallableModel { }

    [ModuleGuid("{09EB1AB0-2FA1-4d41-A853-91778FC86355}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class SchedulerEditModule : ModuleDefinition {

        public SchedulerEditModule() : base() {
            Title = this.__ResStr("modTitle", "Scheduler Item");
            Name = this.__ResStr("modName", "Edit Scheduler Item");
            Description = this.__ResStr("modSummary", "Edits an existing scheduler item");
            DefaultViewName = StandardViews.Edit;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SchedulerEditModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url, string name) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { EventName = name },
                Image = "#Edit",
                LinkText = this.__ResStr("editLink", "Edit"),
                MenuText = this.__ResStr("editText", "Edit"),
                Tooltip = this.__ResStr("editTooltip", "Edit an existing scheduler item"),
                Legend = this.__ResStr("editLegend", "Edits an existing scheduler item"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}