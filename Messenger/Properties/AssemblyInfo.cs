/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System.Reflection;
using System.Runtime.InteropServices;
using YetaWF.Core.Packages;
using YetaWF.PackageAttributes;

[assembly: AssemblyTitle("Messenger")]
[assembly: AssemblyDescription("Messaging support and site announcements")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Softel vdm, Inc.")]
[assembly: AssemblyProduct("Messenger")]
[assembly: AssemblyCopyright("Copyright © 2018 - Softel vdm, Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("1.0.0.0")]

[assembly: Package(PackageTypeEnum.Module, "YetaWF")]
[assembly: PackageInfo("https://YetaWF.com/UpdateServer",
    "https://YetaWF.com/Documentation/YetaWF/Messenger",
    "https://YetaWF.com/Documentation/YetaWF/Support",
    "https://YetaWF.com/Documentation/YetaWF/Messenger#Release%20Notice",
    "https://YetaWF.com/Documentation/YetaWF/Messenger#License")]

[assembly: RequiresPackage("YetaWF.ComponentsHTML")]
