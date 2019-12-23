/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEar#License */

using System.Reflection;
using System.Runtime.InteropServices;
using YetaWF.Core.Packages;
using YetaWF.PackageAttributes;

[assembly: AssemblyTitle("PageEar")]
[assembly: AssemblyDescription("Page ear support (page peel, page corner) for site advertisement")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Softel vdm, Inc.")]
[assembly: AssemblyProduct("PageEar")]
[assembly: AssemblyCopyright("Copyright © 2020 - Softel vdm, Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("5.1.0.0")]

[assembly: Package(PackageTypeEnum.Module, "YetaWF")]
[assembly: PackageInfo("https://YetaWF.com/UpdateServer",
    "https://yetawf.com/Documentation/YetaWF/PageEar",
    "https://YetaWF.com/Documentation/YetaWF/Support",
    "https://yetawf.com/Documentation/YetaWF/PageEar#Release%20Notice",
    "https://yetawf.com/Documentation/YetaWF/PageEar#License")]

[assembly: RequiresPackage("YetaWF.ComponentsHTML")]
