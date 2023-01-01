/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Lightbox#License */

using YetaWF.Core.Packages;

namespace YetaWF.Modules.Lightbox {
    /// <inheritdoc/>
    public class AreaRegistration : YetaWF.Core.Controllers.AreaRegistrationBase {
        /// <summary>
        /// Defines the current package, used by applications that need access to the YetaWF.Core.Packages.Package instance.
        /// </summary>
        public static Package CurrentPackage { get { return _CachedCurrentPackage ??= (_CachedCurrentPackage = Package.GetPackageFromAssembly(typeof(AreaRegistration).Assembly)); } }
        private static Package? _CachedCurrentPackage;
    }
}
