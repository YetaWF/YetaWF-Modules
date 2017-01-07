/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/DevTests#License */

using YetaWF.Core.Packages;

namespace YetaWF.Modules.DevTests.Controllers {
    public class AreaRegistration : YetaWF.Core.Controllers.AreaRegistrationBase {
        public AreaRegistration() : base() { CurrentPackage = this.GetCurrentPackage(); }
        public static Package CurrentPackage;
    }
}
