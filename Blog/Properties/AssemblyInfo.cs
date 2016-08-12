/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Blog#License */

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
[assembly: AssemblyCopyright("Copyright © 2016 - Softel vdm, Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("1.0.3.0")]

[assembly: Package(PackageTypeEnum.Module, "YetaWF")]
[assembly: PackageInfo("http://YetaWF.com/UpdateServer",
    "http://yetawf.com/Documentation/YetaWF/Blog",
    "http://YetaWF.com/Documentation/YetaWF/Support",
    "http://yetawf.com/Documentation/YetaWF/Blog#Release%20Notice",
    "http://yetawf.com/Documentation/YetaWF/Blog#License")]

[assembly: RequiresPackage("YetaWF.ImageRepository")]

[assembly: Resource(Info.Resource_AllowManageComments, "Manage comments (edit, approve, etc.)", Administrator = true, Superuser = true)]
