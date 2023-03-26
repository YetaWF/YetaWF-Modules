/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support.SendSMS;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class SMSProcessorStatusModuleDataProvider : ModuleDefinitionDataProvider<Guid, SMSProcessorStatusModule>, IInstallableModel { }

    [ModuleGuid("{1d81a8a7-7b81-4560-bb4b-fe4b4459bd8d}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class SMSProcessorStatusModule : ModuleDefinition {

        public SMSProcessorStatusModule() {
            Title = this.__ResStr("modTitle", "SMS Processor Status");
            Name = this.__ResStr("modName", "SMS Processor Status");
            Description = this.__ResStr("modSummary", "Displays the status of the SMS processor (if any). A test page for this module can be found at Tests > Modules > SMS (standard YetaWF site).");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SMSProcessorStatusModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url) {
            return new ModuleAction() {
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

            };
        }

        public class DisplayModel {
            public int Available { get; set; }
            public string ProcessorName { get; set; } = null!;
            public bool TestMode { get; set; }
        }

        public async Task<ActionInfo> RenderModuleAsync() {
            if (!Manager.HasSuperUserRole)
                return ActionInfo.Empty;
            SendSMS.GetSMSProcessorCondInfo info = await SendSMS.GetSMSProcessorCondAsync();
            DisplayModel model = new DisplayModel() {
                Available = info.Count,
                TestMode = info.Processor != null ? await info.Processor.IsTestModeAsync() : false,
                ProcessorName = info.Processor?.Name ?? string.Empty,
            };
            if (model.Available == 1 && !model.TestMode)
                return ActionInfo.Empty;
            return await RenderAsync(model);
        }
    }
}
