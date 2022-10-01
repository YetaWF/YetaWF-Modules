/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.Scheduler.DataProvider;

namespace YetaWF.Modules.Scheduler.Modules {

    public class SchedulerConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, SchedulerConfigModule>, IInstallableModel { }

    [ModuleGuid("{A43ECFAE-7ED4-4d96-B5A8-CB5116E5A8DF}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class SchedulerConfigModule : ModuleDefinition {

        public SchedulerConfigModule() {
            Title = this.__ResStr("modTitle", "Scheduler Settings");
            Name = this.__ResStr("modName", "Scheduler Settings");
            Description = this.__ResStr("modSummary", "Used to edit a site's scheduler settings. This can be accessed using Admin > Settings > Scheduler Settings (standard YetaWF site).");
            ShowHelp = true;
            DefaultViewName = StandardViews.Config;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SchedulerConfigModuleDataProvider(); }
        public override DataProviderImpl GetConfigDataProvider() { return new SchedulerConfigDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Config",
                LinkText = this.__ResStr("editLink", "Scheduler Settings"),
                MenuText = this.__ResStr("editText", "Scheduler Settings"),
                Tooltip = this.__ResStr("editTooltip", "Edit the scheduler settings"),
                Legend = this.__ResStr("editLegend", "Edits the scheduler settings"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}