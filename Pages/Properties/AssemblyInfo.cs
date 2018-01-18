/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System.Reflection;
using System.Runtime.InteropServices;
using YetaWF.Core.Identity;
using YetaWF.Core.Packages;
using YetaWF.Modules.Pages.Addons;
using YetaWF.PackageAttributes;

[assembly: AssemblyTitle("Pages")]
[assembly: AssemblyDescription("Page management")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Softel vdm, Inc.")]
[assembly: AssemblyProduct("Pages")]
[assembly: AssemblyCopyright("Copyright © 2018 - Softel vdm, Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("2.8.0.0")]

[assembly: Package(PackageTypeEnum.Module, "YetaWF")]
[assembly: PackageInfo("https://YetaWF.com/UpdateServer",
    "https://yetawf.com/Documentation/YetaWF/Pages",
    "https://YetaWF.com/Documentation/YetaWF/Support",
    "https://yetawf.com/Documentation/YetaWF/Pages#Release%20Notice",
    "https://yetawf.com/Documentation/YetaWF/Pages#License")]

[assembly:PublicPartialViews]

[assembly: Resource(Info.Resource_AllowListOfLocalPagesAjax, "Allow list of local pages retrieval (Ajax) for ListOfLocalPages template", Administrator = true, Superuser = true)]
