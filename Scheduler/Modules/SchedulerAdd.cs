/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Scheduler.Modules {

    public class SchedulerAddModuleDataProvider : ModuleDefinitionDataProvider<Guid, SchedulerAddModule>, IInstallableModel { }

    [ModuleGuid("{F541B60F-4468-40ed-A59D-707463B1FBAA}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class SchedulerAddModule : ModuleDefinition {

        public SchedulerAddModule() : base() {
            Title = this.__ResStr("modTitle", "New Scheduler Item");
            Name = this.__ResStr("modName", "New Scheduler Item");
            Description = this.__ResStr("modSummary", "Creates a new scheduler item. Used by the Scheduler Module.");
            DefaultViewName = StandardViews.Add;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SchedulerAddModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction? GetAction_Add(string? url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Add",
                LinkText = this.__ResStr("addLink", "Add"),
                MenuText = this.__ResStr("addText", "Add"),
                Tooltip = this.__ResStr("addTooltip", "Create a new scheduler item"),
                Legend = this.__ResStr("addLegend", "Creates a new scheduler item"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                SaveReturnUrl = true,
            };
        }
    }
}