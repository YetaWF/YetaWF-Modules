/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/UserProfile#License */

using YetaWF.Core.Packages;

namespace YetaWF.Modules.UserProfile.Controllers {
    public class AreaRegistration : YetaWF.Core.Controllers.AreaRegistrationBase { 
        public AreaRegistration() : base() { CurrentPackage = this.GetCurrentPackage(); }
        public static Package CurrentPackage;
    }
}
