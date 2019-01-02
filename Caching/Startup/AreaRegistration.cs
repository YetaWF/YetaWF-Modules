/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using YetaWF.Core.Packages;

namespace YetaWF.Modules.Caching.Controllers {
    public class AreaRegistration : YetaWF.Core.Controllers.AreaRegistrationBase { 
        public AreaRegistration() : base() { CurrentPackage = this.GetCurrentPackage(); }
        public static Package CurrentPackage;
    }
}
