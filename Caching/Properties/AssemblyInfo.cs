/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using System.Reflection;
using System.Runtime.InteropServices;
using YetaWF.Core.Packages;
using YetaWF.PackageAttributes;

[assembly: AssemblyTitle("Caching")]
[assembly: AssemblyDescription("Caching description")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Softel vdm, Inc.")]
[assembly: AssemblyProduct("Caching")]
[assembly: AssemblyCopyright("Copyright © 2019 - Softel vdm, Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("4.3.0.0")]

[assembly: Package(PackageTypeEnum.Module, "YetaWF")]
[assembly: PackageInfo("http://YetaWF.com/UpdateServer",
    "http://YetaWF.com/Documentation/YetaWF/Caching",
    "http://YetaWF.com/Documentation/YetaWF/Support",
    "http://YetaWF.com/Documentation/YetaWF/Caching#Release%20Notice",
    "http://YetaWF.com/Documentation/YetaWF/Caching#License")]

[assembly: ServiceLevel(ServiceLevelEnum.CachingProvider)]
