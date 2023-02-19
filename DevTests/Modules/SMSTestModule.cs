/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.Endpoints.Support;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support.SendSMS;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class SMSTestModuleDataProvider : ModuleDefinitionDataProvider<Guid, SMSTestModule>, IInstallableModel { }

    [ModuleGuid("{f35f5f1c-d3f7-4b33-98a1-6f0700672258}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class SMSTestModule : ModuleDefinition2 {

        public SMSTestModule() {
            Title = this.__ResStr("modTitle", "SMS Test");
            Name = this.__ResStr("modName", "Test - SMS");
            Description = this.__ResStr("modSummary", "Test module to test sending an SMS message. A test page for this module can be found at Tests > Modules > SMS (standard YetaWF site).");
            DefaultViewName = StandardViews.EditApply;
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

        [Trim]
        [Header("SMS requires an SMS provider. These are not included with YetaWF and are available separately from Softel vdm, Inc. Check the YetaWF web site store for additional information. Please note that SMS providers don't issue an error when the SMS message is sent - Inspect the log file to find errors in SMS processing.")]
        public class Model {

            [Caption("Phone Number"), Description("The phone number which will receive the SMS message")]
            [TextBelow("The phone number is not validated - It is possible to specify an email address instead to test the email fallback if no SMS provider is available.")]
            [UIHint("Text20"), StringLength(30), Required, Trim]
            public string? PhoneNumber { get; set; } = null!;

            [Caption("Text"), Description("The text message to send")]
            [UIHint("TextAreaSourceOnly"), StringLength(SendSMS.MaxMessageLength), Required]
            public string? Text { get; set; } = null!;

            public Model() { }
        }

        public Task<ActionInfo> RenderModuleAsync() {
            Model model = new Model();
            return RenderAsync(model);
        }

        [ExcludeDemoMode]
        public async Task<IResult> UpdateModuleAsync(Model model) {
            if (!ModelState.IsValid)
                return await PartialViewAsync(model);
            SendSMS sendSMS = new SendSMS();
            await sendSMS.SendMessageAsync(model.PhoneNumber!, model.Text!);
            return await FormProcessedAsync(model, this.__ResStr("ok", "SMS sent"));
        }
    }
}
