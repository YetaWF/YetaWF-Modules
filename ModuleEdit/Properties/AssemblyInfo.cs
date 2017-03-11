/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/ModuleEdit#License */

using System.Reflection;
using System.Runtime.InteropServices;
using YetaWF.Core.Packages;
using YetaWF.PackageAttributes;

[assembly: AssemblyTitle("Module Editing")]
[assembly: AssemblyDescription("Module editing support and services")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Softel vdm, Inc.")]
[assembly: AssemblyProduct("ModuleEdit")]
[assembly: AssemblyCopyright("Copyright © 2017 - Softel vdm, Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("2.0.0.0")]

[assembly: Package(PackageTypeEnum.Module, "YetaWF")]
[assembly: PackageInfo("http://YetaWF.com/UpdateServer",
    "http://yetawf.com/Documentation/YetaWF/ModuleEdit",
    "http://YetaWF.com/Documentation/YetaWF/Support",
    "http://yetawf.com/Documentation/YetaWF/ModuleEdit#Release%20Notice",
    "http://yetawf.com/Documentation/YetaWF/ModuleEdit#License")]

[assembly: PublicPartialViews]

[assembly: RequiresPackage("YetaWF.Identity")]