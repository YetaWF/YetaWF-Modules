/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TinyLogin#License */

using YetaWF.Core.Packages;

namespace YetaWF.Modules.TinyLogin.Controllers {
    public class AreaRegistration : YetaWF.Core.Controllers.AreaRegistrationBase {
        public AreaRegistration() : base() { CurrentPackage = this.GetCurrentPackage(); }
        public static Package CurrentPackage;
    }
}
