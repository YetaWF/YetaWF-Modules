/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Modules.ComponentsHTML.Components;

namespace Softelvdm.Modules.IVR.Components {

    public abstract class PhoneNumberComponentBase : YetaWFComponent {

        public const string TemplateName = "PhoneNumber";
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public static string FormatPhoneNumber(string? phoneNumber) {
            if (phoneNumber == null || !phoneNumber.StartsWith("+1") || phoneNumber.Length != 12)
                return phoneNumber ?? string.Empty;
            return $"({phoneNumber.Substring(2, 3)}) {phoneNumber.Substring(5, 3)}-{phoneNumber.Substring(8, 4)}";
        }
    }
    /// <summary>
    /// Displays a phone number. If the model is null, nothing is rendered.
    /// </summary>
    /// <remarks>
    /// This component has limited features. A better alternative is the Softelvdm_TwilioProcessor_PhoneNumber component.
    /// </remarks>
    /// <example>
    /// [Caption("From"), Description("The caller's phone number")]
    /// [UIHint("Softelvdm_IVR_PhoneNumber"), ReadOnly]
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
}
