/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TwilioProcessorDataProvider#License */

using YetaWF.Core.Packages;

namespace Softelvdm.Modules.TwilioProcessorDataProvider.Controllers {
    public class AreaRegistration : YetaWF.Core.Controllers.AreaRegistrationBase {
        public AreaRegistration() : base() { CurrentPackage = this.GetCurrentPackage(); }
        public static Package CurrentPackage;
    }
}
