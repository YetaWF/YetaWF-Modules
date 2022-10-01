/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SitePropertiesService#License */

using System.Reflection;
using System.Runtime.InteropServices;
using YetaWF.Core.Packages;
using YetaWF.PackageAttributes;

[assembly: AssemblyTitle("Site Properties for Services/Batch")]
[assembly: AssemblyDescription("Site properties support for services/batch")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Softel vdm, Inc.")]
[assembly: AssemblyProduct("SitePropertiesService")]
[assembly: AssemblyCopyright("Copyright © 2022 - Softel vdm, Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("5.5.0.0")]

[assembly: Package(PackageTypeEnum.Module, "YetaWF")]
[assembly: PackageInfo("https://YetaWF.com/UpdateServer",
    "https://yetawf.com/Documentation/YetaWF/SitePropertiesService",
    "https://YetaWF.com/Documentation/YetaWF/Support",
    "https://yetawf.com/Documentation/YetaWF/SitePropertiesService#Release%20Notice",
    "https://yetawf.com/Documentation/YetaWF/SitePropertiesService#License")]

[assembly: RequiresPackage("YetaWF.ComponentsHTML")]
