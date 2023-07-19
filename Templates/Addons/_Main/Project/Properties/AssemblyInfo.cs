using System.Reflection;
using System.Runtime.InteropServices;
using YetaWF.Core.Packages;
using YetaWF.PackageAttributes;

[assembly: AssemblyTitle("$projectname$")]
[assembly: AssemblyDescription("$projectname$ description")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("$companyname$")]
[assembly: AssemblyProduct("$projectnamespace$")]
[assembly: AssemblyCopyright("Copyright © 2023 - $companyname$")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("1.0.0.0")]

[assembly: Package(PackageTypeEnum.Module, "$companynamespace$")]
[assembly: PackageInfo("https://$companyurl$/UpdateServer",
    "https://$companyurl$/Documentation/YetaWF/$projectname$",
    "https://$companyurl$/Documentation/YetaWF/Support",
    "https://$companyurl$/Documentation/YetaWF/$projectname$#Release%20Notice",
    "https://$companyurl$/Documentation/YetaWF/$projectname$#License")]

[assembly: RequiresPackage("YetaWF.ComponentsHTML")] // needed for HTML components (can be removed if no views/components are implemented by this package)

// TODO:    The following is only needed if this package contains a skin.
//          All modules used by a skin must reference the package (Add Reference) and the RequiresPackage attributes must list each referenced package.
//
//[assembly: RequiresPackage("YetaWF.Menus")]
//[assembly: RequiresPackage("YetaWF.Text")]
//[assembly: RequiresPackage("YetaWF.TinyLogin")]
