/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;

namespace Softelvdm.Modules.IVR.Components {

    public abstract class PhoneNumberComponentBase : YetaWFComponent {

        public const string TemplateName = "PhoneNumber";
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public static string FormatPhoneNumber(string phoneNumber) {
            if (phoneNumber == null || !phoneNumber.StartsWith("+1") || phoneNumber.Length != 12)
                return phoneNumber;
            return $"({phoneNumber.Substring(2, 3)}) {phoneNumber.Substring(5, 3)}-{phoneNumber.Substring(8, 4)}";
        }
    }
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
