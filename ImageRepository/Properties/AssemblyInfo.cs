/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using System.Reflection;
using System.Runtime.InteropServices;
using YetaWF.PackageAttributes;
using YetaWF.Core.Packages;

[assembly: AssemblyTitle("Image Repository")]
[assembly: AssemblyDescription("Image repository for uploaded images (with IHttpHandler support)")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Softel vdm, Inc.")]
[assembly: AssemblyProduct("ImageRepository")]
[assembly: AssemblyCopyright("Copyright © 2016 - Softel vdm, Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("1.1.0.0")]

[assembly: Package(PackageTypeEnum.Module, "YetaWF")]
[assembly: PackageInfo("http://YetaWF.com/UpdateServer",
    "http://yetawf.com/Documentation/YetaWF/ImageRepository",
    "http://YetaWF.com/Documentation/YetaWF/Support",
    "http://yetawf.com/Documentation/YetaWF/ImageRepository#Release%20Notice",
    "http://yetawf.com/Documentation/YetaWF/ImageRepository#License")]

[assembly: PublicPartialViews]

