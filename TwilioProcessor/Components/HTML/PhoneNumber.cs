/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TwilioProcessor#License */

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Softelvdm.Modules.TwilioProcessorDataProvider.Models.Attributes;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Modules.ComponentsHTML.Components;

namespace Softelvdm.Modules.TwilioProcessor.Components {

    public abstract class PhoneNumberComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(PhoneNumberComponentBase), name, defaultValue, parms); }

        public const string TemplateName = "PhoneNumber";
        public override Package GetPackage() { return TwilioProcessor.Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public static string FormatPhoneNumber(string phoneNumber) {
            return PhoneNumberAttribute.GetDisplay(phoneNumber);
        }
    }
    /// <summary>
    /// Displays a phone number. The model represents a phone number in E164 ISO format. If the model is null, nothing is rendered.
    /// The phone number is rendered in a format typically used by callers, taking into account the site's country (see Admin > Settings > Site Settings, Site tab, Country field).
    /// Phone numbers are automatically formatted as international or domestic phone numbers.
    /// </summary>
    /// <remarks>
    /// All phone numbers used in YetaWF are internally stored in E164 ISO format. For details about E164 see https://en.wikipedia.org/wiki/E.164.
    /// </remarks>
    /// <example>
    /// [Caption("From"), Description("The caller's phone number")]
    /// [UIHint("Softelvdm_TwilioProcessor_PhoneNumber"), ReadOnly]
    /// public string PhoneNumber { get; set; }
    /// </example>
    public class PhoneNumberDisplayComponent : PhoneNumberComponentBase, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<string> RenderAsync(string model) {
            string text = "";
            if (!string.IsNullOrWhiteSpace(model))
                text = FormatPhoneNumber(model);
            return Task.FromResult(text);
        }
    }
    /// <summary>
    /// Allows entry of a phone number. The model represents a phone number in E164 ISO format.
    /// </summary>
    /// <remarks>
    /// The phone number entered can contain country codes, area codes and special characters.
    /// The model returned will contain a phone number in E164 ISO format.
    ///
    /// All phone numbers used in YetaWF are internally stored in E164 ISO format. For details about E164 see https://en.wikipedia.org/wiki/E.164.
    /// </remarks>
    /// <example>
    /// [Caption("From"), Description("The caller's phone number")]
    /// [UIHint("Softelvdm_TwilioProcessor_PhoneNumber")]
    /// public string PhoneNumber { get; set; }
    /// </example>
    public class PhoneNumberEditComponent : PhoneNumberComponentBase, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<string> RenderAsync(string model) {
            string display = PhoneNumberAttribute.GetDisplay(model);
            if (!string.IsNullOrWhiteSpace(display))
                model = display;
            return await TextEditComponentBase.RenderTextAsync(this, model, "yt_softelvdm_twilioprocessor_phonenumber");
        }

        /// <summary>
        /// Called before action runs.
        /// </summary>
        /// <remarks>Used to normalize all phone numbers to E164 format.</remarks>
        public static Task<string> ControllerPreprocessActionAsync(string propName, string model, ModelStateDictionary modelState) {
            if (!string.IsNullOrWhiteSpace(model)) {
                string number = PhoneNumberAttribute.GetE164(model);
                if (!string.IsNullOrWhiteSpace(number))
                    model = number;
                else
                    modelState.AddModelError(propName, __ResStr("invPhone", "{0} is an invalid phone number", model));
            }
            return Task.FromResult(model);
        }
    }
}
