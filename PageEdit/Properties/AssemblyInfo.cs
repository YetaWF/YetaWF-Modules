/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System.Reflection;
using System.Runtime.InteropServices;
using YetaWF.Core.Packages;
using YetaWF.PackageAttributes;

[assembly: AssemblyTitle("Page Editing")]
[assembly: AssemblyDescription("Page editing support and services")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Softel vdm, Inc.")]
[assembly: AssemblyProduct("PageEdit")]
[assembly: AssemblyCopyright("Copyright © 2023 - Softel vdm, Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("5.5.0.0")]

[assembly: Package(PackageTypeEnum.Module, "YetaWF")]
[assembly: PackageInfo("https://YetaWF.com/UpdateServer",
    "https://yetawf.com/Documentation/YetaWF/PageEdit",
    "https://YetaWF.com/Documentation/YetaWF/Support",
    "https://yetawf.com/Documentation/YetaWF/PageEdit#Release%20Notice",
    "https://yetawf.com/Documentation/YetaWF/PageEdit#License")]

[assembly: RequiresPackage("YetaWF.ComponentsHTML")]
[assembly: RequiresPackage("YetaWF.Identity")]
