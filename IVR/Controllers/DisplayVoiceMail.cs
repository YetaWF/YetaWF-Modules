/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using Softelvdm.Modules.IVR.DataProvider;
using System;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace Softelvdm.Modules.IVR.Controllers {

    public class DisplayVoiceMailModuleController : ControllerImpl<Softelvdm.Modules.IVR.Modules.DisplayVoiceMailModule> {

        public DisplayVoiceMailModuleController() { }

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

        [AllowGet]
        public async Task<ActionResult> DisplayVoiceMail(int id) {
            using (VoiceMailDataProvider voiceMailDP = new VoiceMailDataProvider()) {
                VoiceMailData? voiceMail = await voiceMailDP.GetItemByIdentityAsync(id);
                if (voiceMail == null)
                    throw new Error(this.__ResStr("notFound", "Voice mail entry with id {0} not found"), id);
                voiceMail.Heard = true;
                await voiceMailDP.UpdateItemAsync(voiceMail);
                DisplayModel model = new DisplayModel() {
                    Listen = await Module.GetAction_ListenAsync(voiceMail.RecordingUrl)
                };
                model.SetData(voiceMail);
                return View(model);
            }
        }
    }
}
