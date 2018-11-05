using System.Reflection;
using System.Runtime.InteropServices;
using YetaWF.Core.Packages;
using YetaWF.PackageAttributes;

[assembly: AssemblyTitle("Content")]
[assembly: AssemblyDescription("Content editing support")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Softel vdm, Inc.")]
[assembly: AssemblyProduct("Content")]
[assembly: AssemblyCopyright("Copyright © 2018 - Softel vdm, Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("4.0.0.0")]

[assembly: Package(PackageTypeEnum.Module, "YetaWF")]
[assembly: PackageInfo("https://YetaWF.com/UpdateServer",
    "https://YetaWF.com/Documentation/YetaWF/Content",
    "https://YetaWF.com/Documentation/YetaWF/Support",
    "https://YetaWF.com/Documentation/YetaWF/Content#Release%20Notice",
    "https://YetaWF.com/Documentation/YetaWF/Content#License")]

[assembly: RequiresPackage("YetaWF.ComponentsHTML")] // needed for HTML components (can be removed if no views/components are implemented by this package)
