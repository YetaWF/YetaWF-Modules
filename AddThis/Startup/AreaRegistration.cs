/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/AddThis#License */

using YetaWF.Core.Packages;

namespace YetaWF.Modules.AddThis.Controllers {
    public class AreaRegistration : YetaWF.Core.Controllers.AreaRegistrationBase {
        public AreaRegistration() : base() { CurrentPackage = this.GetCurrentPackage(); }
        public static Package CurrentPackage;
    }
}
