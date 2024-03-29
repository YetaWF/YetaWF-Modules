/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System.Reflection;
using System.Runtime.InteropServices;
using YetaWF.Core.Packages;
using YetaWF.PackageAttributes;

[assembly: AssemblyTitle("Search")]
[assembly: AssemblyDescription("keyword search, search management and scheduling")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Softel vdm, Inc.")]
[assembly: AssemblyProduct("Search")]
[assembly: AssemblyCopyright("Copyright © 2023 - Softel vdm, Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("5.5.0.0")]

[assembly: Package(PackageTypeEnum.Module, "YetaWF")]
[assembly: PackageInfo("https://YetaWF.com/UpdateServer",
    "https://yetawf.com/Documentation/YetaWF/Search",
    "https://YetaWF.com/Documentation/YetaWF/Support",
    "https://yetawf.com/Documentation/YetaWF/Search#Release%20Notice",
    "https://yetawf.com/Documentation/YetaWF/Search#License")]

[assembly: RequiresPackage("YetaWF.ComponentsHTML")]

// Required creation order so SearchDataProvider is installed first (in case File I/O is used)
[assembly: InstallOrder(typeof(YetaWF.Modules.Search.DataProvider.SearchDataProvider))]
[assembly: InstallOrder(typeof(YetaWF.Modules.Search.DataProvider.SearchDataUrlDataProvider))]
