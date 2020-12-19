/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Dashboard.Modules {

    public class StatusModuleDataProvider : ModuleDefinitionDataProvider<Guid, StatusModule>, IInstallableModel { }

    [ModuleGuid("{c93657d3-522c-483a-a51a-e20d39d95b6a}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class StatusModule : ModuleDefinition {

        public StatusModule() {
            Title = this.__ResStr("modTitle", "Status Information");
            Name = this.__ResStr("modName", "Status Information");
            Description = this.__ResStr("modSummary", "Displays information about the YetaWF instance. Status information can be accessed using Admin > Dashboard > Status Information (standard YetaWF site).");
            DefaultViewName = StandardViews.Display;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new StatusModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Status Information"),
                MenuText = this.__ResStr("displayText", "Status Information"),
                Tooltip = this.__ResStr("displayTooltip", "Display YetaWF status information"),
                Legend = this.__ResStr("displayLegend", "Displays YetaWF status information"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}
