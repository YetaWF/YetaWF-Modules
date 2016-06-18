/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/TinyLogin#License */

using YetaWF.Core.Packages;

namespace YetaWF.Modules.TinyLogin.Controllers {
    public class AreaRegistration : YetaWF.Core.Controllers.AreaRegistration {
        public AreaRegistration() : base(out CurrentPackage) { }
        public static new Package CurrentPackage;
    }
}
