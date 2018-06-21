/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Reflection;
using System.Runtime.InteropServices;
using YetaWF.Core.Packages;
using YetaWF.PackageAttributes;

[assembly: AssemblyTitle("ComponentsHTML")]
[assembly: AssemblyDescription("Components based on HTML (jQuery, jQuery-UI, Kendo UI)")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Softel vdm, Inc.")]
[assembly: AssemblyProduct("ComponentsHTML")]
[assembly: AssemblyCopyright("Copyright © 2018 - Softel vdm, Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("3.0.0.0")]

[assembly: Package(PackageTypeEnum.Module, "YetaWF")]
[assembly: PackageInfo("https://YetaWF.com/UpdateServer",
    "https://YetaWF.com/Documentation/YetaWF/ComponentsHTML",
    "https://YetaWF.com/Documentation/YetaWF/Support",
    "https://YetaWF.com/Documentation/YetaWF/ComponentsHTML#Release%20Notice",
    "https://YetaWF.com/Documentation/YetaWF/ComponentsHTML#License")]

[assembly: RequiresPackage("YetaWF.ComponentsHTML")]
