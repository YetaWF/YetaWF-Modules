/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using Softelvdm.Modules.IVR.DataProvider;
using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace Softelvdm.Modules.IVR.Modules {

    public class DisplayVoiceMailModuleDataProvider : ModuleDefinitionDataProvider<Guid, DisplayVoiceMailModule>, IInstallableModel { }

    [ModuleGuid("{e8ec7d7d-6c13-4821-aaad-bbc162879ac5}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class DisplayVoiceMailModule : ModuleDefinition2 {

        public DisplayVoiceMailModule() {
            Title = this.__ResStr("modTitle", "Voice Mail Entry");
            Name = this.__ResStr("modName", "Voice Mail Entry");
            Description = this.__ResStr("modSummary", "Displays an existing voice mail entry.");
            DefaultViewName = StandardViews.Display;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new DisplayVoiceMailModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public async Task<ModuleAction?> GetAction_DisplayAsync(string? url, int id) {
            if (url == null) {
                IVRConfig config = await IVRConfigDataProvider.GetConfigAsync();
                url = config.DisplayVoiceMailUrl;
            }
            return new ModuleAction() {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { Id = id },
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Display"),
                MenuText = this.__ResStr("displayText", "Display"),
                Tooltip = this.__ResStr("displayTooltip", "Display the voice mail entry"),
                Legend = this.__ResStr("displayLegend", "Displays an existing voice mail entry"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
        public async Task<ModuleAction> GetAction_ListenAsync(string recordingUrl) {
            return new ModuleAction() {
                Url = recordingUrl,
                Image = await CustomIconAsync("Listen.png"),
                Style = ModuleAction.ActionStyleEnum.NewWindow,
                LinkText = this.__ResStr("listenLink", "Listen"),
                MenuText = this.__ResStr("listenMenu", "Listen"),
                Tooltip = this.__ResStr("listenTT", "Listen to the voice mail"),
                Legend = this.__ResStr("listenLegend", "Used to listen to the voice mail"),
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
            };
        }

        public class DisplayModel {

            [Caption("Listen"), Description("")]
            [UIHint("ModuleAction"), ReadOnly]
            public ModuleAction Listen { get; set; } = null!;

            [Caption("Created"), Description("The date/time the voice mail message was created")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Created { get; set; }

            [Caption("Heard"), Description("Defines whether the voice mail was already listened to")]
            [UIHint("Boolean"), ReadOnly]
            public bool Heard { get; set; }

            [Caption("From"), Description("The caller's phone number")]
            [UIHint("PhoneNumber"), ReadOnly]
            [ExcludeDemoMode]
            public string Caller { get; set; } = null!;

            [Caption("Duration"), Description("The duration of the voice mail message (in seconds)")]
            [UIHint("IntValue"), ReadOnly]
            public int Duration { get; set; }

            [Caption("From City"), Description("The caller's city (if available)")]
            [UIHint("String"), ReadOnly]
            public string? CallerCity { get; set; }
            [Caption("From State"), Description("The caller's state (if available)")]
            [UIHint("String"), ReadOnly]
            public string? CallerState { get; set; }
            [Caption("From Zip Code"), Description("The caller's ZIP code (if available)")]
            [UIHint("String"), ReadOnly]
            public string? CallerZip { get; set; }
            [Caption("From Country"), Description("The caller's country (if available)")]
            [UIHint("String"), ReadOnly]
            public string? CallerCountry { get; set; }

            [Caption("Phone Number"), Description("The phone number for which the voice mail message is saved")]
            [UIHint("PhoneNumber"), ReadOnly]
            public string? To { get; set; }
            [Caption("Extension"), Description("The extension for which the voice mail message is saved")]
            [UIHint("String"), ReadOnly]
            public string? Extension { get; set; }

            [Caption("Call Sid"), Description("The id used by Twilio to identify the call")]
            [UIHint("String"), ReadOnly]
            public string? CallSid { get; set; }

            public string? RecordingUrl { get; set; }

            public void SetData(VoiceMailData data) {
                ObjectSupport.CopyData(data, this);
            }
        }

        public async Task<ActionInfo> RenderModuleAsync() {
            if (!int.TryParse(Manager.RequestQueryString["Id"], out int id)) throw new InternalError($"Argument {nameof(id)} missing");
            using (VoiceMailDataProvider voiceMailDP = new VoiceMailDataProvider()) {
                VoiceMailData? voiceMail = await voiceMailDP.GetItemByIdentityAsync(id);
                if (voiceMail == null)
                    throw new Error(this.__ResStr("notFound", "Voice mail entry with id {0} not found"), id);
                voiceMail.Heard = true;
                await voiceMailDP.UpdateItemAsync(voiceMail);
                DisplayModel model = new DisplayModel() {
                    Listen = await GetAction_ListenAsync(voiceMail.RecordingUrl)
                };
                model.SetData(voiceMail);
                return await RenderAsync(model);
            }
        }
    }
}
