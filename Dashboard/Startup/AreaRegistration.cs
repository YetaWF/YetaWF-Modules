/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using YetaWF.Core.Packages;

namespace YetaWF.Modules.Dashboard;

/// <summary>
/// Holds information about the current package.
/// </summary>
public static class AreaRegistration {
    /// <summary>
    /// Defines the current package, used by applications that need access to the YetaWF.Core.Packages.Package instance.
    /// </summary>
    public static Package CurrentPackage { get; set; } = null!;
}
