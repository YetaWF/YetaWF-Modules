/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Search#License */

using System.Reflection;
using System.Runtime.InteropServices;
using YetaWF.PackageAttributes;
using YetaWF.Core.Packages;

[assembly: AssemblyTitle("Search")]
[assembly: AssemblyDescription("keyword search, search management and scheduling")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Softel vdm, Inc.")]
[assembly: AssemblyProduct("Search")]
[assembly: AssemblyCopyright("Copyright © 2017 - Softel vdm, Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("2.0.1.0")]

[assembly: Package(PackageTypeEnum.Module, "YetaWF")]
[assembly: PackageInfo("http://YetaWF.com/UpdateServer",
    "http://yetawf.com/Documentation/YetaWF/Search",
    "http://YetaWF.com/Documentation/YetaWF/Support",
    "http://yetawf.com/Documentation/YetaWF/Search#Release%20Notice",
    "http://yetawf.com/Documentation/YetaWF/Search#License")]

// Required creation order so SearchDataProvider is installed first (in case File I/O is used)
[assembly: InstallOrder(typeof(YetaWF.Modules.Search.DataProvider.SearchDataProvider))]
[assembly: InstallOrder(typeof(YetaWF.Modules.Search.DataProvider.SearchDataUrlDataProvider))]
