using YetaWF.Core.Packages;

namespace $companynamespace$.Modules.$projectnamespace$;

/// <summary>
/// Holds information about the current package.
/// </summary>
public static class AreaRegistration {
    /// <summary>
    /// Defines the current package, used by applications that need access to the YetaWF.Core.Packages.Package instance.
    /// </summary>
    public static Package CurrentPackage { get; set; } = null!;
}
