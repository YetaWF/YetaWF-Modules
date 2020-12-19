/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using System.Reflection;
using System.Runtime.InteropServices;
using YetaWF.Core.Packages;
using YetaWF.PackageAttributes;

[assembly: AssemblyTitle("Caching")]
[assembly: AssemblyDescription("Caching and locking services")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Softel vdm, Inc.")]
[assembly: AssemblyProduct("Caching")]
[assembly: AssemblyCopyright("Copyright © 2021 - Softel vdm, Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("5.4.0.0")]

[assembly: Package(PackageTypeEnum.Module, "YetaWF")]
[assembly: PackageInfo("http://YetaWF.com/UpdateServer",
    "https://YetaWF.com/Documentation/YetaWFCaching",
    "https://YetaWF.com/Documentation/YetaWFCaching#Support",
    "https://YetaWF.com/Documentation/YetaWFCaching#Release%20Notice",
    "https://YetaWF.com/Documentation/YetaWFCaching#License")]

[assembly: ServiceLevel(ServiceLevelEnum.CachingProvider)]
