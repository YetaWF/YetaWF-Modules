/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/KeepAlive#License */

using YetaWF.Core.Packages;

namespace YetaWF.Modules.KeepAlive.Controllers {
    public class AreaRegistration : YetaWF.Core.Controllers.AreaRegistrationBase {
        public AreaRegistration() : base() { CurrentPackage = this.GetCurrentPackage(); }
        public static Package CurrentPackage;
    }
}
