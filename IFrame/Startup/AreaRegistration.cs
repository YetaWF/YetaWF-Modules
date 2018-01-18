/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IFrame#License */

using YetaWF.Core.Packages;

namespace YetaWF.Modules.IFrame.Controllers {
    public class AreaRegistration : YetaWF.Core.Controllers.AreaRegistrationBase {
        public AreaRegistration() : base() { CurrentPackage = this.GetCurrentPackage(); }
        public static Package CurrentPackage;
    }
}
