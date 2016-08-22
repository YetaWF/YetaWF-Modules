/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/DevTests#License */

using System.Reflection;
using System.Runtime.InteropServices;
using YetaWF.Core.Packages;
using YetaWF.PackageAttributes;

[assembly: AssemblyTitle("DevTests")]
[assembly: AssemblyDescription("Collection of tests used during development - not typically deployed to production sites")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Softel vdm, Inc.")]
[assembly: AssemblyProduct("DevTests")]
[assembly: AssemblyCopyright("Copyright © 2016 - Softel vdm, Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("1.0.5.0")]

[assembly: Package(PackageTypeEnum.Module, "YetaWF")]
[assembly: PackageInfo("http://YetaWF.com/UpdateServer",
    "http://yetawf.com/Documentation/YetaWF/DevTests",
    "http://YetaWF.com/Documentation/YetaWF/Support",
    "http://yetawf.com/Documentation/YetaWF/DevTests#Release%20Notice",
    "http://yetawf.com/Documentation/YetaWF/DevTests#License")]
