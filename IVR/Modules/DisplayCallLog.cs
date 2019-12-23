/* Copyright �2020 Softel vdm, Inc.. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace Softelvdm.Modules.IVR.Modules {

    public class DisplayCallLogModuleDataProvider : ModuleDefinitionDataProvider<Guid, DisplayCallLogModule>, IInstallableModel { }

    [ModuleGuid("{92a027ed-704a-4dd3-bbc7-ff4185539e82}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class DisplayCallLogModule : ModuleDefinition {

        public DisplayCallLogModule() {
            Title = this.__ResStr("modTitle", "Call Log Entry");
            Name = this.__ResStr("modName", "Call Log Entry");
            Description = this.__ResStr("modSummary", "Displays an existing call log entry");
            DefaultViewName = StandardViews.Display;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new DisplayCallLogModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url, int id) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { Id = id },
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Display"),
                MenuText = this.__ResStr("displayText", "Display"),
                Tooltip = this.__ResStr("displayTooltip", "Display the call log entry"),
                Legend = this.__ResStr("displayLegend", "Displays an existing call log entry"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}
