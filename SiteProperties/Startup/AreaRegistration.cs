/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/SiteProperties#License */

using YetaWF.Core.Packages;

namespace YetaWF.Modules.SiteProperties.Controllers {
    public class AreaRegistration : YetaWF.Core.Controllers.AreaRegistration {
        public AreaRegistration() : base(out CurrentPackage) { }
        public static new Package CurrentPackage;
    }
}
