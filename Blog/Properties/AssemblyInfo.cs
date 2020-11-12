/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System.Reflection;
using System.Runtime.InteropServices;
using YetaWF.Core.Identity;
using YetaWF.Core.Packages;
using YetaWF.Modules.Blog.Addons;
using YetaWF.PackageAttributes;

[assembly: AssemblyTitle("Blog")]
[assembly: AssemblyDescription("Basic blog")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Softel vdm, Inc.")]
[assembly: AssemblyProduct("Blog")]
[assembly: AssemblyCopyright("Copyright © 2020 - Softel vdm, Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("5.3.0.0")]

[assembly: Package(PackageTypeEnum.Module, "YetaWF")]
[assembly: PackageInfo("https://YetaWF.com/UpdateServer",
    "https://yetawf.com/Documentation/YetaWF/Blog",
    "https://YetaWF.com/Documentation/YetaWF/Support",
    "https://yetawf.com/Documentation/YetaWF/Blog#Release%20Notice",
    "https://yetawf.com/Documentation/YetaWF/Blog#License")]

[assembly: RequiresPackage("YetaWF.ComponentsHTML")]
[assembly: RequiresPackage("YetaWF.ImageRepository")]

[assembly: Resource(Info.Resource_AllowManageComments, "Manage comments (edit, approve, etc.)", Administrator = true, Superuser = true)]
