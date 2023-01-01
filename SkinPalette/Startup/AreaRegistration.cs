/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SkinPalette#License */

using YetaWF.Core.Packages;

namespace YetaWF.Modules.SkinPalette {

    /// <inheritdoc/>
    public class AreaRegistration : YetaWF.Core.Controllers.AreaRegistrationBase {
        /// <inheritdoc/>
        public static Package CurrentPackage { get { return _CachedCurrentPackage ??= (_CachedCurrentPackage = Package.GetPackageFromAssembly(typeof(AreaRegistration).Assembly)); } }
        private static Package? _CachedCurrentPackage;
    }
}
