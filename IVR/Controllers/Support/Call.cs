/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using Microsoft.AspNetCore.Http;
using Softelvdm.Modules.IVR.DataProvider;
using Softelvdm.Modules.IVR.Modules;
using Softelvdm.Modules.TwilioProcessorDataProvider.DataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Extensions;
using YetaWF.Core.Localize;
using YetaWF.Core.Log;
using YetaWF.Core.Modules;
using YetaWF.Core.Security;
using YetaWF.Core.Support;
using YetaWF.Core.Support.SendSMS;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif
#if !DEBUG
using Softelvdm.Modules.TwilioProcessor.Controllers.Support;
#endif

namespace Softelvdm.Modules.IVR.Controllers {

    public class CallController : YetaWFController {

        public const string SECTION_MAIN = "Main";
        public const string SECTION_MAINHOLIDAY = "MainHoliday";
        public const string SECTION_MAINCLOSED = "MainClosed";
        public const string SECTION_MAINGOODBYE = "MainGoodbye";
        public const string SECTION_GATHEREXTENSION = "GatherExtension";
        public const string SECTION_ENTEREDEXTENSION = "EnteredExtension";

        public CallController() { }

        [AllowPost]
        public async Task<ActionResult> Process(string request, string extension, int errCount, string token) {

            LogCall(request, extension);

            TwilioData twilioConfig = await TwilioConfigDataProvider.GetConfigCondAsync();
            if (twilioConfig == null || !twilioConfig.IsConfigured())
                return RejectResult("Twilio is not configured");
            string authToken = twilioConfig.TestMode ? twilioConfig.TestAuthToken : twilioConfig.LiveAuthToken;
            IVRConfig ivrConfig = await IVRConfigDataProvider.GetConfigCondAsync();
            if (ivrConfig == null || string.IsNullOrWhiteSpace(ivrConfig.PublicKey) || string.IsNullOrWhiteSpace(ivrConfig.PrivateKey))
                return RejectResult("Config settings not available");

#if !DEBUG
            // There is something very peculiar about twilio verification. The initial request will validate correctly, but anything after that will not.
            // Even if the only difference is CallStatus (tested with a redirect to Request=Main. So I gave up and validate just the first one
            // and add my own token validation (as argument)
            if (string.IsNullOrWhiteSpace(token)) {
                if (!Verify.VerifyTwilio(authToken, twilioConfig.TestMode ? ivrConfig.TestVerificationProcessCallUrl : ivrConfig.LiveVerificationProcessCallUrl))
                    return RejectResult("Twilio verification failed");
            } else {
                // verify token. If it wasn't generated within the last 5 minutes, reject it.
                string decryptedToken;
                RSACrypto.Decrypt(ivrConfig.PrivateKey, token, out decryptedToken);
                DateTime tokenTime = new DateTime(Convert.ToInt64(decryptedToken));
                if (tokenTime < DateTime.UtcNow.AddMinutes(-5))
                    return RejectResult("Token verification failed");
            }
#endif
            if (string.IsNullOrWhiteSpace(request)) {

                // call log

                using (CallLogDataProvider callLogDP = new CallLogDataProvider()) {
                    await callLogDP.AddItemAsync(new CallLogEntry {
                        Caller = GetForm("From")?.Truncate(Globals.MaxPhoneNumber),
                        CallerCity = GetForm("CallerCity")?.Truncate(CallLogEntry.MaxCity),
                        CallerCountry = GetForm("CallerCountry")?.Truncate(CallLogEntry.MaxCountry),
                        CallerState = GetForm("CallerState")?.Truncate(CallLogEntry.MaxState),
                        CallerZip = GetForm("CallerZip")?.Truncate(CallLogEntry.MaxZip),
                        To = GetForm("Called")?.Truncate(Globals.MaxPhoneNumber),
                    });
                }

                // check for blocked numbers
                using (BlockedNumberDataProvider blockedDP = new BlockedNumberDataProvider()) {
                    BlockedNumberEntry blockedEntry = await blockedDP.GetItemAsync(GetForm("From"));
                    if (blockedEntry != null)
                        return RejectResult($"Blocked number {GetForm("From")}");
                }

                // notify (new call)
                foreach (ExtensionPhoneNumber notifyNumber in ivrConfig.NotificationNumbers) {
                    if (notifyNumber.SendSMS) {
                        SendSMS sendSMS = new SendSMS();
                        await sendSMS.SendMessageAsync(notifyNumber.PhoneNumber,
                            this.__ResStr("notifySMS", "Incoming call received from {0} ({1}, {2}, {3}, {4}) - {5}",
                            GetForm("Caller"), GetForm("CallerCity"), GetForm("CallerState"), GetForm("CallerZip"), GetForm("CallerCountry"), GetForm("To")),
                            ThrowError: false);
                    }
                }

                // determine main action to run

                request = SECTION_MAIN;
                using (HolidayEntryDataProvider holidayDP = new HolidayEntryDataProvider()) {
                    HolidayEntry holiday = await holidayDP.GetItemAsync(DateTime.Now.Date.ToUniversalTime());
                    if (holiday != null) {
                        request = SECTION_MAINHOLIDAY;
                    } else if (ivrConfig.OpeningHours.IsClosed(DateTime.UtcNow)) {
                        request = SECTION_MAINCLOSED;
                    }
                }
            }

            string called;
            if (!TryGetForm("CalledVia", out called))
                called = GetForm("Called");

            if (ivrConfig.MaxErrors != 0 && errCount >= ivrConfig.MaxErrors)
                request = SECTION_MAINGOODBYE;

            request = request.ToLower();

            using (ScriptDataProvider scriptDP = new ScriptDataProvider()) {
                ScriptData script = await scriptDP.GetScriptAsync(called);
                if (script == null)
                    return RejectResult($"Script not found for {called}");

                // See if a valid extension was entered
                if (request == SECTION_GATHEREXTENSION.ToLower()) {
                    string digits;
                    if (TryGetForm("Digits", out digits)) {
                        Extension ext = script.FindExtension(digits);
                        if (ext != null) {
                            extension = ext.Digits;
                            request = SECTION_ENTEREDEXTENSION.ToLower(); // a valid extension was entered, run EnteredExtension instead
                        }
                    }
                }

                // find the entry that matches the name and parameters
                List<ScriptEntry> entries = (from e in script.Scripts where e.Tag == request select e).ToList();
                foreach (ScriptEntry entry in entries) {
                    if (entry.Parms.Count > 0) {
                        // check parms
                        bool valid = true;
                        foreach (ScriptParm parm in entry.Parms) {
                            if (GetForm(parm.Name) != parm.Value) {
                                valid = false;
                                break;
                            }
                        }
                        if (!valid)
                            continue;
                    }
                    return await RunEntryAsync(ivrConfig, script, called, extension, entry, errCount);
                }
                throw new InternalError($"Nothing to execute - tag \"{request}\" for {called}");
            }
        }
        private ContentResult RejectResult(string reason) {
            string xmlString = @"<Response><Reject reason=""busy"" /></Response>";
            Logging.AddErrorLog($"{nameof(RejectResult)}: {reason} {xmlString}");
            return this.Content(xmlString, "text/xml");
        }

        private async Task<ActionResult> RunEntryAsync(IVRConfig ivrConfig, ScriptData script, string called, string extension, ScriptEntry entry, int errCount) {

            string extensionName = null;
            string extensionSpaced = null;
            if (!string.IsNullOrWhiteSpace(extension)) {
                extensionSpaced = Spaced(extension);
                Extension e = script.FindExtension(extension);
                if (e != null)
                    extensionName = e.Name;
            }
            string digits;
            TryGetForm("Digits", out digits);

            string actionUrl = Utility.UrlFor(typeof(CallController), nameof(Process));
#if DEBUG
            actionUrl = Manager.CurrentSite.MakeFullUrl(actionUrl, SecurityType: YetaWF.Core.Pages.PageDefinition.PageSecurityType.Any);
#else
            actionUrl = Manager.CurrentSite.MakeFullUrl(actionUrl, SecurityType: YetaWF.Core.Pages.PageDefinition.PageSecurityType.httpsOnly);
#endif

            string token = DateTime.UtcNow.Ticks.ToString();
            string encryptedToken;
            RSACrypto.Encrypt(ivrConfig.PublicKey, token, out encryptedToken);

            object parms = new {
                Url = actionUrl,
                Caller = GetForm("Caller"),
                CallerSpaced = Spaced(GetForm("Caller").TruncateStart("+1")),
                CallerCity = GetForm("CallerCity"),
                CallerCountry = GetForm("CallerCountry"),
                Digits = digits,
                Extension = extension,
                ExtensionSpaced = extensionSpaced,
                ExtensionName = extensionName,
                ErrCount = errCount,
                ErrCountPlus1 = errCount+1,
                Token = encryptedToken,
                Voice = ivrConfig.Voice,
                VoiceInternal = ivrConfig.VoiceInternal,
            };

            string text = entry.Text;
            Variables vars = new Variables(Manager, parms) {
                EncodingType = Variables.EncodingTypeEnum.XML
            };

            Extension ext = script.FindExtension(digits);
            if (ext != null)
                text = RepeatableNumbers(ext, text);

            if (text.Contains("RECORDVOICEMAIL")) {
                text = text.Replace("RECORDVOICEMAIL", "");
                VoiceMailData voiceMail;
                using (VoiceMailDataProvider voiceMailDP = new VoiceMailDataProvider()) {
                    voiceMail = new VoiceMailData {
                        Caller = GetForm("Caller").Truncate(Globals.MaxPhoneNumber),
                        CallerCity = GetForm("CallerCity").Truncate(VoiceMailData.MaxCity),
                        CallerState = GetForm("CallerState").Truncate(VoiceMailData.MaxState),
                        CallerZip = GetForm("CallerZip").Truncate(VoiceMailData.MaxZip),
                        CallerCountry = GetForm("CallerCountry").Truncate(VoiceMailData.MaxCountry),
                        CallSid = GetForm("CallSid"),
                        RecordingSid = GetForm("RecordingSid"),
                        Duration = ConvertToInt(GetForm("RecordingDuration")),
                        To = called,
                        Extension = extension,
                        RecordingUrl = GetForm("RecordingUrl").Truncate(Globals.MaxUrl)
                    };
                    if (!await voiceMailDP.AddItemAsync(voiceMail))
                        Logging.AddErrorLog($"Couldn't record voice mail status for call from {GetForm("Caller")}");
                }
                if (!string.IsNullOrWhiteSpace(extension)) {
                    ext = script.FindExtension(extension);
                    if (ext != null) {
                        DisplayVoiceMailModule dispMod = (DisplayVoiceMailModule)await ModuleDefinition.LoadAsync(ModuleDefinition.GetPermanentGuid(typeof(DisplayVoiceMailModule)));
                        ModuleAction displayAction = await dispMod.GetAction_DisplayAsync(null, voiceMail.Id);
                        if (displayAction != null) {
                            string viewUrl = displayAction.GetCompleteUrl();
                            foreach (ExtensionNumber extNumber in ext.Numbers) {
                                if (extNumber.SendSMSVoiceMail) {
                                    SendSMS sendSMS = new SendSMS();
                                    await sendSMS.SendMessageAsync(extNumber.Number,
                                        this.__ResStr("voiceSMS", "A voice mail was received for extension {0} ({1}) from {2}, {3}, {4}, {5} {6} - {7}",
                                        extension, GetForm("To"), GetForm("Caller"), GetForm("CallerCity"), GetForm("CallerState"), GetForm("CallerZip"), GetForm("CallerCountry"),
                                        viewUrl),
                                        ThrowError: false);
                                }
                            }
                        }
                    }
                }
            }

            text = vars.ReplaceVariables(text);
            Logging.AddLog($"{nameof(RunEntryAsync)}: {text}");
            return Content(text, "text/xml");
        }

        private int ConvertToInt(string duration) {
            try {
                return Convert.ToInt32(duration);
            } catch (Exception) {
                return 0;
            }
        }

        private string RepeatableNumbers(Extension ext, string text) {
            // find a line like this:  REPEATABLENUMBERS <Number url="[Var,Url]?Request=CheckAgent&Extension=[Var,Digits]">[Var,ExtensionNumber]</Number>
            // and add a line entry for each available phone number for the extension (in digits)
            return reRepeatableNumbers.Replace(text, (m) => {
                string ret = "";
                string line = m.Value;
                line = line.TruncateStart("REPEATABLENUMBERS");
                foreach (ExtensionNumber extNumber in ext.Numbers) {
                    object parms = new {
                        ExtensionNumber = extNumber.Number
                    };
                    Variables vars = new Variables(Manager, parms) {
                        EncodingType = Variables.EncodingTypeEnum.XML
                    };
                    ret += vars.ReplaceVariables(line);
                }
                return ret;
            });

        }
        Regex reRepeatableNumbers = new Regex(@"REPEATABLENUMBERS.*?$", RegexOptions.Multiline|RegexOptions.Compiled);

        private string Spaced(string phoneNumber) {
            StringBuilder sb = new StringBuilder();
            foreach (char s in phoneNumber) {
                sb.Append(s); sb.Append(' ');
            }
            return sb.ToString().Trim();
        }

        private bool TryGetForm(string name, out string value) {
            YetaWFManager manager = YetaWFManager.Manager;
            HttpRequest req = manager.CurrentRequest;
            if (req.Form.ContainsKey(name)) {
                value = req.Form[name];
                return true;
            }
            value = null;
            return false;
        }

        private string GetForm(string name) {
            string value;
            if (!TryGetForm(name, out value)) {
#if DEBUG
                throw new InternalError($"{name} not available");
#else
                return "";
#endif
            }
            return value;
        }

        private void LogCall(string action, string extension) {

            YetaWFManager manager = YetaWFManager.Manager;
            HttpRequest req = manager.CurrentRequest;
            StringBuilder sb = new StringBuilder();
            sb.Append($"Extension = {extension??"(none)"}\r\n");
            foreach (var s in req.Form.Keys) {
                sb.Append($"{s} = {req.Form[s]}\r\n");
            }
            Logging.AddLog($"{action}: {sb.ToString()}");
        }
    }
}
