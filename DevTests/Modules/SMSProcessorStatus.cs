/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class SMSProcessorStatusModuleDataProvider : ModuleDefinitionDataProvider<Guid, SMSProcessorStatusModule>, IInstallableModel { }

    [ModuleGuid("{1d81a8a7-7b81-4560-bb4b-fe4b4459bd8d}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class SMSProcessorStatusModule : ModuleDefinition {

        public SMSProcessorStatusModule() {
            Title = this.__ResStr("modTitle", "SMS Processor Status");
            Name = this.__ResStr("modName", "SMS Processor Status");
            Description = this.__ResStr("modSummary", "Displays the status of the SMS processor");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SMSProcessorStatusModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Display"),
                MenuText = this.__ResStr("displayText", "Display"),
                Tooltip = this.__ResStr("displayTooltip", "Display the status of the SMS processor"),
                Legend = this.__ResStr("displayLegend", "Displays the status of the SMS processor"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}
