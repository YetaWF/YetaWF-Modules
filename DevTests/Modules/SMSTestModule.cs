/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class SMSTestModuleDataProvider : ModuleDefinitionDataProvider<Guid, SMSTestModule>, IInstallableModel { }

    [ModuleGuid("{f35f5f1c-d3f7-4b33-98a1-6f0700672258}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class SMSTestModule : ModuleDefinition {

        public SMSTestModule() {
            Title = this.__ResStr("modTitle", "SMS Test");
            Name = this.__ResStr("modName", "Test - SMS");
            Description = this.__ResStr("modSummary", "SMS test");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SMSTestModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "SMS"),
                MenuText = this.__ResStr("displayText", "SMS"),
                Tooltip = this.__ResStr("displayTooltip", "Display the SMS test"),
                Legend = this.__ResStr("displayLegend", "Displays the SMS test"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}