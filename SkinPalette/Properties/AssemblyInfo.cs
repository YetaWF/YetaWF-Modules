/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SkinPalette#License */

using System.Reflection;
using System.Runtime.InteropServices;
using YetaWF.Core.Packages;
using YetaWF.PackageAttributes;

[assembly: AssemblyTitle("SkinPalette")]
[assembly: AssemblyDescription("SkinPalette description")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Softelvdm")]
[assembly: AssemblyProduct("SkinPalette")]
[assembly: AssemblyCopyright("Copyright © 2021 - Softelvdm")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("5.4.0.0")]

[assembly: Package(PackageTypeEnum.Module, "YetaWF")]
[assembly: PackageInfo("https://Softelvdm.com/UpdateServer",
    "https://Softelvdm.com/Documentation/YetaWF/SkinPalette",
    "https://Softelvdm.com/Documentation/YetaWF/Support",
    "https://Softelvdm.com/Documentation/YetaWF/SkinPalette#Release%20Notice",
    "https://Softelvdm.com/Documentation/YetaWF/SkinPalette#License")]

[assembly: RequiresPackage("YetaWF.ComponentsHTML")] // needed for HTML components (can be removed if no views/components are implemented by this package)
